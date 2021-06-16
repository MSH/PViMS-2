using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Exceptions;
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

        public ActivityInstance(Activity activity, User currentUser)
        {
            if(activity.ExecutionStatuses.Count == 0)
            {
                throw new DomainException($"Cannot initialise activity instance as activity is not configure with any execution statuses {activity.QualifiedName}");
            }

            _executionEvents = new List<ActivityExecutionStatusEvent>();

            QualifiedName = activity.QualifiedName;
            Current = true;

            InitialiseWithFirstExecutionStatus(activity, currentUser);
        }

        private void InitialiseWithFirstExecutionStatus(Activity activity, User currentUser)
        {
            var executionStatus = activity.ExecutionStatuses.OrderBy(es => es.Id).First();
            var executionStatusEvent = new ActivityExecutionStatusEvent(executionStatus, currentUser, "", "", null);
            _executionEvents.Add(executionStatusEvent);
        }

        public void SetToOld()
        {
            Current = false;
        }

        public ActivityExecutionStatusEvent ExecuteEvent(ActivityExecutionStatus newExecutionStatus, User currentUser, string comments, DateTime? contextDate, string contextCode)
        {
            var newExecutionStatusEvent = new ActivityExecutionStatusEvent(newExecutionStatus, currentUser, comments, contextCode, contextDate);

            CurrentStatus = newExecutionStatus;
            _executionEvents.Add(newExecutionStatusEvent);

            return newExecutionStatusEvent;
        }

        public ActivityExecutionStatusEvent GetLatestEvent()
        {
            if (ExecutionEvents.Count > 0)
            {
                ExecutionEvents.OrderByDescending(ee => ee.EventDateTime)
                                .First(ee => ee.ExecutionStatus.Id == CurrentStatus.Id);
            }
            return null;
        }

        public ActivityExecutionStatusEvent GetLatestE2BGeneratedEvent()
        {
            if (ExecutionEvents.Count > 0)
            {
                ExecutionEvents.OrderByDescending(ee => ee.EventDateTime)
                                .First(ee => ee.ExecutionStatus.Description == "E2BGENERATED");
            }
            return null;
        }

    }
}