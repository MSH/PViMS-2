using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IWorkFlowService
    {
        void AddOrUpdateMedicationsForWorkFlowInstance(Guid contextGuid, List<ReportInstanceMedicationListItem> medications);

        Task CreateWorkFlowInstanceAsync(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier);

        int CheckWorkFlowInstanceCount(string workFlowName);

        void DeleteMedicationsFromWorkFlowInstance(Guid contextGuid, List<ReportInstanceMedicationListItem> medications);

        Task<ActivityExecutionStatusEvent> ExecuteActivityAsync(Guid contextGuid, string newStatus, string comments, DateTime? contextDate, string contextCode);

        TerminologyMedDra GetCurrentAdverseReaction(Patient patient);

        bool ValidateExecutionStatusForCurrentActivity(Guid contextGuid, string validateStatus);

        List<ActivityExecutionStatusForPatient> GetExecutionStatusEventsForPatientView(Patient patient);

        List<ActivityExecutionStatusForPatient> GetExecutionStatusEventsForEventView(PatientClinicalEvent clinicalEvent);

        TerminologyMedDra GetTerminologyMedDraForReportInstance(Guid contextGuid);

        void UpdateIdentifiersForWorkFlowInstance(Guid contextGuid, string patientIdentifier, string sourceIdentifier);
    }
}
