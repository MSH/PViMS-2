using MediatR;
using Microsoft.Extensions.Logging;
using PViMS.Core.Events;
using PVIMS.API.Application.IntegrationEvents;
using PVIMS.API.Application.IntegrationEvents.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.CausalityConfirmed
{
    public class SendIntegrationEventWhenCausalityConfirmedDomainEventHandler
                            : INotificationHandler<CausalityConfirmedDomainEvent>
    {
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<SendIntegrationEventWhenCausalityConfirmedDomainEventHandler> _logger;

        public SendIntegrationEventWhenCausalityConfirmedDomainEventHandler(IIntegrationEventService integrationEventService,
            ILogger<SendIntegrationEventWhenCausalityConfirmedDomainEventHandler> logger)
        {
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CausalityConfirmedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Causality confirmed for report instance: {domainEvent.ReportInstance.Id}. Send integration event.");

            var causalityConfirmedIntegrationEvent = new CausalityConfirmedIntegrationEvent(domainEvent.ReportInstance.Id, domainEvent.ReportInstance.Identifier);
            //await _integrationEventService.AddAndSaveEventAsync(causalityConfirmedIntegrationEvent);
        }
    }
}