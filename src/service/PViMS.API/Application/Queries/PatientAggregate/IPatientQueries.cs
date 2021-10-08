using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public interface IPatientQueries
    {
        Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByAnnualAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByQuarterAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByEncounterAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByEncounterAsync(DateTime searchFrom, DateTime searchTo, int facilityId);

        Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByFacilityAsync(DateTime searchFrom, DateTime searchTo);

        Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByFacilityAsync(DateTime searchFrom, DateTime searchTo, int facilityId);
    }
}
