using System.Collections.Generic;

namespace PVIMS.Core.Entities
{
    public class OrgUnit : EntityBase
    {
        public OrgUnit()
        {
            Children = new HashSet<OrgUnit>();
            Facilities = new HashSet<Facility>();
        }

        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int OrgUnitTypeId { get; set; }

        public virtual OrgUnit Parent { get; set; }
        public virtual OrgUnitType OrgUnitType { get; set; }

        public virtual ICollection<OrgUnit> Children { get; set; }
        public virtual ICollection<Facility> Facilities { get; set; }
    }
}