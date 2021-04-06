using System;
using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class MetaTableType : EntityBase
    {
        public MetaTableType()
        {
            MetaTables = new HashSet<MetaTable>();
        }

        public Guid MetaTableTypeGuid { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MetaTable> MetaTables { get; set; }
    }
}
