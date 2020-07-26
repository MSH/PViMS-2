using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(MetaForm))]
    public class MetaForm : EntityBase
    {
        public MetaForm()
        {
            metaform_guid = Guid.NewGuid();
            IsSystem = false;
        }

        [Required]
        public Guid metaform_guid { get; set; }

        [Required]
        [StringLength(50)]
        public string FormName { get; set; }

        [Required]
        [StringLength(20)]
        public string ActionName { get; set; }

        public string MetaDefinition { get; set; }

        public bool IsSystem { get; set; }
    }
}
