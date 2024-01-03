using MediatR;
using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    [DataContract]
    public class AddConditionToPatientCommand
        : IRequest<PatientConditionIdentifierDto>
    {
        [DataMember]
        public int PatientId { get; private set; }

        [DataMember]
        public string SourceDescription { get; private set; }

        [DataMember]
        public int SourceTerminologyMedDraId { get; private set; }

        [DataMember]
        public DateTime StartDate { get; private set; }

        [DataMember]
        public DateTime? OutcomeDate { get; private set; }

        [DataMember]
        public string Outcome { get; private set; }

        [DataMember]
        public string TreatmentOutcome { get; private set; }

        [DataMember]
        public string CaseNumber { get; private set; }

        [DataMember]
        public string Comments { get; private set; }

        [DataMember]
        public IDictionary<int, string> Attributes { get; private set; }

        public AddConditionToPatientCommand()
        {
        }

        public AddConditionToPatientCommand(int patientId, string sourceDescription, int sourceTerminologyMedDraId, DateTime startDate, DateTime? outcomeDate, string outcome, string treatmentOutcome, string caseNumber, string comments, IDictionary<int, string> attributes) : this()
        {
            PatientId = patientId;
            SourceDescription = sourceDescription;
            SourceTerminologyMedDraId = sourceTerminologyMedDraId;
            StartDate = startDate;
            OutcomeDate = outcomeDate;
            Outcome = outcome;
            TreatmentOutcome = treatmentOutcome;
            CaseNumber = caseNumber;
            Comments = comments;
            Attributes = attributes;
        }
    }
}