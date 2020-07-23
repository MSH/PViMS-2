using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVIMS.Core.Entities
{
    [Table(nameof(DatasetXml))]
    public class DatasetXml : AuditedEntityBase
    {
        public DatasetXml()
        {
            ChildrenNodes = new HashSet<DatasetXmlNode>();
        }

        [StringLength(50)]
        public string Description { get; set; }

        public virtual ICollection<DatasetXmlNode> ChildrenNodes { get; set; }

    }
}