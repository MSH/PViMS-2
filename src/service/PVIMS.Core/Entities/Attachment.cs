using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities.Accounts;
using System;

namespace PVIMS.Core.Entities
{
	public class Attachment : AuditedEntityBase
	{
        public byte[] Content { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public int AttachmentTypeId { get; set; }
        public int? EncounterId { get; set; }
        public int? PatientId { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }
        public int? ActivityExecutionStatusEventId { get; set; }

        public virtual AttachmentType AttachmentType { get; set; }
		public virtual Encounter Encounter { get; set; }
		public virtual Patient Patient { get; set; }
        public virtual User AuditUser { get; set; }
        public virtual ActivityExecutionStatusEvent ActivityExecutionStatusEvent { get; set; }
    }
}