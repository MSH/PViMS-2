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

        public async Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByAnnualAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @$"SELECT c.PeriodYear AS PeriodDisplay, 
                                    c.MedDraTerm AS SystemOrganClass,
                                    c.FacilityName, 
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 1' THEN c.PatientCount ELSE 0 END) AS Grade1Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 2' THEN c.PatientCount ELSE 0 END) AS Grade2Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 3' THEN c.PatientCount ELSE 0 END) AS Grade3Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 4' THEN c.PatientCount ELSE 0 END) AS Grade4Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 5' THEN c.PatientCount ELSE 0 END) AS Grade5Count,
		                            SUM(CASE WHEN c.SeverityGrade = '' THEN c.PatientCount ELSE 0 END) AS GradeUnknownCount
				            FROM (SELECT b.PeriodYear, b.FacilityName, tm1.MedDraTerm, b.SeverityGrade, b.PatientCount
					            FROM TerminologyMedDra tm1 
						            LEFT JOIN 
					            (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], 
						            f.FacilityName,
						            t5.MedDraTerm AS 'Description',
						            ISNULL(mpce.SeverityGrade, '') AS SeverityGrade,
						            COUNT(*) AS PatientCount
					            FROM PatientClinicalEvent pce 
						            INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
						            INNER JOIN Patient p ON pce.Patient_Id = p.Id
						            INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id
						            INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
						            INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
						            INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
						            INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
						            INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
						            INNER JOIN Facility f on pf.Facility_Id = f.Id
					            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' and pce.Archived = 0 and p.Archived = 0 
					            GROUP BY DATEPART(YEAR, pce.OnsetDate), f.FacilityName, t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
					            WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c 
	                        GROUP BY c.PeriodYear, c.FacilityName, c.MedDraTerm
	                        ORDER BY c.MedDraTerm, c.PeriodYear, c.FacilityName";

                return await connection.QueryAsync<AdverseEventFrequencyReportDto>(sql);
            }
        }

        public async Task<IEnumerable<AdverseEventFrequencyReportDto>> GetAdverseEventsByQuarterAsync(DateTime searchFrom, DateTime searchTo)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @$"SELECT c.PeriodQuarter + ' ' + c.PeriodYear AS PeriodDisplay,
                                    c.MedDraTerm AS SystemOrganClass,
                                    c.FacilityName,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 1' THEN c.PatientCount ELSE 0 END) AS Grade1Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 2' THEN c.PatientCount ELSE 0 END) AS Grade2Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 3' THEN c.PatientCount ELSE 0 END) AS Grade3Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 4' THEN c.PatientCount ELSE 0 END) AS Grade4Count,
		                            SUM(CASE WHEN c.SeverityGrade = 'Grade 5' THEN c.PatientCount ELSE 0 END) AS Grade5Count,
		                            SUM(CASE WHEN c.SeverityGrade = '' THEN c.PatientCount ELSE 0 END) AS GradeUnknownCount
	                        FROM (SELECT b.PeriodYear, b.PeriodQuarter, b.FacilityName, tm1.MedDraTerm, b.SeverityGrade, b.PatientCount
	                            FROM TerminologyMedDra tm1 
		                            LEFT JOIN 
	                            (SELECT DATEPART(YEAR, pce.OnsetDate) as [PeriodYear], DATEPART(QUARTER, pce.OnsetDate) as [PeriodQuarter],
		                            f.FacilityName,
		                            t5.MedDraTerm AS 'Description', 
                                    ISNULL(mpce.SeverityGrade, '') AS SeverityGrade,
			                        COUNT(*) AS PatientCount
	                            FROM PatientClinicalEvent pce 
                                    INNER JOIN MetaPatientClinicalEvent mpce on pce.PatientClinicalEventGuid = mpce.PatientClinicalEventGuid 
		                            INNER JOIN Patient p ON pce.Patient_Id = p.Id
		                            INNER JOIN TerminologyMedDra t ON pce.SourceTerminologyMedDra_Id = t.Id
		                            INNER JOIN TerminologyMedDra t2 ON t.Parent_Id = t2.Id
		                            INNER JOIN TerminologyMedDra t3 ON t2.Parent_Id = t3.Id
		                            INNER JOIN TerminologyMedDra t4 ON t3.Parent_Id = t4.Id
		                            INNER JOIN TerminologyMedDra t5 ON t4.Parent_Id = t5.Id
		                            INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
		                            INNER JOIN Facility f on pf.Facility_Id = f.Id
	                            WHERE pce.OnsetDate BETWEEN '{searchFrom.ToString("yyyy-MM-dd")}' AND '{searchTo.ToString("yyyy-MM-dd")}' and pce.Archived = 0 and p.Archived = 0 
	                            GROUP BY DATEPART(YEAR, pce.OnsetDate), DATEPART(QUARTER, pce.OnsetDate), f.FacilityName, t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
	                            WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c
	                    GROUP BY c.PeriodYear, c.PeriodQuarter, c.FacilityName, c.MedDraTerm
	                    ORDER BY c.MedDraTerm, c.PeriodYear, c.PeriodQuarter, c.FacilityName";

                return await connection.QueryAsync<AdverseEventFrequencyReportDto>(sql);
            }
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