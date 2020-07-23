using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(ReportInstanceMedication))]
    public class ReportInstanceMedication : EntityBase
    {
        public ReportInstanceMedication()
        {
        }

        [Required]
        public virtual ReportInstance ReportInstance { get; set; }

        [Required]
        public Guid ReportInstanceMedicationGuid { get; set; }

        public string MedicationIdentifier { get; set; }

        [StringLength(30)]
        public string NaranjoCausality { get; set; }

        [StringLength(30)]
        public string WhoCausality { get; set; }
    }
}