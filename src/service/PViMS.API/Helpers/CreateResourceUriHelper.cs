using Microsoft.AspNetCore.Mvc;
using PVIMS.API.Models.Parameters;
using System;

namespace PVIMS.API.Helpers
{
    public static class CreateResourceUriHelper
    {
        public static string CreateAnalyserTermSetsResourceUri(IUrlHelper urlHelper,
            Guid workFlowGuid,
            ResourceUriType type,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAnalyserTermsByIdentifier",
                      new
                      {
                          workFlowGuid,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber - 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAnalyserTermsByIdentifier",
                      new
                      {
                          workFlowGuid,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber + 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAnalyserTermsByIdentifier",
                    new
                    {
                        workFlowGuid,
                        conditionId = analyserTermSetResourceParameters.ConditionId,
                        cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                        searchFrom = analyserTermSetResourceParameters.SearchFrom,
                        searchTo = analyserTermSetResourceParameters.SearchTo,
                        riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                        pageNumber = analyserTermSetResourceParameters.PageNumber,
                        pageSize = analyserTermSetResourceParameters.PageSize
                    });
            }
        }

        public static string CreateAnalyserTermPatientsResourceUri(IUrlHelper urlHelper,
            Guid workFlowGuid,
            int termId,
            ResourceUriType type,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAnalyserTermPatients",
                      new
                      {
                          workFlowGuid,
                          id = termId,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber - 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAnalyserTermPatients",
                      new
                      {
                          workFlowGuid,
                          id = termId,
                          conditionId = analyserTermSetResourceParameters.ConditionId,
                          cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                          searchFrom = analyserTermSetResourceParameters.SearchFrom,
                          searchTo = analyserTermSetResourceParameters.SearchTo,
                          riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                          pageNumber = analyserTermSetResourceParameters.PageNumber + 1,
                          pageSize = analyserTermSetResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAnalyserTermPatients",
                    new
                    {
                        workFlowGuid,
                        id = termId,
                        conditionId = analyserTermSetResourceParameters.ConditionId,
                        cohortGroupId = analyserTermSetResourceParameters.CohortGroupId,
                        searchFrom = analyserTermSetResourceParameters.SearchFrom,
                        searchTo = analyserTermSetResourceParameters.SearchTo,
                        riskFactorOptionNames = analyserTermSetResourceParameters.RiskFactorOptionNames,
                        pageNumber = analyserTermSetResourceParameters.PageNumber,
                        pageSize = analyserTermSetResourceParameters.PageSize
                    });
            }
        }

        public static string CreateAppointmentsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            AppointmentResourceParameters appointmentResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAppointmentsByDetail",
                      new
                      {
                          orderBy = appointmentResourceParameters.OrderBy,
                          facilityName = appointmentResourceParameters.FacilityName,
                          pageNumber = appointmentResourceParameters.PageNumber - 1,
                          pageSize = appointmentResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAppointmentsByDetail",
                      new
                      {
                          orderBy = appointmentResourceParameters.OrderBy,
                          facilityName = appointmentResourceParameters.FacilityName,
                          pageNumber = appointmentResourceParameters.PageNumber + 1,
                          pageSize = appointmentResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAppointmentsByDetail",
                    new
                    {
                        orderBy = appointmentResourceParameters.OrderBy,
                        facilityName = appointmentResourceParameters.FacilityName,
                        pageNumber = appointmentResourceParameters.PageNumber,
                        pageSize = appointmentResourceParameters.PageSize
                    });
            }
        }

        public static string CreateAppointmentForPatientResourceUri(IUrlHelper urlHelper, long patientId, long appointmentId)
        {
            return urlHelper.Link("GetAppointmentForPatientByIdentifier",
              new { patientId, id = appointmentId });
        }

        public static string CreateNewAppointmentForPatientResourceUri(IUrlHelper urlHelper, long patientId)
        {
            return urlHelper.Link("CreatePatientAppointment",
              new { patientId });
        }

        public static string CreateCohortGroupsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            CohortGroupResourceParameters cohortGroupResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetCohortGroupsByDetail",
                      new
                      {
                          orderBy = cohortGroupResourceParameters.OrderBy,
                          pageNumber = cohortGroupResourceParameters.PageNumber - 1,
                          pageSize = cohortGroupResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetCohortGroupsByDetail",
                      new
                      {
                          orderBy = cohortGroupResourceParameters.OrderBy,
                          pageNumber = cohortGroupResourceParameters.PageNumber + 1,
                          pageSize = cohortGroupResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetCohortGroupsByDetail",
                    new
                    {
                        orderBy = cohortGroupResourceParameters.OrderBy,
                        pageNumber = cohortGroupResourceParameters.PageNumber,
                        pageSize = cohortGroupResourceParameters.PageSize
                    });
            }
        }

        public static string CreateConditionsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            ConditionResourceParameters conditionResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber - 1,
                          pageSize = conditionResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber + 1,
                          pageSize = conditionResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetConditionsByIdentifier",
                    new
                    {
                        orderBy = conditionResourceParameters.OrderBy,
                        pageNumber = conditionResourceParameters.PageNumber,
                        pageSize = conditionResourceParameters.PageSize
                    });
            }
        }

        public static string CreateConceptsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            ConceptResourceParameters conceptResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetConceptsByIdentifier",
                      new
                      {
                          orderBy = conceptResourceParameters.OrderBy,
                          pageNumber = conceptResourceParameters.PageNumber - 1,
                          pageSize = conceptResourceParameters.PageSize,
                          SearchTerm = conceptResourceParameters.SearchTerm
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetConceptsByIdentifier",
                      new
                      {
                          orderBy = conceptResourceParameters.OrderBy,
                          pageNumber = conceptResourceParameters.PageNumber + 1,
                          pageSize = conceptResourceParameters.PageSize,
                          SearchTerm = conceptResourceParameters.SearchTerm
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetConceptsByIdentifier",
                    new
                    {
                        orderBy = conceptResourceParameters.OrderBy,
                        pageNumber = conceptResourceParameters.PageNumber,
                        pageSize = conceptResourceParameters.PageSize,
                        SearchTerm = conceptResourceParameters.SearchTerm
                    });
            }
        }

        public static string CreateDatasetCategoryResourceUri(IUrlHelper urlHelper, string resourceName, long datasetid, long resourceId)
        {
            return urlHelper.Link($"Get{resourceName}ByIdentifier",
              new { datasetid, id = resourceId });
        }

        public static string CreateDownloadActivitySingleAttachmentResourceUri(IUrlHelper urlHelper, Guid workFlowGuid, long reportinstanceId, long activityExecutionStatusEventId, long attachmentId)
        {
            return urlHelper.Link("DownloadActivitySingleAttachment",
              new { workFlowGuid, reportinstanceId, activityExecutionStatusEventId, id = attachmentId });
        }

        public static string CreateE2BInstanceResourceUri(IUrlHelper urlHelper, Guid workFlowGuid, long reportInstanceId)
        {
            return urlHelper.Link("CreateE2BInstance",
              new { workFlowGuid, id = reportInstanceId });
        }

        public static string CreatePatientAppointmentResourceUri(IUrlHelper urlHelper, string resourceName, long patientId, long resourceId)
        {
            return urlHelper.Link($"Get{resourceName}ByIdentifier",
              new { patientId, id = resourceId });
        }

        public static string CreateEncounterForPatientResourceUri(IUrlHelper urlHelper, long patientId, long encounterId)
        {
            return urlHelper.Link("GetEncounterForPatientByIdentifier",
              new { patientId, id = encounterId });
        }

        public static string CreateEncountersResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            EncounterResourceParameters encounterResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetEncountersByDetail",
                      new
                      {
                          orderBy = encounterResourceParameters.OrderBy,
                          facilityName = encounterResourceParameters.FacilityName,
                          pageNumber = encounterResourceParameters.PageNumber - 1,
                          pageSize = encounterResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetEncountersByDetail",
                      new
                      {
                          orderBy = encounterResourceParameters.OrderBy,
                          facilityName = encounterResourceParameters.FacilityName,
                          pageNumber = encounterResourceParameters.PageNumber + 1,
                          pageSize = encounterResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetEncountersByDetail",
                    new
                    {
                        orderBy = encounterResourceParameters.OrderBy,
                        facilityName = encounterResourceParameters.FacilityName,
                        pageNumber = encounterResourceParameters.PageNumber,
                        pageSize = encounterResourceParameters.PageSize
                    });
            }
        }

        public static string CreateEnrolmentForPatientResourceUri(IUrlHelper urlHelper, long patientId, long enrolmentId)
        {
            return urlHelper.Link("GetEnrolmentForPatient",
              new { patientId, id = enrolmentId });
        }

        public static string CreateNewEnrolmentForPatientResourceUri(IUrlHelper urlHelper, long patientId)
        {
            return urlHelper.Link("CreatePatientEnrolment",
              new { patientId });
        }

        public static string CreateUpdateDatasetResourceUri(IUrlHelper urlHelper, long datasetId)
        {
            return urlHelper.Link("UpdateDataset",
              new { id = datasetId });
        }

        public static string CreateUpdateDatasetInstanceResourceUri(IUrlHelper urlHelper, long datasetId, long datasetInstanceId)
        {
            return urlHelper.Link("UpdateDataset",
              new { datasetId, id = datasetInstanceId });
        }

        public static string CreateUpdateDeenrolmentForPatientResourceUri(IUrlHelper urlHelper, long patientId, long cohortGroupEnrolmentId)
        {
            return urlHelper.Link("UpdatePatientDeenrolment",
              new { patientId, cohortGroupEnrolmentId });
        }

        public static string CreateUpdateReportInstanceStatusResourceUri(IUrlHelper urlHelper, Guid workFlowGuid, long reportInstanceId)
        {
            return urlHelper.Link("UpdateReportInstanceStatus",
              new { workFlowGuid, id = reportInstanceId });
        }

        public static string CreateUpdateReportInstanceTerminologyResourceUri(IUrlHelper urlHelper, Guid workFlowGuid, long reportInstanceId)
        {
            return urlHelper.Link("UpdateReportInstanceTerminology",
              new { workFlowGuid, id = reportInstanceId });
        }

        public static string CreateUpdateReportInstanceMedicationCausalityResourceUri(IUrlHelper urlHelper, Guid workFlowGuid, long reportInstanceId, long reportInstanceMedicationId)
        {
            return urlHelper.Link("UpdateReportInstanceMedicationCausality",
              new { workFlowGuid, reportInstanceId, id = reportInstanceMedicationId });
        }

        public static string CreateAuditLogsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            AuditLogResourceParameters auditLogResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAuditLogsByIdentifier",
                      new
                      {
                          orderBy = auditLogResourceParameters.OrderBy,
                          auditType = auditLogResourceParameters.AuditType,
                          searchFrom = auditLogResourceParameters.SearchFrom,
                          searchTo = auditLogResourceParameters.SearchTo,
                          facilityId = auditLogResourceParameters.FacilityId,
                          pageNumber = auditLogResourceParameters.PageNumber - 1,
                          pageSize = auditLogResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAuditLogsByIdentifier",
                      new
                      {
                          orderBy = auditLogResourceParameters.OrderBy,
                          auditType = auditLogResourceParameters.AuditType,
                          searchFrom = auditLogResourceParameters.SearchFrom,
                          searchTo = auditLogResourceParameters.SearchTo,
                          facilityId = auditLogResourceParameters.FacilityId,
                          pageNumber = auditLogResourceParameters.PageNumber + 1,
                          pageSize = auditLogResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAuditLogsByIdentifier",
                    new
                    {
                        orderBy = auditLogResourceParameters.OrderBy,
                        auditType = auditLogResourceParameters.AuditType,
                        searchFrom = auditLogResourceParameters.SearchFrom,
                        searchTo = auditLogResourceParameters.SearchTo,
                        facilityId = auditLogResourceParameters.FacilityId,
                        pageNumber = auditLogResourceParameters.PageNumber,
                        pageSize = auditLogResourceParameters.PageSize
                    });
            }
        }

        public static string CreateFacilitiesResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            FacilityResourceParameters facilityResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetFacilitiesByIdentifier",
                      new
                      {
                          orderBy = facilityResourceParameters.OrderBy,
                          pageNumber = facilityResourceParameters.PageNumber - 1,
                          pageSize = facilityResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetFacilitiesByIdentifier",
                      new
                      {
                          orderBy = facilityResourceParameters.OrderBy,
                          pageNumber = facilityResourceParameters.PageNumber + 1,
                          pageSize = facilityResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetFacilitiesByIdentifier",
                    new
                    {
                        orderBy = facilityResourceParameters.OrderBy,
                        pageNumber = facilityResourceParameters.PageNumber,
                        pageSize = facilityResourceParameters.PageSize
                    });
            }
        }

        public static string CreateUsersResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            UserResourceParameters userResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber - 1,
                          pageSize = userResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber + 1,
                          pageSize = userResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetUsersByIdentifier",
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber,
                        pageSize = userResourceParameters.PageSize
                    });
            }
        }

        public static string CreatePatientsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            PatientResourceParameters patientResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetPatientsByIdentifier",
                      new
                      {
                          orderBy = patientResourceParameters.OrderBy,
                          facilityName = patientResourceParameters.FacilityName,
                          pageNumber = patientResourceParameters.PageNumber - 1,
                          pageSize = patientResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetPatientsByIdentifier",
                      new
                      {
                          orderBy = patientResourceParameters.OrderBy,
                          facilityName = patientResourceParameters.FacilityName,
                          pageNumber = patientResourceParameters.PageNumber + 1,
                          pageSize = patientResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetPatientsByIdentifier",
                    new
                    {
                        orderBy = patientResourceParameters.OrderBy,
                        facilityName = patientResourceParameters.FacilityName,
                        pageNumber = patientResourceParameters.PageNumber,
                        pageSize = patientResourceParameters.PageSize
                    });
            }
        }

        public static string CreateCausalityReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            Guid workFlowGuid,
            CausalityReportResourceParameters causalityReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = causalityReportResourceParameters.PageNumber - 1,
                          pageSize = causalityReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = causalityReportResourceParameters.PageNumber + 1,
                          pageSize = causalityReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetCausalityReport",
                    new
                    {
                        workFlowGuid,
                        pageNumber = causalityReportResourceParameters.PageNumber,
                        pageSize = causalityReportResourceParameters.PageSize
                    });
            }
        }

        public static string CreateOutstandingVisitReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            OutstandingVisitResourceParameters outstandingVisitResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = outstandingVisitResourceParameters.PageNumber - 1,
                          pageSize = outstandingVisitResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = outstandingVisitResourceParameters.PageNumber + 1,
                          pageSize = outstandingVisitResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetOutstandingVisitReport",
                    new
                    {
                        pageNumber = outstandingVisitResourceParameters.PageNumber,
                        pageSize = outstandingVisitResourceParameters.PageSize
                    });
            }
        }

        public static string CreatePatientMedicationReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            PatientMedicationReportResourceParameters patientMedicationReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber - 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber + 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetPatientByMedicationReport",
                    new
                    {
                        pageNumber = patientMedicationReportResourceParameters.PageNumber,
                        pageSize = patientMedicationReportResourceParameters.PageSize
                    });
            }
        }

        public static string CreatePatientTreatmentReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            PatientTreatmentReportResourceParameters patientMedicationReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber - 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber + 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetPatientTreatmentReport",
                    new
                    {
                        pageNumber = patientMedicationReportResourceParameters.PageNumber,
                        pageSize = patientMedicationReportResourceParameters.PageSize
                    });
            }
        }

        public static string CreateAdverseEventReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            AdverseEventReportResourceParameters adverseEventReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAdverseEventReport",
                      new
                      {
                          pageNumber = adverseEventReportResourceParameters.PageNumber - 1,
                          pageSize = adverseEventReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAdverseEventReport",
                      new
                      {
                          pageNumber = adverseEventReportResourceParameters.PageNumber + 1,
                          pageSize = adverseEventReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAdverseEventReport",
                    new
                    {
                        pageNumber = adverseEventReportResourceParameters.PageNumber,
                        pageSize = adverseEventReportResourceParameters.PageSize
                    });
            }
        }

        public static string CreateQuarterlyAdverseEventReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            BaseReportResourceParameters baseReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAdverseEventQuarterlyReport",
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber - 1,
                          pageSize = baseReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAdverseEventQuarterlyReport",
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber + 1,
                          pageSize = baseReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAdverseEventQuarterlyReport",
                    new
                    {
                        pageNumber = baseReportResourceParameters.PageNumber,
                        pageSize = baseReportResourceParameters.PageSize
                    });
            }
        }

        public static string CreateAnnualAdverseEventReportResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            BaseReportResourceParameters baseReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetAdverseEventAnnualReport",
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber - 1,
                          pageSize = baseReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetAdverseEventAnnualReport",
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber + 1,
                          pageSize = baseReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetAdverseEventAnnualReport",
                    new
                    {
                        pageNumber = baseReportResourceParameters.PageNumber,
                        pageSize = baseReportResourceParameters.PageSize
                    });
            }
        }

        public static string CreateProductsResourceUri(IUrlHelper urlHelper,
            ResourceUriType type,
            ProductResourceParameters productResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetProductsByIdentifier",
                      new
                      {
                          orderBy = productResourceParameters.OrderBy,
                          pageNumber = productResourceParameters.PageNumber - 1,
                          pageSize = productResourceParameters.PageSize,
                          SearchTerm = productResourceParameters.SearchTerm
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetProductsByIdentifier",
                      new
                      {
                          orderBy = productResourceParameters.OrderBy,
                          pageNumber = productResourceParameters.PageNumber + 1,
                          pageSize = productResourceParameters.PageSize,
                          SearchTerm = productResourceParameters.SearchTerm
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetProductsByIdentifier",
                    new
                    {
                        orderBy = productResourceParameters.OrderBy,
                        pageNumber = productResourceParameters.PageNumber,
                        pageSize = productResourceParameters.PageSize,
                        SearchTerm = productResourceParameters.SearchTerm
                    });
            }
        }

        public static string CreateReportInstancesResourceUri(IUrlHelper urlHelper,
            Guid workFlowGuid,
            ResourceUriType type,
            ReportInstanceResourceParameters reportInstanceResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy = reportInstanceResourceParameters.OrderBy,
                          qualifiedName = reportInstanceResourceParameters.QualifiedName,
                          searchFrom = reportInstanceResourceParameters.SearchFrom,
                          searchTo = reportInstanceResourceParameters.SearchTo,
                          pageNumber = reportInstanceResourceParameters.PageNumber - 1,
                          pageSize = reportInstanceResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy = reportInstanceResourceParameters.OrderBy,
                          qualifiedName = reportInstanceResourceParameters.QualifiedName,
                          searchFrom = reportInstanceResourceParameters.SearchFrom,
                          searchTo = reportInstanceResourceParameters.SearchTo,
                          pageNumber = reportInstanceResourceParameters.PageNumber + 1,
                          pageSize = reportInstanceResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetReportInstancesByDetail",
                    new
                    {
                        workFlowGuid,
                        orderBy = reportInstanceResourceParameters.OrderBy,
                        qualifiedName = reportInstanceResourceParameters.QualifiedName,
                        searchFrom = reportInstanceResourceParameters.SearchFrom,
                        searchTo = reportInstanceResourceParameters.SearchTo,
                        pageNumber = reportInstanceResourceParameters.PageNumber,
                        pageSize = reportInstanceResourceParameters.PageSize
                    });
            }
        }

        public static string CreateNewReportInstancesResourceUri(IUrlHelper urlHelper,
            Guid workFlowGuid,
            ResourceUriType type,
            ReportInstanceNewResourceParameters reportInstanceResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return urlHelper.Link("GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy = reportInstanceResourceParameters.OrderBy,
                          searchTerm = reportInstanceResourceParameters.SearchTerm,
                          pageNumber = reportInstanceResourceParameters.PageNumber - 1,
                          pageSize = reportInstanceResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return urlHelper.Link("GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy = reportInstanceResourceParameters.OrderBy,
                          searchTerm = reportInstanceResourceParameters.SearchTerm,
                          pageNumber = reportInstanceResourceParameters.PageNumber + 1,
                          pageSize = reportInstanceResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return urlHelper.Link("GetReportInstancesByDetail",
                    new
                    {
                        workFlowGuid,
                        orderBy = reportInstanceResourceParameters.OrderBy,
                        searchTerm = reportInstanceResourceParameters.SearchTerm,
                        pageNumber = reportInstanceResourceParameters.PageNumber,
                        pageSize = reportInstanceResourceParameters.PageSize
                    });
            }
        }

        public static string CreateReportInstanceResourceUri(IUrlHelper urlHelper, Guid workFlowGuid, int reportInstanceId)
        {
            return urlHelper.Link("GetReportInstanceByIdentifier",
              new { workFlowGuid, id = reportInstanceId });
        }

        public static string CreateResourceUri(IUrlHelper urlHelper, string resourceName, long resourceId)
        {
            return urlHelper.Link($"Get{resourceName}ByIdentifier",
              new { id = resourceId });
        }

        public static string CreateDeleteResourceUri(IUrlHelper urlHelper, string resourceName, long resourceId)
        {
            return urlHelper.Link($"Delete{resourceName}",
              new { id = resourceId });
        }

    }
}
