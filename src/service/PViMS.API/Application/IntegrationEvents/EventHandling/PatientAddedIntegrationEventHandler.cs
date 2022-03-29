using MediatR;
using Microsoft.Extensions.Logging;
using PViMS.BuildingBlocks.EventBus.Abstractions;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.IntegrationEvents.Events;
using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Application.IntegrationEvents.EventHandling
{
    public class PatientAddedIntegrationEventHandler : IIntegrationEventHandler<PatientAddedIntegrationEvent>
    {
        private readonly IEventBus _eventBus;
        private readonly IMediator _mediator;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<PatientAddedIntegrationEventHandler> _logger;

        public PatientAddedIntegrationEventHandler(
            IMediator mediator,
            IIntegrationEventService integrationEventService,
            IEventBus eventBus,
            ILogger<PatientAddedIntegrationEventHandler> logger)
        {
            _mediator = mediator;
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
            _logger.LogInformation("----- Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.TransactionId, @event);

            ICollection<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();
            attributes.Add(new AttributeValueForPostDto()
            {
                Id = 1,
                Value = @event.PersonId.ToString()
            });
            attributes.Add(new AttributeValueForPostDto()
            {
                Id = 2,
                Value = @event.RegistrationId.ToString()
            });
            attributes.Add(new AttributeValueForPostDto()
            {
                Id = 4,
                Value = "1"
            });
            attributes.Add(new AttributeValueForPostDto()
            {
                Id = 5,
                Value = "N/A"
            });

            var command = new AddPatientCommand(
                "EDRWeb",
                "Patient",
                "",
                new DateTime(2022, 01, 01, 08, 00, 00, 00),
                "Test Facility",
                1,
                53312,
                null,
                null,
                new DateTime(2022, 03, 01, 08, 00, 00, 00),
                null,
                @event.RegistrationNo.ToString(),
                "",
                1,
                1,
                DateTime.Today,
                attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                $"----- Sending command: AddPatientCommand - {@event.TransactionId} ({command})");

            var patient = await _mediator.Send(command);

            var ackEvent = new PatientAddedAckIntegrationEvent(@event.TransactionId);
            //await _integrationEventService.AddAndSaveEventAsync(ackEvent);

            _eventBus.Publish(ackEvent);
        }
    }
}
