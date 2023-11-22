using MediatR;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PViMS.Core.Events
{
    /// <summary>
    /// Event used when a new report instance task is added
    /// </summary>
    public class ReportInstanceAddedDomainEvent : INotification
    {
        public ReportInstance ReportInstance { get; }

        public ReportInstanceAddedDomainEvent(ReportInstance reportInstance)
        {
            ReportInstance = reportInstance;
        }
    }
}
