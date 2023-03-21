using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class Frequency
        : Enumeration
    {
        public static Frequency Daily = new Frequency(1, "Daily");
        public static Frequency Weekly = new Frequency(2, "Weekly");
        public static Frequency Monthly = new Frequency(3, "Monthly");
        public static Frequency Quarterly = new Frequency(4, "Quarterly");
        public static Frequency BiAnnual = new Frequency(5, "BiAnnual");
        public static Frequency Yearly = new Frequency(6, "Yearly");

        public Frequency(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<Frequency> List() =>
            new[] { Daily, Weekly, Monthly, Quarterly, BiAnnual, Yearly };

        public static Frequency FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for Frequency: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static Frequency From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for Frequency: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}