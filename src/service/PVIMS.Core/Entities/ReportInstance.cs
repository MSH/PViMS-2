using PVIMS.Core.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public class ReportInstance : AuditedEntityBase
	{
        public ReportInstance()
        {
            ReportInstanceGuid = Guid.NewGuid();
            Identifier = "TBD";

            Activities = new HashSet<ActivityInstance>();
            ReportInstanceMedications = new HashSet<ReportInstanceMedication>();
        }

        public ReportInstance(WorkFlow workFlow, User currentUser)
		{
            ReportInstanceGuid = Guid.NewGuid();
            Identifier = "TBD";

            WorkFlow = workFlow;

            Activities = new HashSet<ActivityInstance>();
            ReportInstanceMedications = new HashSet<ReportInstanceMedication>();
            Activities.Add(InitialiseWithFirstActivity(workFlow, currentUser));
        }

        public Guid ReportInstanceGuid { get; set; }
        public DateTime? Finished { get; set; }
        public int WorkFlowId { get; set; }
        public Guid ContextGuid { get; set; }
        public string Identifier { get; set; }
        public string PatientIdentifier { get; set; }
        public int? TerminologyMedDraId { get; set; }
        public string SourceIdentifier { get; set; }

        public virtual WorkFlow WorkFlow { get; set; }
        public virtual TerminologyMedDra TerminologyMedDra { get; set; }

        public virtual ICollection<ActivityInstance> Activities { get; set; }
        public virtual ICollection<ReportInstanceMedication> ReportInstanceMedications { get; set; }

        public void SetIdentifier()
        {
            if(!string.IsNullOrWhiteSpace(Identifier))
            {
                Identifier = string.Format("{0}/{1}/{2}", WorkFlow.Id, Created.Year.ToString("D4"), Id.ToString("D5"));
            }
        }

        private ActivityInstance InitialiseWithFirstActivity(WorkFlow workFlow, User currentUser)
        {
            var activity = workFlow.Activities.OrderBy(a => a.Id).First();
            var status = activity.ExecutionStatuses.Single(es => es.Description == "UNCONFIRMED");
            var activityInstance = new ActivityInstance(activity, currentUser)
            {
                QualifiedName = activity.QualifiedName,
                ReportInstance = this,
                CurrentStatus = status,
                Current = true
            };
            return activityInstance;
        }

        public void SetNewActivity(Activity activity, User currentUser)
        {

            var status = activity.ExecutionStatuses.OrderBy(es => es.Id).First();
            var activityInstance = new ActivityInstance(activity, currentUser)
            {
                QualifiedName = activity.QualifiedName,
                ReportInstance = this,
                CurrentStatus = status,
                Current = true
            };

            Activities.Add(activityInstance);
        }

        public ActivityInstance CurrentActivity
        {
            get
            {
                return Activities.Single(a => a.Current == true);
            }
        }

        public string DisplayStatus
        {
            get
            {
                return String.Format("{0} | {1}", CurrentActivity.QualifiedName, CurrentActivity.CurrentStatus.Description);
            }
        }
    }
}