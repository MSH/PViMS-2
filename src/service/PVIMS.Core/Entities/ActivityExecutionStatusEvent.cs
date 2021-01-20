using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(ActivityExecutionStatusEvent))]
    public class ActivityExecutionStatusEvent : EntityBase
    {
        public ActivityExecutionStatusEvent()
        {
            Attachments = new HashSet<Attachment>();
        }

        public DateTime EventDateTime { get; set; }
        public string Comments { get; set; }
        public int ActivityInstanceId { get; set; }
        public int? EventCreatedById { get; set; }
        public int ExecutionStatusId { get; set; }
        public DateTime? ContextDateTime { get; set; }
        public string ContextCode { get; set; }


        [Required]
        public virtual ActivityInstance ActivityInstance { get; set; }

        [Required]
        public virtual ActivityExecutionStatus ExecutionStatus { get; set; }

        public DateTime EventDateTime { get; set; }
        public User EventCreatedBy { get; set; }

        public DateTime? ContextDateTime { get; set; }
        [StringLength(20)]
        public string ContextCode { get; set; }

        public string Comments { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}