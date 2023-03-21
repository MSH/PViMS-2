using PVIMS.Core.Entities;
using System.Collections.Generic;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class DashboardElement 
        : AuditedEntityBase
	{
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public string LongName { get; private set; }

        public bool Active { get; private set; }
        public int Order { get; private set; }

        public string SourceUrl { get; private set; }

        public virtual ICollection<DashboardSeries> SeriesElements { get; set; }
        public virtual ICollection<DashboardVisualisation> VisualisationElements { get; set; }

        protected DashboardElement()
        {
            SeriesElements = new HashSet<DashboardSeries>();
            VisualisationElements = new HashSet<DashboardVisualisation>();
        }

        public DashboardElement(string name, string shortName, string longName, int order, string sourceUrl): this()
        {
            Name = name;
            ShortName = shortName;
            LongName = longName;
            Order = order;
            SourceUrl = sourceUrl;

            Active = true;
        }
    }
}