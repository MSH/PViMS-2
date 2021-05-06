using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class ActivityInstance 
        : AuditedEntityBase
	{
        public string QualifiedName { get; private set; }

        public int CurrentStatusId { get; private set; }
        public virtual ActivityExecutionStatus CurrentStatus { get; private set; }

        public bool Current { get; private set; }
        
        public int ReportInstanceId { get; private set; }
        public virtual ReportInstance ReportInstance { get; private set; }

        private readonly List<ActivityExecutionStatusEvent> _executionEvents;
        public IReadOnlyCollection<ActivityExecutionStatusEvent> ExecutionEvents => _executionEvents;

        protected ActivityInstance()
        {
            _executionEvents = new List<ActivityExecutionStatusEvent>();
        }

        public ActivityInstance(string qualifiedName, ActivityExecutionStatus currentStatus, Activity activity, User currentUser)
        {
            QualifiedName = qualifiedName;
            CurrentStatus = currentStatus;

            Current = true;
            InitialiseWithFirstExecutionStatus(activity, currentUser);
        }

        private void InitialiseWithFirstExecutionStatus(Activity activity, User currentUser)
        {
            var status = activity.ExecutionStatuses.OrderBy(es => es.Id).First();
            var statusEvent = new ActivityExecutionStatusEvent(status, currentUser, "", "", null);
            _executionEvents.Add(statusEvent);
        }

        public ActivityExecutionStatusEvent AddNewEvent(ActivityExecutionStatus newStatus, User currentUser, string comments, DateTime? contextDate, string contextCode)
        {
            if(CurrentStatus.Description == newStatus.Description) { return null; };

            var statusEvent = new ActivityExecutionStatusEvent(newStatus, currentUser, comments, contextCode, contextDate);

            CurrentStatus = newStatus;
            _executionEvents.Add(statusEvent);

            return statusEvent;
        }

        public void SetToOld()
        {
            Current = false;
        }
    }
}