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

        Task CreateWorkFlowInstanceAsync(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier);

        Task<ActivityExecutionStatusEvent> ExecuteActivityAsync(Guid contextGuid, string newStatus, string comments, DateTime? contextDate, string contextCode);

        TerminologyMedDra GetCurrentAdverseReaction(Patient patient);

        bool ValidateExecutionStatusForCurrentActivity(Guid contextGuid, string validateStatus);

        TerminologyMedDra GetTerminologyMedDraForReportInstance(Guid contextGuid);

        void UpdateIdentifiersForWorkFlowInstance(Guid contextGuid, string patientIdentifier, string sourceIdentifier);
    }
}
