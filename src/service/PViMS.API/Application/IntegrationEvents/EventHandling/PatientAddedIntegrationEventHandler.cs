using MediatR;
using Microsoft.Extensions.Logging;
using PViMS.BuildingBlocks.EventBus.Abstractions;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace PVIMS.API.Application.IntegrationEvents.EventHandling
{
    public class PatientAddedIntegrationEventHandler : IIntegrationEventHandler<PatientAddedIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PatientAddedIntegrationEventHandler> _logger;

        public PatientAddedIntegrationEventHandler(
            IMediator mediator,
            ILogger<PatientAddedIntegrationEventHandler> logger)
        {
            _mediator = mediator;
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
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

            var command = new AddPatientCommand(
                "Test",
                "Patient",
                "",
                new System.DateTime(2022, 01, 01, 08, 00, 00, 00),
                "Test Facility",
                patientForCreation.ConditionGroupId,
                patientForCreation.MeddraTermId,
                patientForCreation.CohortGroupId,
                patientForCreation.EnroledDate,
                patientForCreation.StartDate,
                patientForCreation.OutcomeDate,
                patientForCreation.CaseNumber,
                "",
                patientForCreation.EncounterTypeId,
                patientForCreation.PriorityId,
                patientForCreation.EncounterDate,
                patientForCreation.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                $"----- Sending command: AddPatientCommand - {@event.Identifier} ({command})");

            await _mediator.Send(command);
        }
    }
}
