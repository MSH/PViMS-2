using PVIMS.Core.Entities;
using PVIMS.Core.SeedWork;
using System.Collections.Generic;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class Dashboard 
        : AuditedEntityBase, IAggregateRoot
    {
        public string UID { get; private set; }
        
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public string LongName { get; private set; }

        public int FrequencyId { get; private set; }
        public bool Active { get; private set; }

        public string Icon { get; private set; }

        public virtual ICollection<DashboardElement> Elements { get; set; }

        protected Dashboard()
        {
            Elements = new HashSet<DashboardElement>();
        }

        public Dashboard(string uid, string name, string shortName, string longName, Frequency frequency, string icon): this()
        {
            UID = uid;
            Name = name;
            ShortName = shortName;
            LongName = longName;
            FrequencyId = frequency.Id;
            Icon = icon;

            Active = true;
        }
    }
}