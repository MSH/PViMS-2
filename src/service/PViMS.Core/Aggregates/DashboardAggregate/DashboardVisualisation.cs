using PVIMS.Core.Entities;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class DashboardVisualisation 
        : AuditedEntityBase
	{
        public int ChartTypeId { get; private set; }

        public string Attributes { get; private set; }

        protected DashboardVisualisation()
        {
        }

        public DashboardVisualisation(int chartTypeId, string attributes): this()
        {
            ChartTypeId = chartTypeId;
            Attributes = attributes;
        }
    }
}