using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(MetaDependency))]
    public class MetaDependency : EntityBase
    {
        [Required]
        public Guid metadependency_guid { get; set; }

        [Required]
        public virtual MetaTable ParentTable { get; set; }

        [Required]
        [StringLength(50)]
        public string ParentColumnName { get; set; }

        [Required]
        public virtual MetaTable ReferenceTable { get; set; }

        [Required]
        [StringLength(50)]
        public string ReferenceColumnName { get; set; }
    }
}
