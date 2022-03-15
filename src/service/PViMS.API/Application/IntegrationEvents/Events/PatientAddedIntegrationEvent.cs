using PViMS.BuildingBlocks.EventBus.Events;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record PatientAddedIntegrationEvent : IntegrationEvent
    {
        public string Identifier { get; }

        public PatientAddedIntegrationEvent(string identifier)
        {
            Identifier = identifier;
        }
    }
}