using PViMS.BuildingBlocks.EventBus.Events;
using System;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record PatientAddedAckIntegrationEvent : IntegrationEvent
    {
        public bool Processed { get; private init; }

        public PatientAddedAckIntegrationEvent(Guid acknowledgeId): base(acknowledgeId)
        {
            Processed = true;
        }
    }
}