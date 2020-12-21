using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using PVIMS.Infrastructure;
using PVIMS.Core.Entities;

namespace PVIMS.Services
{
    public class ReportService : IReportService 
    {
        private readonly PVIMSDbContext _context;

        public ReportService(PVIMSDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ICollection<AdverseEventList> GetAdverseEventItems(DateTime searchFrom, DateTime searchTo, AdverseEventCriteria adverseEventCriteria, AdverseEventStratifyCriteria adverseEventStratifyCriteria)
        {
            string sql = "";

            if (adverseEventCriteria == AdverseEventCriteria.ReportSource)
            {
                switch (adverseEventStratifyCriteria)
                {
                    case AdverseEventStratifyCriteria.AgeGroup:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description', 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END AS 'Criteria',
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END
                                        , [Istheadverseeventserious?]
                            ORDER BY mpce.SourceTerminologyMedDra asc, Criteria asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Facility:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            mpf.Facility AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
	                            INNER JOIN MetaPatientFacility mpf ON mp.Id = mpf.Patient_Id AND mpf.EnrolledDate = (SELECT MAX(EnrolledDate) FROM MetaPatientFacility impf WHERE impf.Patient_Id = mp.Id)
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY mpce.SourceTerminologyMedDra, mpf.Facility, [Istheadverseeventserious?]
                            ORDER BY mpce.SourceTerminologyMedDra asc, mpf.Facility asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Drug:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            mpm.Medication AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
                                INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id
                                INNER JOIN MetaPatientMedication mpm ON rim.ReportInstanceMedicationGuid = mpm.PatientMedicationGuid
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, mpm.Medication, [Istheadverseeventserious?] 
                            ORDER BY mpce.SourceTerminologyMedDra asc, mpm.Medication asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Cohort:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            cg.CohortName AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN CohortGroupEnrolment cge ON mp.Id = cge.Patient_Id
                                INNER JOIN CohortGroup cg ON cge.CohortGroup_Id = cg.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, cg.CohortName, [Istheadverseeventserious?] 
                            ORDER BY mpce.SourceTerminologyMedDra asc, cg.CohortName asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    default:
                        break;
                }
            }
            else // MedDRA
            {
                switch (adverseEventStratifyCriteria)
                {
                    case AdverseEventStratifyCriteria.AgeGroup:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description', 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END AS 'Criteria',
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY t.MedDraTerm, 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END
                                        , [Istheadverseeventserious?]
                            ORDER BY t.MedDraTerm asc, Criteria asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Facility:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            mpf.Facility AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            INNER JOIN MetaPatientFacility mpf ON mp.Id = mpf.Patient_Id AND mpf.EnrolledDate = (SELECT MAX(EnrolledDate) FROM MetaPatientFacility impf WHERE impf.Patient_Id = mp.Id)
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY t.MedDraTerm, mpf.Facility, [Istheadverseeventserious?]
                            ORDER BY t.MedDraTerm asc, mpf.Facility asc, [Istheadverseeventserious?] asc", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Drug:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            mpm.Medication AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
                                INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id
                                INNER JOIN MetaPatientMedication mpm ON rim.ReportInstanceMedicationGuid = mpm.PatientMedicationGuid
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY t.MedDraTerm, mpm.Medication, [Istheadverseeventserious?]
                            ORDER BY t.MedDraTerm asc, mpm.Medication asc, [Istheadverseeventserious?] asc", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Cohort:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            cg.CohortName AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
                                INNER JOIN CohortGroupEnrolment cge ON mp.Id = cge.Patient_Id
                                INNER JOIN CohortGroup cg ON cge.CohortGroup_Id = cg.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY t.MedDraTerm, cg.CohortName, [Istheadverseeventserious?] 
                            ORDER BY t.MedDraTerm asc, cg.CohortName asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    default:
                        break;
                }
            }

            return _context.AdverseEventLists
                .FromSqlInterpolated($"Exec spAdverseEventList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<AdverseEventQuarterlyList> GetAdverseEventQuarterlyItems(DateTime searchFrom, DateTime searchTo)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT c.PeriodYear, c.PeriodQuarter, c.FacilityName, c.MedDraTerm, 
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
	                    WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0 
	                    GROUP BY DATEPART(YEAR, pce.OnsetDate), DATEPART(QUARTER, pce.OnsetDate), f.FacilityName, t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
	                    WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c
	            GROUP BY c.PeriodYear, c.PeriodQuarter, c.FacilityName, c.MedDraTerm
	            ORDER BY c.MedDraTerm, c.PeriodYear, c.PeriodQuarter, c.FacilityName
                    ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));

            return _context.AdverseEventQuarterlyLists
                .FromSqlInterpolated($"Exec spAdverseEventQuarterlyList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<AdverseEventAnnualList> GetAdverseEventAnnualItems(DateTime searchFrom, DateTime searchTo)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT c.PeriodYear, c.FacilityName, c.MedDraTerm, 
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
					WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' and pce.Archived = 0 and p.Archived = 0 
					GROUP BY DATEPART(YEAR, pce.OnsetDate), f.FacilityName, t5.MedDraTerm, mpce.SeverityGrade) as b on tm1.MedDraTerm = b.[Description] 
					WHERE tm1.MedDraTermType = 'SOC' AND b.PeriodYear IS NOT NULL) as c 
	            GROUP BY c.PeriodYear, c.FacilityName, c.MedDraTerm
	            ORDER BY c.MedDraTerm, c.PeriodYear, c.FacilityName
                    ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));

            return _context.AdverseEventAnnualLists
                .FromSqlInterpolated($"Exec spAdverseEventAnnualList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<CausalityNotSetList> GetCausalityNotSetItems(DateTime searchFrom, DateTime searchTo, CausalityConfigType causalityConfig, int facilityId, CausalityCriteria causalityCriteria)
        {
            string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";
            switch (causalityConfig)
            {
                case CausalityConfigType.BothScales:
                    where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.NaranjoCausality IS NOT NULL AND rim.WhoCausality IS NOT NULL " : " AND (rim.NaranjoCausality IS NULL OR rim.WhoCausality IS NULL) ";
                    break;

                case CausalityConfigType.WHOScale:
                    where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.WhoCausality IS NOT NULL " : " AND rim.WhoCausality IS NULL ";
                    break;

                case CausalityConfigType.NaranjoScale:
                    where += (causalityCriteria == CausalityCriteria.CausalitySet) ? " AND rim.NaranjoCausality IS NOT NULL " : " AND rim.NaranjoCausality IS NULL ";
                    break;
            }

            string sql = string.Format(@"
                SELECT p.Id AS Patient_Id, f.FacilityName, p.FirstName, p.Surname, pce.SourceTerminologyMedDra AS AdverseEvent, pce.OnsetDate, rim.NaranjoCausality, rim.WhoCausality, rim.MedicationIdentifier, pce.[Istheadverseeventserious?] AS Serious 
                    FROM ReportInstance ri
		                INNER JOIN MetaPatientClinicalEvent pce ON ri.ContextGuid = pce.PatientClinicalEventGuid 
		                INNER JOIN Patient p on pce.Patient_Id = p.Id
                        INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                        INNER JOIN Facility f ON pf.Facility_Id = f.Id 
                        INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id 
                WHERE pce.OnsetDate BETWEEN '{0}' AND '{1}' 
	                AND p.Archived = 0 {2} 
                ORDER BY pce.OnsetDate asc ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), where);

            return _context.CausalityNotSetLists
                .FromSqlInterpolated($"Exec spCausalityNotSetList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<OutstandingVisitList> GetOutstandingVisitItems(DateTime searchFrom, DateTime searchTo, int facilityId)
        {
            string where = facilityId > 0 ? " AND pf.Facility_Id = " + facilityId.ToString() : "";

            string sql = string.Format(@"
                SELECT p.Id AS Patient_Id, p.FirstName, p.Surname, a.AppointmentDate FROM Patient p 
	                INNER JOIN Appointment a ON p.Id = a.Patient_Id 
                    INNER JOIN PatientFacility pf ON pf.Id = (select top 1 Id from PatientFacility ipf where ipf.Patient_Id = p.Id and ipf.EnrolledDate <= GETDATE() order by ipf.EnrolledDate desc, ipf.Id desc)
                WHERE a.AppointmentDate < DATEADD(dd, 3, GETDATE())
	                AND a.AppointmentDate BETWEEN '{0}' AND '{1}' {2} 
	                AND a.Cancelled = 0
                    AND p.Archived = 0 and a.Archived = 0
	                AND NOT EXISTS(SELECT Id FROM Encounter ie WHERE ie.Patient_Id = p.Id AND ie.Archived = 0 AND ie.EncounterDate BETWEEN DATEADD(dd, -3, a.AppointmentDate) AND DATEADD(dd, 3, a.AppointmentDate))
                ORDER BY a.AppointmentDate desc ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), where);

            return _context.OutstandingVisitLists
                .FromSqlInterpolated($"Exec spOutstandingVisitList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<PatientOnStudyList> GetPatientOnStudyItems(DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria)
        {
            string sql = "";
            switch (patientOnStudyCriteria)
            {
                case PatientOnStudyCriteria.HasEncounterinDateRange:
                    sql = string.Format(@"
                        SELECT f.FacilityName, f.Id as FacilityId
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Encounter ie 
                                        inner join Patient ip on ie.Patient_Id = ip.Id
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ie.Archived = 0 and ip.Archived = 0 and ie.Archived = 0
                                        and ie.EncounterDate between '{0}' and '{1}'
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
                                        and ie.EncounterDate between '{0}' and '{1}'
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
                                        and ie.EncounterDate between '{0}' and '{1}'
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
                                        and ie.EncounterDate between '{0}' and '{1}'
                                        and ifa.Id = f.Id
                                ) AS PatientWithEventCount        
                        FROM Facility f
                        ORDER BY f.FacilityName 
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                    break;

                case PatientOnStudyCriteria.PatientRegisteredinFacilityinDateRange:
                    sql = string.Format(@"
                        SELECT f.FacilityName, f.Id as FacilityId
                            ,	(
                                    select count(distinct(ip.Id))
                                    from Patient ip 
                                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
                                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 and ipf.Archived = 0
                                        and ipf.EnrolledDate between '{0}' and '{1}'
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
                                        and ipf.EnrolledDate between '{0}' and '{1}'
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
                                        and ipf.EnrolledDate between '{0}' and '{1}'
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
                                        and ipf.EnrolledDate between '{0}' and '{1}'
                                        and ifa.Id = f.Id
                                ) AS PatientWithEventCount
                        FROM Facility f
                        ORDER BY f.FacilityName 
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                    break;

                default:
                    break;
            }

            return _context.PatientOnStudyLists
                .FromSqlInterpolated($"Exec spPatientOnStudyList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<PatientList> GetPatientListOnStudyItems(DateTime searchFrom, DateTime searchTo, PatientOnStudyCriteria patientOnStudyCriteria, int facilityId)
        {
            string sql = "";
            switch (patientOnStudyCriteria)
            {
                case PatientOnStudyCriteria.HasEncounterinDateRange:
                    sql = string.Format(@"
                        SELECT s.Id AS PatientId, s.FirstName, s.Surname, s.FacilityName  FROM 
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Encounter ie 
				                        inner join Patient ip on ie.Patient_Id = ip.Id
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
			                        where ie.Archived = 0 and ip.Archived = 0 
                                        and ie.EncounterDate between '{0}' and '{1}'
				                        and ifa.Id = {2}
				                    group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName 
		                        ) AS s
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), facilityId);
                    break;

                case PatientOnStudyCriteria.PatientRegisteredinFacilityinDateRange:
                    sql = string.Format(@"
                        SELECT s.Id AS PatientId, s.FirstName, s.Surname, s.FacilityName  FROM
	                        	(
			                        select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			                        from Patient ip 
				                        inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				                        inner join Facility ifa on ipf.Facility_Id = ifa.Id
                                    where ip.Archived = 0 
			                            and ip.Created between '{0}' and '{1}'
				                        and ifa.Id = {2}
		                        ) AS s
                        ", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"), facilityId);
                    break;

                default:
                    break;
            }

            return _context.PatientLists
                .FromSqlInterpolated($"Exec spPatientList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<DrugList> GetPatientsByDrugItems(string searchTerm)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT CONCAT(c.ConceptName collate SQL_Latin1_General_CP1_CI_AS, ' (', RTRIM(mf.[Description]) collate SQL_Latin1_General_CP1_CI_AS, ')') as ConceptName, c.Id as ConceptId 
	                ,(
		                select count(distinct(ip.Id))
		                from Patient ip
			                inner join PatientMedication ipm on ip.Id = ipm.Patient_Id 
			                inner join Concept ic on ipm.Concept_Id = ic.Id
		                where ic.Id = c.Id and ip.Archived = 0 and ipm.Archived = 0 
	                ) AS PatientCount
                FROM Concept c 
                    INNER JOIN MedicationForm mf on c.MedicationForm_Id = mf.Id 
                {0} 
                ORDER BY c.ConceptName", !String.IsNullOrWhiteSpace(searchTerm) ? $"WHERE c.ConceptName LIKE '%{searchTerm.TrimEnd()}%'" : "");

            return _context.DrugLists
                .FromSqlInterpolated($"Exec spDrugList {searchTerm}")
                .ToList();
        }

        public ICollection<PatientList> GetPatientListByDrugItems(int conceptId)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT s.Id AS PatientId, s.FirstName, s.Surname, s.FacilityName  FROM 
	                (
			            select ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
			            from Patient ip 
				            inner join PatientMedication ipm on ip.Id = ipm.Patient_Id 
				            inner join PatientFacility ipf on ip.Id = ipf.Patient_Id AND ipf.EnrolledDate = (select MAX(EnrolledDate) FROM PatientFacility iipf WHERE iipf.Patient_Id = ip.Id)
				            inner join Facility ifa on ipf.Facility_Id = ifa.Id
				            inner join Concept ic on ipm.Concept_Id = ic.Id
			            where ic.Id = {0} and ip.Archived = 0 and ipm.Archived = 0 
			            group by ip.Id, ip.FirstName, ip.Surname, ifa.FacilityName
		            ) AS s", conceptId);

            return _context.PatientLists
                .FromSqlInterpolated($"Exec spPatientList {conceptId}")
                .ToList();
        }
    }
}
