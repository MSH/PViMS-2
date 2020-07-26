using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(DatasetXmlAttribute))]
    public class DatasetXmlAttribute : AuditedEntityBase
    {
        [Required]
        [StringLength(50)]
        public string AttributeName { get; set; }

        [Required]
        public virtual DatasetXmlNode ParentNode { get; set; }

        public string AttributeValue { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
    }
}