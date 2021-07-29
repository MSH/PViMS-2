using PVIMS.API.Helpers;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Models.ValueTypes;
using System;

namespace PVIMS.API.Infrastructure.Services
{
    public interface ILinkGeneratorService
    {
        string CreateIdResourceUriForWrapper(ResourceUriType type, string actionName, string orderBy, int pageNumber, int pageSize);

        string CreateReportResourceUriForWrapper(ResourceUriType type, string actionName, BaseReportResourceParameters baseReportResourceParameters);

        string CreateResourceUri(string resourceName, long resourceId);

        string CreateResourceUriForReportInstance(string actionName, Guid workFlowGuid, long reportInstanceId);

        string CreateDeleteResourceUri(string resourceName, long resourceId);

        string CreateAdverseEventReportResourceUri(ResourceUriType type, AdverseEventReportResourceParameters adverseEventReportResourceParameters);

        string CreateAnalyserTermSetsResourceUri(Guid workFlowGuid, ResourceUriType type, AnalyserTermSetResourceParameters analyserTermSetResourceParameters);

        string CreateAnalyserTermPatientsResourceUri(Guid workFlowGuid, int termId, ResourceUriType type, AnalyserTermSetResourceParameters analyserTermSetResourceParameters);

        string CreateAppointmentsResourceUri(ResourceUriType type, AppointmentResourceParameters appointmentResourceParameters);

        string CreateAuditLogsResourceUri(ResourceUriType type, AuditLogResourceParameters auditLogResourceParameters);

        string CreateCausalityReportResourceUri(Guid workFlowGuid, ResourceUriType type, CausalityReportResourceParameters causalityReportResourceParameters);

        string CreateConceptsResourceUri(ResourceUriType type, string orderBy, string searchTerm, YesNoBothValueType active, int pageNumber, int pageSize);

        string CreateConditionsResourceUri(ResourceUriType type, ConditionResourceParameters conditionResourceParameters);

        string CreateCustomAttributesResourceUri(ResourceUriType type, CustomAttributeResourceParameters customAttributeResourceParameters);

        string CreateDatasetCategoryResourceUri(long datasetid, long resourceId);

        string CreateDatasetCategoryElementResourceUri(long datasetid, long datasetCategoryId, long resourceId);

        string CreateDownloadActivitySingleAttachmentResourceUri(Guid workFlowGuid, long reportinstanceId, long activityExecutionStatusEventId, long attachmentId);

        string CreateEncountersResourceUri(ResourceUriType type, string orderBy, string facilityName, int pageNumber, int pageSize);

        string CreateEncounterForPatientResourceUri(long patientId, long encounterId);

        string CreateEnrolmentForPatientResourceUri(long patientId, long enrolmentId);

        string CreateMetaWidgetResourceUri(long metaPageId, long metaWidgetId);

        string CreateNewAppointmentForPatientResourceUri(long patientId);

        string CreateNewEnrolmentForPatientResourceUri(long patientId);

        string CreateOutstandingVisitReportResourceUri(ResourceUriType type, OutstandingVisitResourceParameters outstandingVisitResourceParameters);

        string CreatePatientAppointmentResourceUri(long patientId, long resourceId);

        string CreatePatientsResourceUri(ResourceUriType type, string orderBy, string facilityName, int pageNumber, int pageSize);

        string CreatePatientMedicationReportResourceUri(ResourceUriType type, PatientMedicationReportResourceParameters patientMedicationReportResourceParameters);

        string CreatePatientTreatmentReportResourceUri(ResourceUriType type, PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters);

        string CreateProductsResourceUri(ResourceUriType type, ProductResourceParameters productResourceParameters);

        string CreateReportInstanceResourceUri(Guid workFlowGuid, int reportInstanceId);

        string CreateReportInstancesResourceUri(Guid workFlowGuid, ResourceUriType type, string orderBy, string qualifiedName, DateTime searchFrom, DateTime searchTo, int pageNumber, int pageSize);

        string CreateUpdateDatasetInstanceResourceUri(long datasetId, long datasetInstanceId);

        string CreateUpdateDeenrolmentForPatientResourceUri(long patientId, long cohortGroupEnrolmentId);

        string CreateUsersResourceUri(ResourceUriType type, UserResourceParameters userResourceParameters);
    }
}
