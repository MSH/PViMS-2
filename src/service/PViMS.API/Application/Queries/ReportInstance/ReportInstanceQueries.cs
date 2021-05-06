using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.ReportInstance
{
    public class ReportInstanceQueries
        : IReportInstanceQueries
    {
        private string _connectionString = string.Empty;

        public ReportInstanceQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForPatientViewAsync(
            int patientId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<ReportInstanceEventDto>(
                    @$"SELECT	evt.Id,
		                        pce.Id AS PatientClinicalEventId,
		                        pce.SourceDescription AS AdverseEvent,
		                        convert(varchar(20), evt.EventDateTime , 120) AS ExecutedDate,
		                        u.FirstName + ' ' + u.LastName AS ExecutedBy,
		                        ai.QualifiedName AS Activity,
		                        stat.FriendlyDescription AS ExecutionEvent,
		                        evt.Comments
                        FROM PatientClinicalEvent pce
	                        INNER JOIN ReportInstance ri ON pce.PatientClinicalEventGuid = ri.ContextGuid
	                        INNER JOIN ActivityInstance ai ON ri.Id = ai.ReportInstance_Id
	                        INNER JOIN ActivityExecutionStatusEvent evt ON ai.Id = evt.ActivityInstance_Id
	                        INNER JOIN ActivityExecutionStatus stat ON evt.ExecutionStatus_Id = stat.Id
	                        INNER JOIN [User] u ON evt.EventCreatedBy_Id = u.Id
                        WHERE pce.Patient_Id = {patientId}
                        ORDER BY evt.EventDateTime desc");
            }
        }

        public async Task<IEnumerable<ReportInstanceEventDto>> GetExecutionStatusEventsForEventViewAsync(
            int patientClinicalEventId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<ReportInstanceEventDto>(
                    @$"SELECT	evt.Id,
		                        pce.Id AS PatientClinicalEventId,
		                        pce.SourceDescription AS AdverseEvent,
		                        convert(varchar(20), evt.EventDateTime , 120) AS ExecutedDate,
		                        u.FirstName + ' ' + u.LastName AS ExecutedBy,
		                        ai.QualifiedName AS Activity,
		                        stat.FriendlyDescription AS ExecutionEvent,
		                        evt.Comments
                        FROM PatientClinicalEvent pce
	                        INNER JOIN ReportInstance ri ON pce.PatientClinicalEventGuid = ri.ContextGuid
	                        INNER JOIN ActivityInstance ai ON ri.Id = ai.ReportInstance_Id
	                        INNER JOIN ActivityExecutionStatusEvent evt ON ai.Id = evt.ActivityInstance_Id
	                        INNER JOIN ActivityExecutionStatus stat ON evt.ExecutionStatus_Id = stat.Id
	                        INNER JOIN [User] u ON evt.EventCreatedBy_Id = u.Id
                        WHERE pce.Id = {patientClinicalEventId}
                        ORDER BY evt.EventDateTime desc");
            }
        }
    }
}
