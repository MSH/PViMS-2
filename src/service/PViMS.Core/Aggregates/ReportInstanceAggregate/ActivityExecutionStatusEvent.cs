using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using System;
using System.Collections.Generic;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class ActivityExecutionStatusEvent 
        : EntityBase
    {
        public DateTime EventDateTime { get; private set; }
        public string Comments { get; private set; }
        public int EventCreatedById { get; private set; }
        public int ExecutionStatusId { get; private set; }
        public DateTime? ContextDateTime { get; private set; }
        public string ContextCode { get; private set; }
        public int ActivityInstanceId { get; private set; }

        private readonly List<Attachment> _attachments;
        public IReadOnlyCollection<Attachment> Attachments => _attachments;

        public virtual ActivityExecutionStatus ExecutionStatus { get; private set; }
        public virtual ActivityInstance ActivityInstance { get; private set; }
        public virtual User EventCreatedBy { get; private set; }

        protected ActivityExecutionStatusEvent()
        {
            _attachments = new List<Attachment>();
        }

        public ActivityExecutionStatusEvent(ActivityExecutionStatus newStatus, User currentUser, string comments, string contextCode, DateTime? contextDateTime)
        {
            EventCreatedById = currentUser.Id;
            EventCreatedBy = currentUser;

            ExecutionStatusId = newStatus.Id;
            ExecutionStatus = newStatus;
            Comments = comments;

            ContextCode = contextCode;
            ContextDateTime = contextDateTime;

            EventDateTime = DateTime.Now;
        }

        public void AddAttachment(string fileName, AttachmentType attachmentType, long size, Byte[] content, string description)
        {
            var newAttachment = new Attachment
            {
                Description = description,
                FileName = fileName,
                AttachmentType = attachmentType,
                Size = size,
                Content = content
            };

            _attachments.Add(newAttachment);
        }
    }
}