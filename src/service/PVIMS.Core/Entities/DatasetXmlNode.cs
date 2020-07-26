using PVIMS.Core.ValueTypes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(DatasetXmlNode))]
    public class DatasetXmlNode : AuditedEntityBase
    {
        public DatasetXmlNode()
        {
            ChildrenNodes = new HashSet<DatasetXmlNode>();
            NodeAttributes = new HashSet<DatasetXmlAttribute>();
        }

        [Required]
        [StringLength(50)]
        public string NodeName { get; set; }
        [Required]
        public NodeType NodeType { get; set; }

        public virtual DatasetXmlNode ParentNode { get; set; }
        public string NodeValue { get; set; }
        public virtual DatasetElement DatasetElement { get; set; }
        public virtual DatasetElementSub DatasetElementSub { get; set; }

        public virtual ICollection<DatasetXmlNode> ChildrenNodes { get; set; }
        public virtual ICollection<DatasetXmlAttribute> NodeAttributes { get; set; }
   }
}