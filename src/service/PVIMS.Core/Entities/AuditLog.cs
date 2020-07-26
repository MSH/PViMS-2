using PVIMS.Core.ValueTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(AuditLog))]
    public partial class AuditLog : EntityBase
    {
        [Required]
        public virtual AuditType AuditType { get; set; }
        [Required]
        public DateTime ActionDate { get; set; }

        public virtual User User { get; set; }
        public string Details { get; set; }
        public string Log { get; set; }
    }
}
