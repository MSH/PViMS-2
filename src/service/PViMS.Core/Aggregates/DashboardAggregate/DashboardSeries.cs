using PVIMS.Core.Entities;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class DashboardSeries
        : AuditedEntityBase
    {
        public string Name { get; private set; }
        public string Attributes { get; private set; }

        public string ListSQL { get; private set; }
        public string ValueSQL { get; private set; }

        public int DashboardUnitId { get; private set; }

        protected DashboardSeries()
        {
        }

        public DashboardSeries(string attributes, string listSQL, string valueSQL, DashboardUnit dashboardUnit) : this()
        {
            Attributes = attributes;
            ListSQL = listSQL;
            ValueSQL = valueSQL;
            DashboardUnitId = dashboardUnit.Id;
        }
    }
}