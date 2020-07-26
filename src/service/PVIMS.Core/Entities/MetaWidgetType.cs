using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(MetaWidgetType))]
    public class MetaWidgetType : EntityBase
    {
        [Required]
        public Guid metawidgettype_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }
    }
}
