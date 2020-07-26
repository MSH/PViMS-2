using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(MetaTableType))]
    public class MetaTableType : EntityBase
    {
        [Required]
        public Guid metatabletype_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
