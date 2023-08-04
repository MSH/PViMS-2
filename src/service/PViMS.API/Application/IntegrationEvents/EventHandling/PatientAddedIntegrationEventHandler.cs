using DocumentFormat.OpenXml.VariantTypes;
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
    public class PatientAddedIntegrationEventHandler : IIntegrationEventHandler<PatientAddedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IMediator _mediator;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<PatientAddedIntegrationEventHandler> _logger;

        public PatientAddedIntegrationEventHandler(
            IMediator mediator,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IIntegrationEventService integrationEventService,
            IEventBus eventBus,
            ILogger<PatientAddedIntegrationEventHandler> logger)
        {
            _mediator = mediator;
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
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
        public async Task Handle(PatientAddedIntegrationEvent @event)
        {
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Transaction_Id, @event);

            var attributes = await PrepareAttributesAsync(@event);

            var command = new AddPatientCommand(
                firstName: @event.First_Name,
                lastName: @event.Last_Name,
                middleName: String.Empty,
                dateOfBirth: Convert.ToDateTime(@event.Date_Of_Birth.Substring(0, 10)),
                facilityName: @event.Facility_Name,
                conditionGroupId: 1,
                meddraTermId: 52627,
                cohortGroupId: 2,
                enroledDate: null,
                startDate: DateTime.Today,
                outcomeDate: null,
                caseNumber: @event.Episode_Id.ToString(),
                comments: String.Empty,
                encounterTypeId: 1,
                priorityId: 1,
                encounterDate: @event.Creation_Date.Date,
                attributes: attributes.ToDictionary(x => x.Id, x => x.Value),
                originatorGuid: @event.Patient_Guid,
                originatorPatientGuid: @event.Patient_Guid,
                created: @event.Creation_Date);

            _logger.LogInformation(
                $"----- Sending command: AddPatientCommand - {@event.Transaction_Id} ({command})");

            var processed = true;
            var errorMessage = string.Empty;
            try
            {
                var patient = await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                processed = false;
                errorMessage = ex.Message;
            }

            var ackEvent = new AcknowledgementIntegrationEvent(@event.Transaction_Id, processed, errorMessage);
            var source = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await _integrationEventService.AddAndSaveEventAsync(ackEvent, source);

            _eventBus.Publish(ackEvent);
        }

        private async Task<ICollection<AttributeValueForPostDto>> PrepareAttributesAsync(PatientAddedIntegrationEvent @event)
        {
            ICollection<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();

            var genderCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Gender");
            var selectionDataItem = await _selectionDataItemRepository.GetAsync(s => s.AttributeKey == "Gender" && s.Value == @event.Gender);
            if (genderCustomAttribute != null && selectionDataItem != null)
            {
                attributes.Add(new AttributeValueForPostDto() { Id = genderCustomAttribute.Id, Value = selectionDataItem.SelectionKey });
            }

            var medicalRecordNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Medical Record Number");
            if (medicalRecordNumberCustomAttribute != null)
            {
                attributes.Add(new AttributeValueForPostDto() { Id = medicalRecordNumberCustomAttribute.Id, Value = @event.Registration_No.ToString() });
            }

            return attributes;
        }
    }
}
