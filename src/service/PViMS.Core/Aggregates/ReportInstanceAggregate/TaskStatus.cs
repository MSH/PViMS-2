using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class TaskStatus
        : Enumeration
    {
        public static TaskStatus New = new TaskStatus(1, nameof(New).ToLowerInvariant());
        public static TaskStatus UnderInvestigation = new TaskStatus(2, nameof(UnderInvestigation).ToLowerInvariant());
        public static TaskStatus OnHold = new TaskStatus(3, nameof(OnHold).ToLowerInvariant());
        public static TaskStatus Completed = new TaskStatus(4, nameof(Completed).ToLowerInvariant());
        public static TaskStatus Cancelled = new TaskStatus(5, nameof(Cancelled).ToLowerInvariant());

        public TaskStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<TaskStatus> List() =>
            new[] { New, UnderInvestigation, OnHold, Completed, Cancelled };

        public static TaskStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for TaskStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static TaskStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for TaskStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}