using PVIMS.Core.ValueTypes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(Config))]
    public class Config : AuditedEntityBase
    {
        [Required]
        public virtual ConfigType ConfigType { get; set; }

        [Required]
        [StringLength(100)]
        public string ConfigValue { get; set; }
    }
}