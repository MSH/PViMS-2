using Dapper;
using Microsoft.Data.SqlClient;
using PVIMS.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.AppointmentAggregate
{
    public class AppointmentQueries
        : IAppointmentQueries
    {
        private string _connectionString = string.Empty;

        public AppointmentQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<OutstandingVisitReportDto>> GetOutstandingVisitsAsync(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";

                return await connection.QueryAsync<OutstandingVisitReportDto>(
                    @$"SELECT   p.Id AS PatientId, 
                                p.FirstName, 
                                p.Surname as LastName, 
                                a.AppointmentDate,
                                f.FacilityName as Facility
                       FROM Patient p 
	                        INNER JOIN Appointment a ON p.Id = a.Patient_Id 
                            INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                            INNER JOIN Facility f ON pf.Facility_Id = f.Id
                       WHERE a.AppointmentDate < DATEADD(dd, 3, GETDATE())
	                        AND a.AppointmentDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' {where} 
	                        AND a.Cancelled = 0
                            AND p.Archived = 0 and a.Archived = 0
	                        AND NOT EXISTS(SELECT Id FROM Encounter ie WHERE ie.Patient_Id = p.Id AND ie.Archived = 0 AND ie.EncounterDate BETWEEN DATEADD(dd, -3, a.AppointmentDate) AND DATEADD(dd, 3, a.AppointmentDate))
                       ORDER BY a.AppointmentDate desc");
            }
        }
    }
}