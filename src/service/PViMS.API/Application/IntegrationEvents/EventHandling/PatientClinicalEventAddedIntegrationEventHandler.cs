using MediatR;
using Microsoft.Extensions.Logging;
using PViMS.BuildingBlocks.EventBus.Abstractions;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.IntegrationEvents.Events;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PVIMS.API.Application.IntegrationEvents.EventHandling
{
    public class PatientClinicalEventAddedIntegrationEventHandler 
        : IIntegrationEventHandler<PatientClinicalEventAddedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IMediator _mediator;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<PatientClinicalEventAddedIntegrationEventHandler> _logger;

        public PatientClinicalEventAddedIntegrationEventHandler(
            IMediator mediator,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IIntegrationEventService integrationEventService,
            IEventBus eventBus,
            ILogger<PatientClinicalEventAddedIntegrationEventHandler> logger)
        {
            _mediator = mediator;
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Event handler which confirms that the grace period
        /// has been completed and order will not initially be cancelled.
        /// Therefore, the order process continues for validation. 
        /// </summary>
        /// <param name="event">       
        /// </param>
        /// <returns></returns>
        public async Task Handle(PatientClinicalEventAddedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Transaction_Id, @event);

            var processed = true;
            var message = string.Empty;

            try
            {
                var attributes = await PrepareAttributesAsync(@event);

                var patientFromRepo = await _patientRepository.GetAsync(p => p.PatientGuid == @event.Patient_Guid, new string[] { "PatientClinicalEvents.SourceTerminologyMedDra" });
                if (patientFromRepo == null)
                {
                    throw new KeyNotFoundException($"Unable to find patient {@event.Patient_Guid}");
                }

                var command = new AddClinicalEventToPatientCommand(
                    patientId: patientFromRepo.Id,
                    patientIdentifier: String.Empty,
                    sourceDescription: @event.Source_Description,
                    sourceTerminologyMedDraId: null,
                    onsetDate: Convert.ToDateTime(@event.Onset_Date.Substring(0, 10)),
                    resolutionDate: null,
                    attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    $"----- Sending command: AddClinicalEventToPatientCommand - {@event.Transaction_Id} ({command})");

               var patient = await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                processed = false;
                message = ex.Message;
            }

            var ackEvent = new AcknowledgementIntegrationEvent(@event.Transaction_Id, processed, message);
            var source = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await _integrationEventService.AddAndSaveEventAsync(ackEvent, source);

            _eventBus.Publish(ackEvent);
        }

        private async Task<ICollection<AttributeValueForPostDto>> PrepareAttributesAsync(PatientClinicalEventAddedIntegrationEvent @event)
        {
            ICollection<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();

            var nameOfReporterAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent" 
                && ca.AttributeKey == "Name of reporter");
            if (nameOfReporterAttribute != null && !String.IsNullOrWhiteSpace(@event.Full_Name_Reporter))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = nameOfReporterAttribute.Id, Value = @event.Full_Name_Reporter });
            }

            var contactNumberAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent" 
                && ca.AttributeKey == "Contact number");
            if (contactNumberAttribute != null && !String.IsNullOrWhiteSpace(@event.Reporter_Contact_Number))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = contactNumberAttribute.Id, Value = @event.Reporter_Contact_Number });
            }

            var contactEmailAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Email address");
            if (contactEmailAttribute != null && !String.IsNullOrWhiteSpace(@event.Reporter_Contact_Email))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = contactEmailAttribute.Id, Value = @event.Reporter_Contact_Email });
            }

            var professionAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Profession");
            if (professionAttribute != null && !String.IsNullOrWhiteSpace(@event.Reporter_Type))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = professionAttribute.Id, Value = @event.Reporter_Type });
            }

            var reportDateAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Date of report");
            if (reportDateAttribute != null && !String.IsNullOrWhiteSpace(@event.Report_Date))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = reportDateAttribute.Id, Value = @event.Report_Date });
            }

            var severityGradeAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Severity Grade");
            //var selectionDataItem = await _selectionDataItemRepository.GetAsync(s => s.AttributeKey == "Severity Grade" && s.Value == @event.Severity_Grade);
            //if (severityGradeAttribute != null && selectionDataItem != null)
            //{
            //    attributes.Add(new AttributeValueForPostDto() { Id = severityGradeAttribute.Id, Value = selectionDataItem.SelectionKey });
            //}
            if (severityGradeAttribute != null && !String.IsNullOrWhiteSpace(@event.Severity_Grade))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = severityGradeAttribute.Id, Value = @event.Severity_Grade });
            }

            var outcomeAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Outcome");
            if (outcomeAttribute != null && !String.IsNullOrWhiteSpace(@event.Outcome))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = outcomeAttribute.Id, Value = @event.Outcome });
            }

            var intensityAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Intensity (Severity)");
            if (intensityAttribute != null && !String.IsNullOrWhiteSpace(@event.Intensity))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = intensityAttribute.Id, Value = @event.Intensity });
            }

            var seriousnessAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Seriousness)");
            if (seriousnessAttribute != null && !String.IsNullOrWhiteSpace(@event.Seriousness))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = seriousnessAttribute.Id, Value = @event.Seriousness });
            }

            var commentsAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Comments)");
            if (commentsAttribute != null && !String.IsNullOrWhiteSpace(@event.Comments))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = commentsAttribute.Id, Value = @event.Comments });
            }

            var treatmentAttribute = await _customAttributeRepository.GetAsync(ca => ca.ExtendableTypeName == "PatientClinicalEvent"
                && ca.AttributeKey == "Treatment details");
            if (treatmentAttribute != null && !String.IsNullOrWhiteSpace(@event.Treatment_Of_Reaction))
            {
                attributes.Add(new AttributeValueForPostDto() { Id = treatmentAttribute.Id, Value = @event.Treatment_Of_Reaction });
            }

            return attributes;
        }
    }
}
