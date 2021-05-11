using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PVIMS.API.Helpers;
using PVIMS.API.Models.Parameters;
using System;

namespace PVIMS.API.Infrastructure.Services
{
    public class LinkGeneratorService : ILinkGeneratorService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _linkGenerator;

        public LinkGeneratorService(IHttpContextAccessor accessor,
            LinkGenerator linkGenerator)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }


        public string CreateIdResourceUriForWrapper(ResourceUriType type,
            string actionName,
            IdResourceParameters idResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName, 
                      new
                      {
                          orderBy = idResourceParameters.OrderBy,
                          pageNumber = idResourceParameters.PageNumber - 1,
                          pageSize = idResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          orderBy = idResourceParameters.OrderBy,
                          pageNumber = idResourceParameters.PageNumber + 1,
                          pageSize = idResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          orderBy = idResourceParameters.OrderBy,
                          pageNumber = idResourceParameters.PageNumber,
                          pageSize = idResourceParameters.PageSize
                      });
            }
        }

        public string CreateReportResourceUriForWrapper(ResourceUriType type,
            string actionName,
           BaseReportResourceParameters baseReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber - 1,
                          pageSize = baseReportResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber + 1,
                          pageSize = baseReportResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
                      new
                      {
                          pageNumber = baseReportResourceParameters.PageNumber,
                          pageSize = baseReportResourceParameters.PageSize
                      });
            }
        }

        public string CreateResourceUri(string resourceName, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"Get{resourceName}ByIdentifier",
              new { id = resourceId });
        }

        public string CreateResourceUriForReportInstance(string actionName, Guid workFlowGuid, long reportInstanceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, actionName,
              new { workFlowGuid, id = reportInstanceId });
        }

        public string CreateDeleteResourceUri(string resourceName, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"Delete{resourceName}",
              new { id = resourceId });
        }

        public string CreateAdverseEventReportResourceUri(ResourceUriType type,
            AdverseEventReportResourceParameters adverseEventReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAdverseEventReport",
                      new
                      {
                          pageNumber = adverseEventReportResourceParameters.PageNumber - 1,
                          pageSize = adverseEventReportResourceParameters.PageSize,
                          AdverseEventCriteria = adverseEventReportResourceParameters.AdverseEventCriteria,
                          AdverseEventStratifyCriteria = adverseEventReportResourceParameters.AdverseEventStratifyCriteria
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAdverseEventReport",
                      new
                      {
                          pageNumber = adverseEventReportResourceParameters.PageNumber + 1,
                          pageSize = adverseEventReportResourceParameters.PageSize,
                          AdverseEventCriteria = adverseEventReportResourceParameters.AdverseEventCriteria,
                          AdverseEventStratifyCriteria = adverseEventReportResourceParameters.AdverseEventStratifyCriteria
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAdverseEventReport",
                      new
                      {
                          pageNumber = adverseEventReportResourceParameters.PageNumber,
                          pageSize = adverseEventReportResourceParameters.PageSize,
                          AdverseEventCriteria = adverseEventReportResourceParameters.AdverseEventCriteria,
                          AdverseEventStratifyCriteria = adverseEventReportResourceParameters.AdverseEventStratifyCriteria
                      });
            }
        }

        public string CreateAnalyserTermSetsResourceUri(Guid workFlowGuid, 
            ResourceUriType type,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermsByIdentifier",
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
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermsByIdentifier",
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
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermsByIdentifier",
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

        public string CreateAnalyserTermPatientsResourceUri(Guid workFlowGuid,
            int termId,
            ResourceUriType type,
            AnalyserTermSetResourceParameters analyserTermSetResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermPatients",
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
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermPatients",
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
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAnalyserTermPatients",
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

        public string CreateAppointmentsResourceUri(ResourceUriType type,
            AppointmentResourceParameters appointmentResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAppointmentsByDetail",
                      new
                      {
                          orderBy = appointmentResourceParameters.OrderBy,
                          facilityName = appointmentResourceParameters.FacilityName,
                          pageNumber = appointmentResourceParameters.PageNumber - 1,
                          pageSize = appointmentResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAppointmentsByDetail",
                      new
                      {
                          orderBy = appointmentResourceParameters.OrderBy,
                          facilityName = appointmentResourceParameters.FacilityName,
                          pageNumber = appointmentResourceParameters.PageNumber + 1,
                          pageSize = appointmentResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAppointmentsByDetail",
                      new
                      {
                          orderBy = appointmentResourceParameters.OrderBy,
                          facilityName = appointmentResourceParameters.FacilityName,
                          pageNumber = appointmentResourceParameters.PageNumber,
                          pageSize = appointmentResourceParameters.PageSize
                      });
            }
        }

        public string CreateAuditLogsResourceUri(ResourceUriType type,
            AuditLogResourceParameters auditLogResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAuditLogsByIdentifier",
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
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAuditLogsByIdentifier",
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
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetAuditLogsByIdentifier",
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

        public string CreateCausalityReportResourceUri(Guid workFlowGuid, 
            ResourceUriType type,
            CausalityReportResourceParameters causalityReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = causalityReportResourceParameters.PageNumber - 1,
                          pageSize = causalityReportResourceParameters.PageSize,
                          FacilityId = causalityReportResourceParameters.FacilityId,
                          CausalityCriteria = causalityReportResourceParameters.CausalityCriteria
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = causalityReportResourceParameters.PageNumber + 1,
                          pageSize = causalityReportResourceParameters.PageSize,
                          FacilityId = causalityReportResourceParameters.FacilityId,
                          CausalityCriteria = causalityReportResourceParameters.CausalityCriteria
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCausalityReport",
                      new
                      {
                          workFlowGuid,
                          pageNumber = causalityReportResourceParameters.PageNumber,
                          pageSize = causalityReportResourceParameters.PageSize,
                          FacilityId = causalityReportResourceParameters.FacilityId,
                          CausalityCriteria = causalityReportResourceParameters.CausalityCriteria
                      });
            }
        }

        public string CreateConceptsResourceUri(ResourceUriType type,
           ConceptResourceParameters conceptResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConceptsByIdentifier",
                      new
                      {
                          orderBy = conceptResourceParameters.OrderBy,
                          pageNumber = conceptResourceParameters.PageNumber - 1,
                          pageSize = conceptResourceParameters.PageSize,
                          SearchTerm = conceptResourceParameters.SearchTerm,
                          Active = conceptResourceParameters.Active
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConceptsByIdentifier",
                      new
                      {
                          orderBy = conceptResourceParameters.OrderBy,
                          pageNumber = conceptResourceParameters.PageNumber + 1,
                          pageSize = conceptResourceParameters.PageSize,
                          SearchTerm = conceptResourceParameters.SearchTerm,
                          Active = conceptResourceParameters.Active
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConceptsByIdentifier",
                      new
                      {
                          orderBy = conceptResourceParameters.OrderBy,
                          pageNumber = conceptResourceParameters.PageNumber,
                          pageSize = conceptResourceParameters.PageSize,
                          SearchTerm = conceptResourceParameters.SearchTerm,
                          Active = conceptResourceParameters.Active
                      });
            }
        }

        public string CreateConditionsResourceUri(ResourceUriType type,
           ConditionResourceParameters conditionResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber - 1,
                          pageSize = conditionResourceParameters.PageSize,
                          Active = conditionResourceParameters.Active
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber + 1,
                          pageSize = conditionResourceParameters.PageSize,
                          Active = conditionResourceParameters.Active
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetConditionsByIdentifier",
                      new
                      {
                          orderBy = conditionResourceParameters.OrderBy,
                          pageNumber = conditionResourceParameters.PageNumber,
                          pageSize = conditionResourceParameters.PageSize,
                          Active = conditionResourceParameters.Active
                      });
            }
        }

        public string CreateCustomAttributesResourceUri(ResourceUriType type,
           CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber - 1,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber + 1,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
            }
        }

        public string CreateDatasetCategoryResourceUri(long datasetid, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetDatasetCategoryByIdentifier",
              new { datasetid, id = resourceId });
        }

        public string CreateDatasetCategoryElementResourceUri(long datasetid, long datasetCategoryId, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetDatasetCategoryElementByIdentifier",
              new { datasetid, datasetCategoryId, id = resourceId });
        }

        public string CreateDownloadActivitySingleAttachmentResourceUri(Guid workFlowGuid, long reportinstanceId, long activityExecutionStatusEventId, long attachmentId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"DownloadActivitySingleAttachment",
              new { workFlowGuid, reportinstanceId, activityExecutionStatusEventId, id = attachmentId });
        }

        public string CreateEncountersResourceUri(ResourceUriType type,
           EncounterResourceParameters encounterResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetEncountersByDetail",
                      new
                      {
                          orderBy = encounterResourceParameters.OrderBy,
                          facilityName = encounterResourceParameters.FacilityName,
                          pageNumber = encounterResourceParameters.PageNumber - 1,
                          pageSize = encounterResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetEncountersByDetail",
                      new
                      {
                          orderBy = encounterResourceParameters.OrderBy,
                          facilityName = encounterResourceParameters.FacilityName,
                          pageNumber = encounterResourceParameters.PageNumber + 1,
                          pageSize = encounterResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetEncountersByDetail",
                      new
                      {
                          orderBy = encounterResourceParameters.OrderBy,
                          facilityName = encounterResourceParameters.FacilityName,
                          pageNumber = encounterResourceParameters.PageNumber,
                          pageSize = encounterResourceParameters.PageSize
                      });
            }
        }

        public string CreateEncounterForPatientResourceUri(long patientId, long encounterId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetEncounterForPatientByIdentifier",
              new { patientId, id = encounterId });
        }

        public string CreateEnrolmentForPatientResourceUri(long patientId, long enrolmentId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetEnrolmentForPatient",
              new { patientId, id = enrolmentId });
        }

        public string CreateFacilitiesResourceUri(ResourceUriType type,
           FacilityResourceParameters facilityResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetFacilitiesByIdentifier",
                      new
                      {
                          orderBy = facilityResourceParameters.OrderBy,
                          pageNumber = facilityResourceParameters.PageNumber - 1,
                          pageSize = facilityResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetFacilitiesByIdentifier",
                      new
                      {
                          orderBy = facilityResourceParameters.OrderBy,
                          pageNumber = facilityResourceParameters.PageNumber + 1,
                          pageSize = facilityResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetFacilitiesByIdentifier",
                      new
                      {
                          orderBy = facilityResourceParameters.OrderBy,
                          pageNumber = facilityResourceParameters.PageNumber,
                          pageSize = facilityResourceParameters.PageSize
                      });
            }
        }

        public string CreateMetaWidgetResourceUri(long metaPageId, long metaWidgetId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetMetaWidgetByIdentifier",
              new { metaPageId, id = metaWidgetId });
        }

        public string CreateNewAppointmentForPatientResourceUri(long patientId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"CreatePatientAppointment",
              new { patientId });
        }

        public string CreateNewEnrolmentForPatientResourceUri(long patientId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"CreatePatientEnrolment",
              new { patientId });
        }

        public string CreateOutstandingVisitReportResourceUri(ResourceUriType type,
           OutstandingVisitResourceParameters outstandingVisitResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = outstandingVisitResourceParameters.PageNumber - 1,
                          pageSize = outstandingVisitResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = outstandingVisitResourceParameters.PageNumber + 1,
                          pageSize = outstandingVisitResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetOutstandingVisitReport",
                      new
                      {
                          pageNumber = outstandingVisitResourceParameters.PageNumber,
                          pageSize = outstandingVisitResourceParameters.PageSize
                      });
            }
        }

        public string CreatePatientAppointmentResourceUri(long patientId, long resourceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetAppointmentByIdentifier",
              new { patientId, id = resourceId });
        }

        public string CreatePatientsResourceUri(ResourceUriType type,
           PatientResourceParameters patientResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientsByIdentifier",
                      new
                      {
                          orderBy = patientResourceParameters.OrderBy,
                          facilityName = patientResourceParameters.FacilityName,
                          pageNumber = patientResourceParameters.PageNumber - 1,
                          pageSize = patientResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientsByIdentifier",
                      new
                      {
                          orderBy = patientResourceParameters.OrderBy,
                          facilityName = patientResourceParameters.FacilityName,
                          pageNumber = patientResourceParameters.PageNumber + 1,
                          pageSize = patientResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientsByIdentifier",
                      new
                      {
                          orderBy = patientResourceParameters.OrderBy,
                          facilityName = patientResourceParameters.FacilityName,
                          pageNumber = patientResourceParameters.PageNumber,
                          pageSize = patientResourceParameters.PageSize
                      });
            }
        }

        public string CreatePatientMedicationReportResourceUri(ResourceUriType type,
           PatientMedicationReportResourceParameters patientMedicationReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber - 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize,
                          SearchTerm = patientMedicationReportResourceParameters.SearchTerm
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber + 1,
                          pageSize = patientMedicationReportResourceParameters.PageSize,
                          SearchTerm = patientMedicationReportResourceParameters.SearchTerm
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientByMedicationReport",
                      new
                      {
                          pageNumber = patientMedicationReportResourceParameters.PageNumber,
                          pageSize = patientMedicationReportResourceParameters.PageSize,
                          SearchTerm = patientMedicationReportResourceParameters.SearchTerm
                      });
            }
        }

        public string CreatePatientTreatmentReportResourceUri(ResourceUriType type,
           PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = patientTreatmentReportResourceParameters.PageNumber - 1,
                          pageSize = patientTreatmentReportResourceParameters.PageSize,
                          PatientOnStudyCriteria = patientTreatmentReportResourceParameters.PatientOnStudyCriteria
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = patientTreatmentReportResourceParameters.PageNumber + 1,
                          pageSize = patientTreatmentReportResourceParameters.PageSize,
                          PatientOnStudyCriteria = patientTreatmentReportResourceParameters.PatientOnStudyCriteria
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetPatientTreatmentReport",
                      new
                      {
                          pageNumber = patientTreatmentReportResourceParameters.PageNumber,
                          pageSize = patientTreatmentReportResourceParameters.PageSize,
                          PatientOnStudyCriteria = patientTreatmentReportResourceParameters.PatientOnStudyCriteria
                      });
            }
        }

        public string CreateProductsResourceUri(ResourceUriType type,
           ProductResourceParameters productResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetProductsByIdentifier",
                      new
                      {
                          orderBy = productResourceParameters.OrderBy,
                          pageNumber = productResourceParameters.PageNumber - 1,
                          pageSize = productResourceParameters.PageSize,
                          SearchTerm = productResourceParameters.SearchTerm,
                          Active = productResourceParameters.Active
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetProductsByIdentifier",
                      new
                      {
                          orderBy = productResourceParameters.OrderBy,
                          pageNumber = productResourceParameters.PageNumber + 1,
                          pageSize = productResourceParameters.PageSize,
                          SearchTerm = productResourceParameters.SearchTerm,
                          Active = productResourceParameters.Active
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetProductsByIdentifier",
                      new
                      {
                          orderBy = productResourceParameters.OrderBy,
                          pageNumber = productResourceParameters.PageNumber,
                          pageSize = productResourceParameters.PageSize,
                          SearchTerm = productResourceParameters.SearchTerm,
                          Active = productResourceParameters.Active
                      });
            }
        }

        public string CreateReportInstanceResourceUri(Guid workFlowGuid, int reportInstanceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"GetReportInstanceByIdentifier",
              new { workFlowGuid, id = reportInstanceId });
        }

        public string CreateReportInstancesResourceUri(Guid workFlowGuid, 
           ResourceUriType type,
           string orderBy,
           string qualifiedName,
           DateTime searchFrom,
           DateTime searchTo,
           int pageNumber,
           int pageSize)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy,
                          qualifiedName,
                          searchFrom,
                          searchTo,
                          pageNumber = pageNumber - 1,
                          pageSize
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy,
                          qualifiedName,
                          searchFrom,
                          searchTo,
                          pageNumber = pageNumber + 1,
                          pageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetReportInstancesByDetail",
                      new
                      {
                          workFlowGuid,
                          orderBy,
                          qualifiedName,
                          searchFrom,
                          searchTo,
                          pageNumber,
                          pageSize
                      });
            }
        }

        public string CreateUpdateDatasetInstanceResourceUri(long datasetId, long datasetInstanceId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"UpdateDataset",
              new { datasetId, id = datasetInstanceId });
        }

        public string CreateUpdateDeenrolmentForPatientResourceUri(long patientId, long cohortGroupEnrolmentId)
        {
            return _linkGenerator.GetPathByName(_accessor.HttpContext, $"UpdatePatientDeenrolment",
              new { patientId, cohortGroupEnrolmentId });
        }

        public string CreateUsersResourceUri(ResourceUriType type,
           UserResourceParameters userResourceParameters)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber - 1,
                          pageSize = userResourceParameters.PageSize,
                          SearchTerm = userResourceParameters.SearchTerm
                      });
                case ResourceUriType.NextPage:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber + 1,
                          pageSize = userResourceParameters.PageSize,
                          SearchTerm = userResourceParameters.SearchTerm
                      });
                case ResourceUriType.Current:
                default:
                    return _linkGenerator.GetPathByName(_accessor.HttpContext, "GetUsersByIdentifier",
                      new
                      {
                          orderBy = userResourceParameters.OrderBy,
                          pageNumber = userResourceParameters.PageNumber,
                          pageSize = userResourceParameters.PageSize,
                          SearchTerm = userResourceParameters.SearchTerm
                      });
            }
        }
    }
}
