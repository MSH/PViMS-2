using System;

namespace PVIMS.Core.Entities
{
    public class ReportInstanceMedication : EntityBase
    {
        public ReportInstanceMedication()
        {
        }

        public string MedicationIdentifier { get; set; }
        public string NaranjoCausality { get; set; }
        public string WhoCausality { get; set; }
        public int ReportInstanceId { get; set; }
        public Guid ReportInstanceMedicationGuid { get; set; }

        public virtual ReportInstance ReportInstance { get; set; }
    }
}