using PViMS.BuildingBlocks.EventBus.Events;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record CausalityConfirmedIntegrationEvent : IntegrationEvent
    {
        public int ReportInstanceId { get; }
        public string Identifier { get; }

        public CausalityConfirmedIntegrationEvent(int reportInstanceId, string identifier)
        {
            ReportInstanceId = reportInstanceId;
            Identifier = identifier;
        }
    }
}