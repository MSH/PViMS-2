using VPS.Common.Domain;

namespace PVIMS.Core.Models
{
    public class AdverseEventAnnualList : Entity<int>
    {
        public int? PeriodYear { get; set; }
        public string FacilityName { get; set; }
        public string MedDraTerm { get; set; }
        public int Grade1Count { get; set; }
        public int Grade2Count { get; set; }
        public int Grade3Count { get; set; }
        public int Grade4Count { get; set; }
        public int Grade5Count { get; set; }
        public int GradeUnknownCount { get; set; }
    }
}
