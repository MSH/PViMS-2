using System;
using System.Text.Json.Serialization;

namespace PViMS.BuildingBlocks.EventBus.Events
{
    public record IntegrationEvent
    {
        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            TransactionId = id;
            CreationDate = createDate;
        }

        [JsonInclude]
        public Guid TransactionId { get; private init; }

        [JsonInclude]
        public Guid? AcknowledgeId { get; private init; }

        [JsonInclude]
        public Guid SenderId { get; private init; }

        [JsonInclude]
        public DateTime CreationDate { get; private init; }
    }
}
