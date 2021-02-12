using PVIMS.Core.Entities.Accounts;
using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class ActivityExecutionStatusEvent : EntityBase
    {
        public ActivityExecutionStatusEvent()
        {
            Attachments = new HashSet<Attachment>();
        }

        public DateTime EventDateTime { get; set; }
        public string Comments { get; set; }
        public int ActivityInstanceId { get; set; }
        public int EventCreatedById { get; set; }
        public int ExecutionStatusId { get; set; }
        public DateTime? ContextDateTime { get; set; }
        public string ContextCode { get; set; }

        public virtual ActivityInstance ActivityInstance { get; set; }
        public virtual User EventCreatedBy { get; set; }
        public virtual ActivityExecutionStatus ExecutionStatus { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}