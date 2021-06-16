using Microsoft.AspNetCore.Http;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.Services
{
    public class WorkFlowService : IWorkFlowService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        private readonly IRepositoryInt<Activity> _activityRepository;
        private readonly IRepositoryInt<ActivityExecutionStatusEvent> _activityExecutionStatusEventRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IArtefactService _artefactService { get; set; }
        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }

        public WorkFlowService(IUnitOfWorkInt unitOfWork, 
            ICustomAttributeService attributeService, 
            IPatientService patientService, 
            IArtefactService artefactService, 
            IRepositoryInt<Activity> activityRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<ActivityExecutionStatusEvent> activityExecutionStatusEventRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<WorkFlow> workFlowRepository,
            IRepositoryInt<User> userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _activityExecutionStatusEventRepository = activityExecutionStatusEventRepository ?? throw new ArgumentNullException(nameof(activityExecutionStatusEventRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task AddOrUpdateMedicationsForWorkFlowInstanceAsync(Guid contextGuid, List<ReportInstanceMedicationListItem> medications)
        {
            if(medications == null)
            {
                throw new ArgumentNullException(nameof(medications));
            }
            if (medications.Count == 0) { return; };

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Medications" });
            if (reportInstance == null) {
                return;
            };

            foreach (ReportInstanceMedicationListItem medication in medications)
            {
                if (reportInstance.HasMedication(medication.ReportInstanceMedicationGuid))
                {
                    reportInstance.SetMedicationIdentifier(medication.ReportInstanceMedicationGuid, medication.MedicationIdentifier);
                }
                else
                {
                    reportInstance.AddMedication(medication.MedicationIdentifier, medication.ReportInstanceMedicationGuid);
                }
            }

            _reportInstanceRepository.Update(reportInstance);
            await _unitOfWork.CompleteAsync();
        }

        public async Task CreateWorkFlowInstanceAsync(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier)
        {
            if (String.IsNullOrWhiteSpace(workFlowName))
            {
                throw new ArgumentNullException($"{nameof(workFlowName)} Parameter may not be null");
            }

            // Ensure instance does not exist for this context
            var workFlow = await _workFlowRepository.GetAsync(wf => wf.Description == workFlowName, new string[] { "Activities.ExecutionStatuses" });
            if (workFlow == null)
            {
                throw new KeyNotFoundException($"{nameof(workFlowName)} Unable to locate work flow");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await _userRepository.GetAsync(u => u.UserName == userName);
            if (currentUser == null)
            {
                throw new KeyNotFoundException($"Unable to locate current user");
            }

            var reportInstance = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid);
            if (reportInstance == null)
            {
                reportInstance = new ReportInstance(workFlow, currentUser, contextGuid, patientIdentifier, sourceIdentifier);
                await _reportInstanceRepository.SaveAsync(reportInstance);

                reportInstance.SetSystemIdentifier();

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<ActivityExecutionStatusEvent> ExecuteActivityAsync(Guid contextGuid, string newExecutionStatus, string comments, DateTime? contextDate, string contextCode)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Activities.CurrentStatus", "WorkFlow" });
            if (reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate report instance using contextGuid {contextGuid}");
            }

            var newActivityExecutionStatus = await GetExecutionStatusForActivityAsync(reportInstanceFromRepo.CurrentActivity.QualifiedName, reportInstanceFromRepo.WorkFlowId, newExecutionStatus);
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await _userRepository.GetAsync(u => u.UserName == userName);

            var activityExecutionStatusEvent = reportInstanceFromRepo.ExecuteNewEventForActivity(newActivityExecutionStatus, currentUser, comments, contextDate, contextCode);
            _reportInstanceRepository.Update(reportInstanceFromRepo);

            if (newActivityExecutionStatus.Description == "E2BGENERATED")
            {
                await ProcessE2BGenerationActivityAsync(reportInstanceFromRepo, activityExecutionStatusEvent);
            }

            await _unitOfWork.CompleteAsync();

            return activityExecutionStatusEvent;
        }

        public TerminologyMedDra GetCurrentAdverseReaction(Patient patient)
        {
            foreach (PatientClinicalEvent clinicalEvent in patient.PatientClinicalEvents)
            {
                var term = GetTerminologyMedDraForReportInstance(clinicalEvent.PatientClinicalEventGuid);
                if (term != null) { return term; };
            };

            return null;
        }

        public async Task<bool> ValidateExecutionStatusForCurrentActivityAsync(Guid contextGuid, string executionStatusToBeValidated)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(ri => ri.ContextGuid == contextGuid, new string[] { "Activities" } );
            if(reportInstanceFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate report instance using contextGuid {contextGuid}");
            }    

            var activity = await _activityRepository.GetAsync(a => a.QualifiedName == reportInstanceFromRepo.CurrentActivity.QualifiedName && a.WorkFlow.Id == reportInstanceFromRepo.WorkFlowId, new string[] { "ExecutionStatuses" });
            if (activity == null)
            {
                throw new KeyNotFoundException($"Unable to locate activity using QualifiedName {reportInstanceFromRepo.CurrentActivity.QualifiedName}");
            }

            return activity.ExecutionStatuses.Any(aes => aes.Description == executionStatusToBeValidated);
        }

        public TerminologyMedDra GetTerminologyMedDraForReportInstance(Guid contextGuid)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            
            if(reportInstance != null)
            {
                return reportInstance.TerminologyMedDra;
            }
            return null;
        }

        public void UpdateIdentifiersForWorkFlowInstance(Guid contextGuid, string patientIdentifier, string sourceIdentifier)
        {
            if (String.IsNullOrWhiteSpace(patientIdentifier))
            {
                throw new ArgumentNullException(nameof(patientIdentifier));
            }
            if (String.IsNullOrWhiteSpace(sourceIdentifier))
            {
                throw new ArgumentNullException(nameof(sourceIdentifier));
            }

            var reportInstance = _reportInstanceRepository.Get(ri => ri.ContextGuid == contextGuid, new string[] { "WorkFlow" });
            if(reportInstance == null)
            {
                throw new ArgumentException("reportInstance may not be null");
            }
            if (reportInstance.WorkFlow == null)
            {
                throw new ArgumentException("reportInstance work flow may not be null");
            }

            reportInstance.SetEventIdentifiers(patientIdentifier, sourceIdentifier);

            _reportInstanceRepository.Update(reportInstance);
        }

        private async Task ProcessE2BGenerationActivityAsync(ReportInstance reportInstanceFromRepo, ActivityExecutionStatusEvent activityExecutionStatusEvent)
        {
            await CreatePatientSummaryAndLinkToExecutionEventAsync(reportInstanceFromRepo, activityExecutionStatusEvent);
            await CreatePatientExtractAndLinkToExecutionEventAsync(reportInstanceFromRepo, activityExecutionStatusEvent);
            await CreateE2BExtractAndLinkToExecutionEventAsync(reportInstanceFromRepo, activityExecutionStatusEvent);
        }

        private async Task CreatePatientSummaryAndLinkToExecutionEventAsync(ReportInstance reportInstance, ActivityExecutionStatusEvent executionEvent)
        {
            if (reportInstance == null)
            {
                throw new ArgumentNullException(nameof(reportInstance));
            }

            if (executionEvent == null)
            {
                throw new ArgumentNullException(nameof(executionEvent));
            }

            var artefactModel = reportInstance.WorkFlow.Description == "New Active Surveilliance Report" ? 
                await _artefactService.CreatePatientSummaryForActiveReportAsync(reportInstance.ContextGuid) : 
                await _artefactService.CreatePatientSummaryForSpontaneousReportAsync(reportInstance.ContextGuid);

            using (var tempFile = File.OpenRead(artefactModel.FullPath))
            {
                if (tempFile.Length > 0)
                {
                    BinaryReader rdr = new BinaryReader(tempFile);
                    executionEvent.AddAttachment(Path.GetFileName(artefactModel.FileName),
                        _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "docx"),
                        tempFile.Length,
                        rdr.ReadBytes((int)tempFile.Length), 
                        "PatientSummary");

                    _activityExecutionStatusEventRepository.Update(executionEvent);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        private async Task CreatePatientExtractAndLinkToExecutionEventAsync(ReportInstance reportInstance, ActivityExecutionStatusEvent executionEvent)
        {
            if (reportInstance == null)
            {
                throw new ArgumentNullException(nameof(reportInstance));
            }

            if (executionEvent == null)
            {
                throw new ArgumentNullException(nameof(executionEvent));
            }

            ArtefactInfoModel artefactModel = null;
            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                var clinicalEvent = await _patientClinicalEventRepository.GetAsync(pce => pce.PatientClinicalEventGuid == reportInstance.ContextGuid);
                if(clinicalEvent == null)
                {
                    throw new KeyNotFoundException(nameof(clinicalEvent));
                }

                artefactModel = _artefactService.CreateActiveDatasetForDownload(new long[] { clinicalEvent.Patient.Id }, 0);
            }
            else
            {
                var sourceInstance = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                if (sourceInstance == null)
                {
                    throw new KeyNotFoundException(nameof(sourceInstance));
                }

                artefactModel = await _artefactService.CreateDatasetInstanceForDownloadAsync(sourceInstance.Id);
            }

            using (var tempFile = File.OpenRead(artefactModel.FullPath))
            {
                if (tempFile.Length > 0)
                {
                    BinaryReader rdr = new BinaryReader(tempFile);
                    executionEvent.AddAttachment(Path.GetFileName(artefactModel.FileName),
                        _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xlsx"),
                        tempFile.Length,
                        rdr.ReadBytes((int)tempFile.Length),
                        "PatientExtract");

                    _activityExecutionStatusEventRepository.Update(executionEvent);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        private async Task CreateE2BExtractAndLinkToExecutionEventAsync(ReportInstance reportInstance, ActivityExecutionStatusEvent executionEvent)
        {
            if (reportInstance == null)
            {
                throw new ArgumentNullException(nameof(reportInstance));
            }

            if (executionEvent == null)
            {
                throw new ArgumentNullException(nameof(executionEvent));
            }

            var activityInstance = reportInstance.CurrentActivity;

            var evt = activityInstance.ExecutionEvents.OrderByDescending(ee => ee.EventDateTime).First(ee => ee.ExecutionStatus.Description == "E2BINITIATED");
            var tag = (reportInstance.WorkFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

            var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.Tag == tag && di.ContextId == evt.Id);
            var artefactModel = await _artefactService.CreateE2BAsync(datasetInstance.Id);

            using (var tempFile = File.OpenRead(artefactModel.FullPath))
            {
                if (tempFile.Length > 0)
                {
                    BinaryReader rdr = new BinaryReader(tempFile);
                    executionEvent.AddAttachment(Path.GetFileName(artefactModel.FileName),
                        _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xml"),
                        tempFile.Length,
                        rdr.ReadBytes((int)tempFile.Length),
                        "E2b");

                    _activityExecutionStatusEventRepository.Update(executionEvent);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        private async Task<ActivityExecutionStatus> GetExecutionStatusForActivityAsync(string qualifiedName, int workFlowId, string newExecutionStatus)
        {
            var activity = await _activityRepository.GetAsync(a => a.QualifiedName == qualifiedName && a.WorkFlow.Id == workFlowId, new string[] { "ExecutionStatuses" });
            if (activity == null)
            {
                throw new KeyNotFoundException($"Unable to locate activity using QualifiedName {qualifiedName}");
            }

            return activity.ExecutionStatuses.Single(aes => aes.Description == newExecutionStatus);
        }
    }
}
