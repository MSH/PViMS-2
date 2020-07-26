using System;
using System.Collections.Generic;

using PVIMS.Core.Models;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Core.Services
{
    public interface IReportService
    {
        ICollection<AdverseEventList> GetAdverseEventItems(DateTime searchFrom, DateTime searchTo, AdverseEventCriteria adverseEventCriteria, AdverseEventStratifyCriteria adverseEventStratifyCriteria);
        ICollection<AdverseEventQuarterlyList> GetAdverseEventQuarterlyItems(DateTime searchFrom, DateTime searchTo);
        ICollection<AdverseEventAnnualList> GetAdverseEventAnnualItems(DateTime searchFrom, DateTime searchTo);
        ICollection<CausalityNotSetList> GetCausalityNotSetItems(DateTime searchFrom, DateTime searchTo, CausalityConfigType causalityConfig, int facilityId, CausalityCriteria causailityCriteria);
        ICollection<OutstandingVisitList> GetOutstandingVisitItems(DateTime searchFrom, DateTime searchTo, int facilityId);
        ICollection<DrugList> GetPatientsByDrugItems(string searchTerm);
        ICollection<PatientList> GetPatientListByDrugItems(int conceptId);
        ICollection<PatientOnStudyList> GetPatientOnStudyItems(DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria);
        ICollection<PatientList> GetPatientListOnStudyItems(DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria, int facilityId);
    }
}
