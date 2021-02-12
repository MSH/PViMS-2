using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class OrgUnitType : EntityBase
    {
        public OrgUnitType()
        {
            Children = new HashSet<OrgUnitType>();
            OrgUnits = new HashSet<OrgUnit>();
        }

        public string Description { get; set; }
        public int? ParentId { get; set; }

        public virtual OrgUnitType Parent { get; set; }

        public virtual ICollection<OrgUnitType> Children { get; set; }
        public virtual ICollection<OrgUnit> OrgUnits { get; set; }
    }
}