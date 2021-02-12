using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public partial class CohortGroup : EntityBase
    {
        public CohortGroup()
        {
            CohortGroupEnrolments = new HashSet<CohortGroupEnrolment>();
            EncounterTypeWorkPlans = new HashSet<EncounterTypeWorkPlan>();
        }

        public string CohortName { get; set; }
        public string CohortCode { get; set; }
        public int LastPatientNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int MinEnrolment { get; set; }
        public int MaxEnrolment { get; set; }
        public int? ConditionId { get; set; }

        public virtual Condition Condition { get; set; }
        public virtual ICollection<CohortGroupEnrolment> CohortGroupEnrolments { get; set; }
        public virtual ICollection<EncounterTypeWorkPlan> EncounterTypeWorkPlans { get; set; }

        public string DisplayName
        {
            get
            {
                return String.Format("{0} ({1})", CohortName, CohortCode);
            }
        }
    }
}
