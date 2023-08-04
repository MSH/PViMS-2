using PViMS.BuildingBlocks.EventBus.Events;
using System;
using System.Text.Json.Serialization;

namespace PVIMS.API.Application.IntegrationEvents.Events
{
    public record PatientClinicalEventAddedIntegrationEvent 
        : IntegrationEvent
    {
        [JsonInclude]
        public int Id { get; private init; }

        [JsonInclude]
        public int Episode_Id { get; private init; }

        [JsonInclude]
        public Guid Patient_Guid { get; private init; }

        [JsonInclude]
        public Guid Patient_Clinical_Event_Guid { get; private init; }

        [JsonInclude]
        public string Full_Name_Reporter { get; private init; }

        [JsonInclude]
        public string Reporter_Contact_Email { get; private init; }

        [JsonInclude]
        public string Reporter_Contact_Number { get; private init; }

        [JsonInclude]
        public string Reporter_Type { get; private init; }

        [JsonInclude]
        public string Report_Date { get; private init; }

        [JsonInclude]
        public string Onset_Date { get; private init; }

        [JsonInclude]
        public string Resolution_Date { get; private init; }

        [JsonInclude]
        public string Severity_Grade { get; private init; }

        [JsonInclude]
        public string Severity_Grading_Scale { get; private init; }

        [JsonInclude]
        public string Outcome { get; private init; }

        [JsonInclude]
        public string Intensity { get; private init; }

        [JsonInclude]
        public int? Is_Serious { get; private init; }

        [JsonInclude]
        public string Seriousness { get; private init; }

        [JsonInclude]
        public string Comments { get; private init; }

        [JsonInclude]
        public string Source_Description { get; private init; }

        [JsonInclude]
        public string Terminology_Meddra_Code { get; private init; }

        [JsonInclude]
        public string Treatment_Of_Reaction { get; private init; }
    }
}