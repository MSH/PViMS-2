using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    public interface IAppointmentQueries
    {
        Task<IEnumerable<AppointmentSearchDto>> GetAppointmentsUsingPatientAttributeAsync(int criteriaId, DateTime searchFrom, DateTime searchTo, int facilityId = 0, int patientId = 0, string firstName = "", string lastName = "", string customAttributeKey = "", string customPath = "", string customValue = "");

        Task<IEnumerable<OutstandingVisitReportDto>> GetOutstandingVisitsAsync(DateTime searchFrom, DateTime searchTo, int facilityId);
    }
}
