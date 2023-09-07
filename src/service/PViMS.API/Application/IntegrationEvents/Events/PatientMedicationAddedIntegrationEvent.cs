using PViMS.BuildingBlocks.EventBus.Events;
using System;
using System.Text.Json.Serialization;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record PatientMedicationAddedIntegrationEvent
        : IntegrationEvent
    {
        [JsonInclude]
        public int Id { get; private init; }

        [JsonInclude]
        public int Episode_Id { get; private init; }

        [JsonInclude]
        public Guid Patient_Guid { get; private init; }

        [JsonInclude]
        public Guid Patient_Medication_Guid { get; private init; }

        [JsonInclude]
        public string Start_Date { get; private init; }

        [JsonInclude]
        public string End_Date { get; private init; }

        [JsonInclude]
        public string Dose { get; private init; }

        [JsonInclude]
        public string Dose_Frequency { get; private init; }

        [JsonInclude]
        public string Dose_Unit { get; private init; }

        [JsonInclude]
        public string Route { get; private init; }

        [JsonInclude]
        public string Indication { get; private init; }

        [JsonInclude]
        public string Type_Of_Indication { get; private init; }

        [JsonInclude]
        public string Reason_For_Stopping { get; private init; }

        [JsonInclude]
        public string Clinician_Action { get; private init; }

        [JsonInclude]
        public string Batch_Number { get; private init; }

        [JsonInclude]
        public string Challenge_Effect { get; private init; }

        [JsonInclude]
        public string Comments { get; private init; }

        [JsonInclude]
        public string Source_Description { get; private init; }
    }
}