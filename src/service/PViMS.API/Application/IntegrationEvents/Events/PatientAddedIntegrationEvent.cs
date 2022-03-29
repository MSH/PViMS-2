using PViMS.BuildingBlocks.EventBus.Events;
using System;
using System.Text.Json.Serialization;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record PatientAddedIntegrationEvent : IntegrationEvent
    {
        [JsonInclude]
        public int EpisodeId { get; private init; }

        [JsonInclude]
        public string PatientCategory { get; private init; }

        [JsonInclude]
        public int PersonId { get; private init; }

        [JsonInclude]
        public bool Processed { get; private init; }

        [JsonInclude]
        public string Regimen { get; private init; }

        [JsonInclude]
        public int RegistrationId { get; private init; }

        [JsonInclude]
        public int RegistrationNo { get; private init; }

        [JsonInclude]
        public string RegistrationType { get; private init; }

        [JsonInclude]
        public int RegistrationYear { get; private init; }
        
        [JsonInclude]
        public DateTime? TreatmentStartDate { get; private init; }
    }
}