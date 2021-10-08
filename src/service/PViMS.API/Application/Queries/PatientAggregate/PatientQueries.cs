using Dapper;
using Microsoft.Data.SqlClient;
using PVIMS.API.Models;
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

        public async Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByEncounterAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PatientsOnTreatmentDto>(
                    @$"SELECT f.FacilityName, f.Id as FacilityId
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ie.Archived = 0 and ip.Archived = 0 and ie.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and (impce.[Istheadverseeventserious?] <> 'Yes' or impce.[Istheadverseeventserious?] is null)
                                ) AS PatientWithNonSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and impce.[Istheadverseeventserious?] = 'Yes'
                                ) AS PatientWithSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                    where ie.Archived = 0 and ip.Archived = 0 and ipce.Archived = 0
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientWithEventCount        
                        FROM Facility f
                        ORDER BY f.FacilityName");
            }
        }

        public async Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByEncounterAsync(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PatientListDto>(
                    @$"SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS FullName, s.FacilityName  FROM 
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Encounter ie 
				                        inner join Patient ip on ie.Patient_Id = ip.Id
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
			                        where ie.Archived = 0 and ip.Archived = 0 
                                        and ie.EncounterDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
				                        and ifa.Id = {facilityId}
				                    group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName 
		                        ) AS s");
            }
        }

        public async Task<IEnumerable<PatientsOnTreatmentDto>> GetPatientsOnTreatmentByFacilityAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PatientsOnTreatmentDto>(
                    @$"SELECT f.FacilityName, f.Id as FacilityId
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip 
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 and ipf.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ip.Archived = 0 and ipf.Archived = 0 and ipce.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and (impce.[Istheadverseeventserious?] <> 'Yes' or impce.[Istheadverseeventserious?] is null)
                                ) AS PatientWithNonSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                        inner join MetaPatientClinicalEvent impce on ipce.PatientClinicalEventGuid = impce.PatientClinicalEventGuid 
                                    where ip.Archived = 0 and ipf.Archived = 0 and ipce.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                        and impce.[Istheadverseeventserious?] = 'Yes'
                                ) AS PatientWithSeriousEventCount
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                        inner join PatientClinicalEvent ipce on ip.Id = ipce.Patient_Id 
                                    where ip.Archived = 0 and ipf.Archived = 0 and ipce.Archived = 0
                                        and ipf.EnrolledDate between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
                                        and ifa.Id = f.Id
                                ) AS PatientWithEventCount
                        FROM Facility f
                        ORDER BY f.FacilityName ");
            }
        }

        public async Task<IEnumerable<PatientListDto>> GetPatientOnTreatmentListByFacilityAsync(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                return await connection.QueryAsync<PatientListDto>(
                    @$"SELECT s.Id AS PatientId, s.FirstName + ' ' + s.Surname AS FullName, s.FacilityName  FROM
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Patient ip 
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 
			                            and ip.Created between '{searchFrom.ToString("yyyy-MM-dd")}' and '{searchTo.ToString("yyyy-MM-dd")}'
				                        and ifa.Id = {facilityId}
		                        ) AS s");
            }
        }
    }
}