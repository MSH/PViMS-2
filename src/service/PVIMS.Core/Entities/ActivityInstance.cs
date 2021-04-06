using PVIMS.Core.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Entities
{
    public class ActivityInstance : AuditedEntityBase
	{
        public ActivityInstance()
        {
            Current = true;
            ExecutionEvents = new HashSet<ActivityExecutionStatusEvent>();
        }

        public ActivityInstance(Activity activity, User currentUser)
        {
            Current = true;
            ExecutionEvents = new HashSet<ActivityExecutionStatusEvent>();
            ExecutionEvents.Add(InitialiseWithFirstExecutionStatus(activity, currentUser));
        }

        public string QualifiedName { get; set; }
        public int CurrentStatusId { get; set; }
        public int ReportInstanceId { get; set; }
        public bool Current { get; set; }

        public virtual ActivityExecutionStatus CurrentStatus { get; set; }
        public virtual ReportInstance ReportInstance { get; set; }

        public virtual ICollection<ActivityExecutionStatusEvent> ExecutionEvents { get; set; }

        private ActivityExecutionStatusEvent InitialiseWithFirstExecutionStatus(Activity activity, User currentUser)
        {
            var status = activity.ExecutionStatuses.OrderBy(es => es.Id).First();
            var statusEvent = new ActivityExecutionStatusEvent()
            {
                ActivityInstance = this,
                Comments = "",
                EventCreatedBy = currentUser,
                EventDateTime = DateTime.Now,
                ExecutionStatus = status
            };
            return statusEvent;
        }

        public ActivityExecutionStatusEvent AddNewEvent(ActivityExecutionStatus newStatus, User currentUser, string comments, DateTime? contextDate, string contextCode)
        {
            if(CurrentStatus.Description == newStatus.Description) { return null; };

            var statusEvent = new ActivityExecutionStatusEvent()
            {
                ActivityInstance = this,
                Comments = comments,
                EventCreatedBy = currentUser,
                EventDateTime = DateTime.Now,
                ExecutionStatus = newStatus,
                ContextCode = contextCode,
                ContextDateTime = contextDate
            };

            CurrentStatus = newStatus;
            ExecutionEvents.Add(statusEvent);

            return statusEvent;
        }
    }
}