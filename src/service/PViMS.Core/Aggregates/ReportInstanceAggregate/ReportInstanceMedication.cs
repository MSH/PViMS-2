using PVIMS.Core.Entities;
using System;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportInstanceMedication 
        : EntityBase
    {
        public string MedicationIdentifier { get; private set; }
        public string NaranjoCausality { get; private set; }
        //public int? NaranjoScore { get; private set; }
        public string WhoCausality { get; private set; }
        public Guid ReportInstanceMedicationGuid { get; set; }
        public int ReportInstanceId { get; private set; }

        public virtual ReportInstance ReportInstance { get; private set; }

        public ReportInstanceMedication()
        {
        }

        public ReportInstanceMedication(string medicationIdentifier, string naranjoCausality, string whoCausality, Guid reportInstanceMedicationGuid)
        {
            MedicationIdentifier = medicationIdentifier;
            NaranjoCausality = naranjoCausality;
            WhoCausality = whoCausality;
            ReportInstanceMedicationGuid = reportInstanceMedicationGuid;
        }

        public void ChangeMedicationIdentifier(string medicationIdentifier)
        {
            MedicationIdentifier = medicationIdentifier;
        }

        public void ChangeWhoCausality(string causality)
        {
            WhoCausality = causality;
        }

        public void ChangeNaranjoCausality(string causality, int? score)
        {
            NaranjoCausality = score.HasValue ? $"{causality} ({score})" : causality;
            //NaranjoScore = score;
        }
    }
}