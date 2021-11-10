using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;

namespace PVIMS.Core.Services
{
    public interface IWorkFlowService
    {
        Task AddOrUpdateMedicationsForWorkFlowInstanceAsync(Guid contextGuid, List<ReportInstanceMedicationListItem> medications);

        Task CreateWorkFlowInstanceAsync(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier, string facilityIdentifier);

        Task<ActivityExecutionStatusEvent> ExecuteActivityAsync(Guid contextGuid, string newExecutionStatus, string comments, DateTime? contextDate, string contextCode);

        TerminologyMedDra GetCurrentAdverseReaction(Patient patient);

        Task<bool> ValidateExecutionStatusForCurrentActivityAsync(Guid contextGuid, string executionStatusToBeValidated);

        TerminologyMedDra GetTerminologyMedDraForReportInstance(Guid contextGuid);

        Task UpdatePatientIdentifierForReportInstanceAsync(Guid contextGuid, string patientIdentifier);

        Task UpdateSourceIdentifierForReportInstanceAsync(Guid contextGuid, string sourceIdentifier);
    }
}
