using PVIMS.Core.Entities.Accounts;
using System;

namespace PVIMS.Core.Entities
{
    public class PatientStatusHistory : AuditedEntityBase
	{
        public DateTime EffectiveDate { get; set; }
        public string Comments { get; set; }
        public int PatientId { get; set; }
        public int PatientStatusId { get; set; }
        public bool Archived { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ArchivedReason { get; set; }
        public int? AuditUserId { get; set; }

		public virtual Patient Patient { get; set; }
		public virtual PatientStatus PatientStatus { get; set; }
        public virtual User AuditUser { get; set; }
	}
}