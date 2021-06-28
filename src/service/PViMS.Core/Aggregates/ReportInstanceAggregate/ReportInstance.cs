using PViMS.Core.Events;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.ReportInstanceAggregate
{
    public class ReportInstance 
        : AuditedEntityBase, IAggregateRoot
    {
        public Guid ReportInstanceGuid { get; private set; }
        public DateTime? FinishedDate { get; private set; }
        
        public int WorkFlowId { get; private set; }
        public virtual WorkFlow WorkFlow { get; set; }

        public Guid ContextGuid { get; private set; }
        public string Identifier { get; private set; }
        public string PatientIdentifier { get; private set; }

        public int? TerminologyMedDraId { get; private set; }
        public virtual TerminologyMedDra TerminologyMedDra { get; set; }

        public string SourceIdentifier { get; private set; }

        private List<ActivityInstance> _activities;
        public IEnumerable<ActivityInstance> Activities => _activities.AsReadOnly();

        private List<ReportInstanceMedication> _medications;
        public IEnumerable<ReportInstanceMedication> Medications => _medications.AsReadOnly();

        private List<ReportInstanceTask> _tasks;
        public IEnumerable<ReportInstanceTask> Tasks => _tasks.AsReadOnly();

        protected ReportInstance()
        {
            _activities = new List<ActivityInstance>();
            _medications = new List<ReportInstanceMedication>();
            _tasks = new List<ReportInstanceTask>();
        }

        public ReportInstance(WorkFlow workFlow, User currentUser, Guid contextGuid, string patientIdentifier, string sourceIdentifier)
		{
            _activities = new List<ActivityInstance>();
            _medications = new List<ReportInstanceMedication>();
            _tasks = new List<ReportInstanceTask>();

            ReportInstanceGuid = Guid.NewGuid();
            Identifier = "TBD";

            ContextGuid = contextGuid;
            PatientIdentifier = patientIdentifier;
            SourceIdentifier = sourceIdentifier;

            WorkFlowId = workFlow.Id;
            WorkFlow = workFlow;

            InitialiseWithFirstActivity(workFlow, currentUser);
        }

        public ActivityExecutionStatusEvent ExecuteNewEventForActivity(ActivityExecutionStatus newExecutionStatus, User currentUser, string comments, DateTime? contextDate, string contextCode)
        {
            if (CurrentActivity.CurrentStatus.Description == newExecutionStatus.Description) { return null; };

            var activityExecutionStatusEvent = CurrentActivity.ExecuteEvent(newExecutionStatus, currentUser, comments, contextDate, contextCode);

            switch (newExecutionStatus.Description)
            {
                case "CONFIRMED":
                    CheckIfAbleToChangeFromConfirmationActivity();
                    CurrentActivity.SetToOld();
                    MoveToNextActivity(WorkFlow, currentUser);
                    break;

                case "CAUSALITYSET":
                    CurrentActivity.SetToOld();
                    MoveToNextActivity(WorkFlow, currentUser);
                    break;

                default:
                    break;
            }

            return activityExecutionStatusEvent;
        }

        public void AddMedication(string medicationIdentifier, Guid reportInstanceMedicationGuid)
        {
            var medication = new ReportInstanceMedication(medicationIdentifier, "", "", reportInstanceMedicationGuid);
            _medications.Add(medication);
        }

        public ReportInstanceTask AddTask(string source, string description, TaskType taskType)
        {
            var taskDetail = new TaskDetail(source, description);
            var taskStatus = TaskStatus.New;

            var newTask = new ReportInstanceTask(taskDetail, taskType, taskStatus);
            _tasks.Add(newTask);

            AddTaskAddedDomainEvent(newTask);

            return newTask;
        }

        public ReportInstanceTaskComment AddTaskComment(int taskId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new DomainException("A blank comment may not be added to the task");
            }
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if(task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }

            return task.AddComment(comment);
        }

        public void ChangeTaskDetails(int taskId, string source, string description)
        {
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }

            var taskDetail = new TaskDetail(source, description);
            task.ChangeDetails(taskDetail);
        }

        public void ChangeTaskStatusToUnderInvestigation(int taskId)
        {
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }

            if (task.TaskStatusId == TaskStatus.Cancelled.Id ||
                task.TaskStatusId == TaskStatus.Completed.Id)
            {
                throw new DomainException("Cannot change status of task that is cancelled or completed");
            }

            task.ChangeTaskStatusToUnderInvestigation();
        }

        public void ChangeTaskStatusToOnHold(int taskId)
        {
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }

            if (task.TaskStatusId == TaskStatus.Cancelled.Id ||
                task.TaskStatusId == TaskStatus.Completed.Id)
            {
                throw new DomainException("Cannot change status of task that is cancelled or completed");
            }

            task.ChangeTaskStatusToOnHold();
        }

        public void ChangeTaskStatusToCompleted(int taskId)
        {
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }

            if (task.TaskStatusId == TaskStatus.Cancelled.Id ||
                task.TaskStatusId == TaskStatus.Completed.Id)
            {
                throw new DomainException("Cannot change status of task that is cancelled or completed");
            }

            task.ChangeTaskStatusToCompleted();
        }

        public void ChangeTaskStatusToCancelled(int taskId)
        {
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }

            if (task.TaskStatusId == TaskStatus.Cancelled.Id ||
                task.TaskStatusId == TaskStatus.Completed.Id)
            {
                throw new DomainException("Cannot change status of task that is cancelled or completed");
            }

            task.ChangeTaskStatusToCancelled();
            AddTaskCancelledDomainEvent(task);
        }

        public void DeleteTaskComment(int taskId, int taskCommentId)
        {
            var task = _tasks.SingleOrDefault(t => t.Id == taskId);
            if (task == null)
            {
                throw new KeyNotFoundException(nameof(taskId));
            }
            task.DeleteComment(taskCommentId);
        }

        public bool HasActiveTasks()
        {
            return _tasks.Any(t => t.TaskStatusId != TaskStatus.Completed.Id && t.TaskStatusId != TaskStatus.Cancelled.Id);
        }

        public bool HasMedication(Guid reportInstanceMedicationGuid)
        {
            return _medications.Any(m => m.ReportInstanceMedicationGuid == reportInstanceMedicationGuid);
        }

        public void SetEventIdentifiers(string patientIdentifier, string sourceIdentifier)
        {
            PatientIdentifier = patientIdentifier;
            SourceIdentifier = sourceIdentifier;
        }

        public void SetMedicationIdentifier(Guid reportInstanceMedicationGuid, string medicationIdentifier)
        {
            var medication = _medications.SingleOrDefault(m => m.ReportInstanceMedicationGuid == reportInstanceMedicationGuid);
            if (medication == null)
            {
                throw new KeyNotFoundException(nameof(reportInstanceMedicationGuid));
            }

            medication.ChangeMedicationIdentifier(medicationIdentifier);
        }

        public void SetNaranjoCausality(Guid reportInstanceMedicationGuid, string naranjoCausality)
        {
            var medication = _medications.SingleOrDefault(m => m.ReportInstanceMedicationGuid == reportInstanceMedicationGuid);
            if (medication == null)
            {
                throw new KeyNotFoundException(nameof(reportInstanceMedicationGuid));
            }

            medication.SetNaranjoCausality(naranjoCausality);
        }

        public void SetSystemIdentifier()
        {
            if (!string.IsNullOrWhiteSpace(Identifier))
            {
                Identifier = $"{WorkFlowId}/{Created.Year.ToString("D4")}/{Id.ToString("D5")}";
            }
        }

        public void SetWhoCausality(Guid reportInstanceMedicationGuid, string whoCausality)
        {
            var medication = _medications.SingleOrDefault(m => m.ReportInstanceMedicationGuid == reportInstanceMedicationGuid);
            if (medication == null)
            {
                throw new KeyNotFoundException(nameof(reportInstanceMedicationGuid));
            }

            medication.SetWhoCausality(whoCausality);
        }

        public ActivityInstance CurrentActivity
        {
            get
            {
                return Activities.Single(a => a.Current == true);
            }
        }

        public string DisplayStatus
        {
            get
            {
                return $"{CurrentActivity.QualifiedName} | {CurrentActivity.CurrentStatus.Description}";
            }
        }

        private void InitialiseWithFirstActivity(WorkFlow workFlow, User currentUser)
        {
            if(workFlow.Activities.Count == 0)
            {
                throw new DomainException($"Workflow {workFlow.Description} does not have any activities configured");
            }

            var firstActivity = workFlow.Activities.OrderBy(a => a.Id).First();
            var firstActivityInstance = new ActivityInstance(firstActivity, currentUser);
            _activities.Add(firstActivityInstance);
        }

        private void MoveToNextActivity(WorkFlow workFlow, User currentUser)
        {
            if (workFlow.Activities.Count == 0)
            {
                throw new DomainException($"Workflow {workFlow.Description} does not have any activities configured");
            }

            if (CurrentActivity.QualifiedName == "Confirm Report Data")
            {
                var activity = workFlow.Activities.Single(a => a.QualifiedName == "Set MedDRA and Causality");
                var activityInstance = new ActivityInstance(activity, currentUser);
                _activities.Add(activityInstance);
            }

            if (CurrentActivity.QualifiedName == "Set MedDRA and Causality")
            {
                var activity = workFlow.Activities.Single(a => a.QualifiedName == "Extract E2B");
                var activityInstance = new ActivityInstance(activity, currentUser);
                _activities.Add(activityInstance);
            }
        }

        private void CheckIfAbleToChangeFromConfirmationActivity()
        {
            if (HasActiveTasks())
            {
                throw new DomainException($"Unable to change activity from CONFIRMED as there are active tasks");
            }
        }

        private void AddTaskAddedDomainEvent(ReportInstanceTask newTask)
        {
            var domainEvent = new TaskAddedDomainEvent(newTask);

            this.AddDomainEvent(domainEvent);
        }

        private void AddTaskCancelledDomainEvent(ReportInstanceTask cancelledTask)
        {
            var domainEvent = new TaskCancelledDomainEvent(cancelledTask);

            this.AddDomainEvent(domainEvent);
        }
    }
}