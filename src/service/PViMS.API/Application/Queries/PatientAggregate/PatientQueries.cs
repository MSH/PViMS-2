using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.PatientAggregate
{
    public class PatientQueries
        : IPatientQueries
    {
        private string _connectionString = string.Empty;

        public PatientQueries(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsWithMissingTLDAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PatientDto>(
                    @$"	SELECT	p.PatientGuid,
								p.Id
							FROM Patient p
							WHERE p.Archived = 0
								AND NOT EXISTS
									(
										SELECT Id
											FROM PatientMedication pm 
										WHERE pm.Patient_Id = p.Id
										AND pm.MedicationSource IN(
											'TDF/3TC/DTg',
											'Tenofavir +Lamivudina +dolutegravir cp',
											'tenofovir/lamivudina/dolutegravir',
											'Tenofovir-Lamivudina-Dolutegravir', 
											'Tenofovir-Lamivudina-Dolutregavir',
											'Tfd+3tc+dtg',
											'Tdf+3fc+dtg',
											'Tdf+3tc+dtg',
											'TDF-3TC-DTG ( 650mg) COmp.'
										)
									)
						ORDER BY p.Id");
            }
        }
    }
}
