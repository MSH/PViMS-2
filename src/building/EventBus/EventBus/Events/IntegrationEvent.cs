using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace PViMS.BuildingBlocks.EventBus.Events
{
    public record IntegrationEvent
    {
        public IntegrationEvent()
        {
            Transaction_Id = Guid.NewGuid();
            Creation_Date = DateTime.UtcNow;
            Processed = false;
        }

        public IntegrationEvent(Guid acknowledgeId, bool processed, string errorMessage)
        {
            Acknowledge_Id = acknowledgeId;
            Transaction_Id = Guid.NewGuid();
            Creation_Date = DateTime.UtcNow;
            Processed = processed;
            Error_Message = errorMessage;
            Sender_Id = Guid.Parse("E9B71820-76EC-4009-8078-47FCEC14F95C");
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Transaction_Id = id;
            Creation_Date = createDate;
        }

        [JsonInclude]
        public Guid Transaction_Id { get; private init; }

        [JsonInclude]
        public Guid? Acknowledge_Id { get; private init; }

        [JsonInclude]
        public Guid Sender_Id { get; private init; }

        [JsonInclude]
        public DateTime Creation_Date { get; private init; }

        [JsonInclude]
        public bool Processed { get; private init; }

        [JsonInclude]
        public string Error_Message { get; private init; }
    }
}
