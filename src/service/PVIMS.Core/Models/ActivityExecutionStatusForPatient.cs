using System.Collections.Generic;

using PVIMS.Core.Entities;

namespace PVIMS.Core.Models
{
    public class ActivityExecutionStatusForPatient
    {
        public ActivityExecutionStatusForPatient()
        {
            ActivityItems = new List<ActivityExecutionStatusInfo>();
        }

        public PatientClinicalEvent PatientClinicalEvent { get; set; }
        public ICollection<ActivityExecutionStatusInfo> ActivityItems { get; private set; }

        public class ActivityExecutionStatusInfo
        {
            public string Activity { get; set; }
            public string ExecutionStatus { get; set; }
            public string ExecutedBy { get; set; }
            public string ExecutedDate { get; set; }
            public string Comments { get; set; }
        }
    }
}