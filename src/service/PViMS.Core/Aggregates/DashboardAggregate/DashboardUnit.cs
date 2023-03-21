using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class DashboardUnit
        : Enumeration
    {
        public static DashboardUnit Patient = new DashboardUnit(1, "Patient");
        public static DashboardUnit PatientClinicalEvent = new DashboardUnit(2, "PatientClinicalEvent");
        public static DashboardUnit PatientMedication = new DashboardUnit(3, "PatientMedication");
        public static DashboardUnit PatientCondition = new DashboardUnit(4, "PatientCondition");
        public static DashboardUnit Encounter = new DashboardUnit(5, "Encounter");
        public static DashboardUnit DatasetInstance = new DashboardUnit(6, "DatasetInstance");

        public DashboardUnit(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<DashboardUnit> List() =>
            new[] { Patient, PatientClinicalEvent, PatientMedication, PatientCondition, Encounter, DatasetInstance };

        public static DashboardUnit FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for DashboardUnit: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static DashboardUnit From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for DashboardUnit: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}