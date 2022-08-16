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

        public async Task<IEnumerable<AppointmentSearchDto>> GetAppointmentsUsingPatientAttributeAsync(int criteriaId, DateTime searchFrom, DateTime searchTo, int facilityId = 0, int patientId = 0, string firstName = "", string lastName = "", string customAttributeKey = "", string customPath = "", string customValue = "")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sql = $@"SELECT	
                                a.Id,
				                CONVERT(VARCHAR(10), a.AppointmentDate, 101) AS AppointmentDate,
				                p.Id as PatientId,
				                p.FirstName,
				                p.Surname AS LastName,
				                f.FacilityName,
				                a.Reason,
				                CASE 
                                    WHEN a.Cancelled = 1 THEN 'Cancelled' 
                                    WHEN e.Id IS NOT NULL THEN 'Appointment met' 
                                    WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) >= GETDATE() THEN 'Appointment' 
                                    WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 0 THEN 'MISSED' 
                                    WHEN e.Id IS NULL AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 1 THEN 'Did Not Arrive' 
                                END AS AppointmentStatus,
				                ISNULL(e.Id, 0) as EncounterId
			                FROM Appointment a
				                INNER JOIN Patient p ON a.Patient_Id = p.Id
				                INNER JOIN PatientFacility pf ON p.Id = pf.Patient_Id 
                                    AND pf.Id = (SELECT TOP 1 Id FROM PatientFacility ipf WHERE Patient_Id = p.Id ORDER BY EnrolledDate DESC)
				                INNER JOIN Facility f ON pf.Facility_Id = f.Id
				                LEFT JOIN Encounter e ON e.Patient_Id = p.Id 
                                    AND e.EncounterDate >= DATEADD(dd, 1, a.AppointmentDate) 
                                    AND e.EncounterDate <= DATEADD(dd, 5, a.AppointmentDate)
			                CROSS APPLY p.CustomAttributesXmlSerialised.nodes('CustomAttributeSet/CustomStringAttribute') as X(Y)
			                WHERE a.Archived = 0 AND p.Archived = 0 ";

                PrepareDateRangeQuery(searchFrom, searchTo, sql);
                PrepareCriteriaQuery(criteriaId, sql);
                PrepareFacilityQuery(facilityId, sql);
                PreparePatientQuery(patientId, firstName, lastName, sql);
                PrepareCustomAttributeQuery(customAttributeKey, customValue, sql);

                return await connection.QueryAsync<AppointmentSearchDto>(sql);
            }
        }

        private void PrepareCustomAttributeQuery(string customAttributeKey, string customValue, string sql)
        {
            if (!String.IsNullOrWhiteSpace(customValue))
            {
                sql += $@"AND X.Y.value('(Key)[1]', 'VARCHAR(MAX)') = '{customAttributeKey}' AND X.Y.value('(Value)[1]', 'VARCHAR(MAX)')  LIKE '%{customValue}%' ";
            }
        }

        private void PreparePatientQuery(int patientId, string firstName, string lastName, string sql)
        {
            if (patientId > 0)
            {
                sql += $@"AND p.Id = {patientId} ";
            }
            if (!String.IsNullOrWhiteSpace(firstName))
            {
                sql += $@"AND p.FirstName LIKE '%{firstName}%' ";
            }
            if (!String.IsNullOrWhiteSpace(lastName))
            {
                sql += $@"AND p.LastName LIKE '%{lastName}%' ";
            }
        }

        private void PrepareFacilityQuery(int facilityId, string sql)
        {
            if (facilityId > 0)
            {
                sql += $@"AND f.Id = {facilityId} ";
            }
        }

        private void PrepareDateRangeQuery(DateTime searchFrom, DateTime searchTo, string sql)
        {
            if (searchFrom.Date > DateTime.MinValue.Date)
            {
                sql += $@"AND a.AppointmentDate >='{searchFrom.ToString("yyyy-MM-dd")}' ";
            }
            if (searchTo.Date < DateTime.MaxValue.Date)
            {
                sql += $@"AND a.AppointmentDate <='{searchTo.ToString("yyyy-MM-dd")}' ";
            }
        }

        private void PrepareCriteriaQuery(int criteriaId, string sql)
        {
            if (criteriaId == 3)
            {
                sql += $@"AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 0 AND e.Id IS NULL ";
            }
            if (criteriaId == 4)
            {
                sql += $@"AND DATEADD(dd, 3, a.AppointmentDate) < GETDATE() AND a.DNA = 1 AND e.Id IS NULL ";
            }
            if (criteriaId == 5)
            {
                sql += $@"AND e.Id IS NOT NULL ";
            }
            if (criteriaId == 5)
            {
                sql += $@"AND a.Cancelled = 0 ";
            }
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