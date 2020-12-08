using PVIMS.Core;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PVIMS.Services
{
    public class WorkFlowService : IWorkFlowService 
    {
        private readonly IUnitOfWorkInt _unitOfWork;

        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;

        public IArtefactService _artefactService { get; set; }
        public ICustomAttributeService _attributeService { get; set; }
        public IPatientService _patientService { get; set; }
        public UserContext _userContext { get; set; }

        public WorkFlowService(IUnitOfWorkInt unitOfWork, 
            ICustomAttributeService attributeService, 
            IPatientService patientService, 
            IArtefactService artefactService, 
            UserContext userContext,
            IRepositoryInt<ReportInstance> reportInstanceRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _attributeService = attributeService ?? throw new ArgumentNullException(nameof(attributeService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
        }

        public void AddOrUpdateMedicationsForWorkFlowInstance(Guid contextGuid, List<ReportInstanceMedicationListItem> medications)
        {
            if(medications == null)
            {
                throw new ArgumentNullException(nameof(medications));
            }
            if (medications.Count == 0) { return; };

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>()
                .Queryable()
                .SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            if (reportInstance == null) {
                throw new ArgumentException("contextGuid may not be null");
            };

            // Full managements of medications list for report instance
            ArrayList addCollection = new ArrayList();
            ArrayList modifyCollection = new ArrayList();
            foreach (ReportInstanceMedicationListItem medication in medications)
            {
                if(reportInstance.Medications != null)
                {
                    var exists = reportInstance.Medications.Any(m => m.ReportInstanceMedicationGuid == medication.ReportInstanceMedicationGuid);
                    if (exists)
                    {
                        modifyCollection.Add(medication);
                    }
                    else
                    {
                        addCollection.Add(medication);
                    }
                }
                else
                {
                    addCollection.Add(medication);
                }
            }

            foreach (ReportInstanceMedicationListItem medication in addCollection)
            {
                var med = new ReportInstanceMedication() { MedicationIdentifier = medication.MedicationIdentifier, ReportInstance = reportInstance, ReportInstanceMedicationGuid = medication.ReportInstanceMedicationGuid };
                reportInstance.Medications.Add(med);

                _unitOfWork.Repository<ReportInstanceMedication>().Save(med);
            }
            foreach (ReportInstanceMedicationListItem medication in modifyCollection)
            {
                var med = reportInstance.Medications.Single(m => m.ReportInstanceMedicationGuid == medication.ReportInstanceMedicationGuid);
                med.MedicationIdentifier = medication.MedicationIdentifier;

                _unitOfWork.Repository<ReportInstanceMedication>().Update(med);
            }
        }

        public void CreateWorkFlowInstance(string workFlowName, Guid contextGuid, string patientIdentifier, string sourceIdentifier)
        {
            if (String.IsNullOrWhiteSpace(workFlowName))
            {
                throw new ArgumentNullException(nameof(workFlowName));
            }

            // Ensure instance does not exist for this context
            var workFlow = _unitOfWork.Repository<WorkFlow>().Queryable().SingleOrDefault(wf => wf.Description == workFlowName);
            if (workFlow == null)
            {
                throw new ArgumentException("Unable to locate work flow");
            }

            User currentUser = _unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == _userContext.UserName);

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().SingleOrDefault(ri => ri.ContextGuid == contextGuid);
            if(reportInstance == null)
            {
                reportInstance = new ReportInstance(workFlow, currentUser)
                {
                    ContextGuid = contextGuid,
                    PatientIdentifier = patientIdentifier,
                    SourceIdentifier = sourceIdentifier
                };
                _unitOfWork.Repository<ReportInstance>().Save(reportInstance);

                reportInstance.SetIdentifier();

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
                _unitOfWork.Complete();
            }
        }

        public int CheckWorkFlowInstanceCount(string workFlowName)
        {
            var config = _unitOfWork.Repository<Config>().Queryable().Where(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount).SingleOrDefault();
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    var alertCount = Convert.ToInt32(config.ConfigValue);

                    // How many instances within the last alertcount
                    var compDate = DateTime.Now.AddDays(alertCount * -1);
                    return _unitOfWork.Repository<ReportInstance>().Queryable().Where(rp => rp.WorkFlow.Description == workFlowName && rp.Created >= compDate && rp.Finished == null).Count();
                }
            }
            return 0;
        }

        public void DeleteMedicationsFromWorkFlowInstance(Guid contextGuid, List<ReportInstanceMedicationListItem> medications)
        {
            if (medications == null)
            {
                throw new ArgumentNullException(nameof(medications));
            }
            if (medications.Count == 0) { return; };

            ReportInstance reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == contextGuid);
            if (reportInstance == null)
            {
                throw new ArgumentException("contextGuid may not be null");
            };

            // Full managements of medications list for report instance
            ArrayList deleteCollection = new ArrayList();
            foreach (ReportInstanceMedication medication in reportInstance.Medications)
            {
                var exists = medications.Any(m => m.ReportInstanceMedicationGuid == medication.ReportInstanceMedicationGuid);
                if (exists) { deleteCollection.Add(medication); };
            }

            foreach (ReportInstanceMedication medication in deleteCollection)
            {
                reportInstance.Medications.Remove(medication);
                _unitOfWork.Repository<ReportInstanceMedication>().Delete(medication);
            }

            _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
        }

        public ActivityExecutionStatusEvent ExecuteActivity(Guid contextGuid, string newStatus, string comments, DateTime? contextDate, string contextCode)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == contextGuid);
            var activityInstance = reportInstance.CurrentActivity;
            var newExecutionStatus = GetExecutionStatusForActivity(reportInstance, newStatus);
            var currentUser = _unitOfWork.Repository<User>().Queryable().SingleOrDefault(u => u.UserName == _userContext.UserName);

            var newEvent = activityInstance.AddNewEvent(newExecutionStatus, currentUser, comments, contextDate, contextCode);

            _unitOfWork.Repository<ActivityInstance>().Update(activityInstance);

            if (activityInstance.CurrentStatus.Description == "E2BGENERATED")
            {
                CreatePatientSummaryAndLink(reportInstance, newEvent);
                CreatePatientExtractAndLink(reportInstance, newEvent);
                CreateE2BExtractAndLink(reportInstance, newEvent);
            }

            if (activityInstance.CurrentStatus.Description == "CONFIRMED")
            {
                activityInstance.Current = false;
                _unitOfWork.Repository<ActivityInstance>().Update(activityInstance);

                var newActivity = _unitOfWork.Repository<Activity>()
                    .Queryable()
                    .Single(a => a.WorkFlow.Id == reportInstance.WorkFlow.Id && a.QualifiedName == "Set MedDRA and Causality");
                reportInstance.SetNewActivity(newActivity, currentUser);

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
            }

            if (activityInstance.CurrentStatus.Description == "CAUSALITYSET")
            {
                activityInstance.Current = false;
                _unitOfWork.Repository<ActivityInstance>().Update(activityInstance);

                var newActivity = _unitOfWork.Repository<Activity>()
                    .Queryable()
                    .Single(a => a.WorkFlow.Id == reportInstance.WorkFlow.Id && a.QualifiedName == "Extract E2B");
                reportInstance.SetNewActivity(newActivity, currentUser);

                _unitOfWork.Repository<ReportInstance>().Update(reportInstance);
            }

            _unitOfWork.Complete();

            return newEvent;
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

        public bool ValidateExecutionStatusForCurrentActivity(Guid contextGuid, string validateStatus)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>()
                .Queryable()
                .Single(ri => ri.ContextGuid == contextGuid);

            var activityInstance = reportInstance.Activities
                .Single(a => a.Current == true);

            var activity = _unitOfWork.Repository<Activity>()
                .Queryable()
                .Single(a => a.QualifiedName == activityInstance.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id);

            return _unitOfWork.Repository<ActivityExecutionStatus>()
                .Queryable()
                .Any(aes => aes.Activity.Id == activity.Id && aes.Description == validateStatus);
        }

        public List<ActivityExecutionStatusForPatient> GetExecutionStatusEventsForPatientView(Patient patient)
        {
            var clinicalEvents =
                _unitOfWork.Repository<PatientClinicalEvent>()
                    .Queryable()
                    .Where(pce => pce.Patient.Id == patient.Id && pce.Archived == false);

            List<ActivityExecutionStatusForPatient> results = new List<ActivityExecutionStatusForPatient>();

            foreach (var clinicalEvent in clinicalEvents)
            {
                var reportInstance = _unitOfWork.Repository<ReportInstance>()
                    .Queryable()
                    .SingleOrDefault(ri => ri.ContextGuid == clinicalEvent.PatientClinicalEventGuid);
                if(reportInstance != null)
                {
                    var result = new ActivityExecutionStatusForPatient();
                    result.PatientClinicalEvent = clinicalEvent;

                    var items = _unitOfWork.Repository<ActivityExecutionStatusEvent>()
                        .Queryable()
                        .Where(aese => aese.ActivityInstance.ReportInstance.Id == reportInstance.Id)
                        .OrderByDescending(aese => aese.EventDateTime)
                        .Take(1);
                    foreach (ActivityExecutionStatusEvent item in items)
                    {
                        var activityItem = new ActivityExecutionStatusForPatient.ActivityExecutionStatusInfo()
                        {
                            Activity = item.ActivityInstance?.QualifiedName,
                            ExecutedDate = item.EventDateTime.ToString(),
                            ExecutedBy = item.EventCreatedBy?.FullName,
                            ExecutionStatus = item.ExecutionStatus?.FriendlyDescription,
                            Comments = item.Comments
                        };
                        result.ActivityItems.Add(activityItem);
                    };

                    results.Add(result);
                }
            }

            return results;
        }

        public List<ActivityExecutionStatusForPatient> GetExecutionStatusEventsForEventView(PatientClinicalEvent clinicalEvent)
        {
            List<ActivityExecutionStatusForPatient> results = new List<ActivityExecutionStatusForPatient>();

            var reportInstance = _unitOfWork.Repository<ReportInstance>()
                .Queryable()
                .SingleOrDefault(ri => ri.ContextGuid == clinicalEvent.PatientClinicalEventGuid);
            if (reportInstance != null)
            {
                var result = new ActivityExecutionStatusForPatient();
                result.PatientClinicalEvent = clinicalEvent;

                var items = _unitOfWork.Repository<ActivityExecutionStatusEvent>()
                    .Queryable()
                    .Where(aese => aese.ActivityInstance.ReportInstance.Id == reportInstance.Id)
                    .OrderByDescending(aese => aese.EventDateTime);
                foreach (ActivityExecutionStatusEvent item in items)
                {
                    var activityItem = new ActivityExecutionStatusForPatient.ActivityExecutionStatusInfo()
                    {
                        Activity = item.ActivityInstance?.QualifiedName,
                        ExecutedDate = item.EventDateTime.ToString(),
                        ExecutedBy = item.EventCreatedBy?.FullName,
                        ExecutionStatus = item.ExecutionStatus?.FriendlyDescription,
                        Comments = item.Comments
                    };
                    result.ActivityItems.Add(activityItem);
                };

                results.Add(result);
            }

            return results;
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

            reportInstance.PatientIdentifier = patientIdentifier;
            reportInstance.SourceIdentifier = sourceIdentifier;

            _reportInstanceRepository.Update(reportInstance);
        }

        private void CreatePatientSummaryAndLink(ReportInstance reportInstance, ActivityExecutionStatusEvent newEvent)
        {
            var model = reportInstance.WorkFlow.Description == "New Active Surveilliance Report" ? _artefactService.CreatePatientSummaryForActiveReport(reportInstance.ContextGuid) : _artefactService.CreatePatientSummaryForSpontaneousReport(reportInstance.ContextGuid);

            // Create patient summary and link to event
            Attachment att;
            AttachmentType attType = _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "docx");
            FileStream tempFile = File.OpenRead(model.FullPath);

            if (tempFile.Length > 0)
            {
                BinaryReader rdr = new BinaryReader(tempFile);
                byte[] buffer = rdr.ReadBytes((int)tempFile.Length);

                // Create the attachment
                att = new Attachment
                {
                    ActivityExecutionStatusEvent = newEvent,
                    Description = "PatientSummary",
                    FileName = Path.GetFileName(model.FileName),
                    AttachmentType = attType,
                    Size = tempFile.Length,
                    Content = buffer
                };
                newEvent.Attachments.Add(att);

                _unitOfWork.Repository<Attachment>().Save(att);
            }
            tempFile.Close();
            tempFile = null;
        }

        private void CreatePatientExtractAndLink(ReportInstance reportInstance, ActivityExecutionStatusEvent newEvent)
        {
            ArtefactInfoModel path = null;
            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                var clinicalEvt = _unitOfWork.Repository<PatientClinicalEvent>().Queryable().Single(pce => pce.PatientClinicalEventGuid == reportInstance.ContextGuid);
                path = _artefactService.CreateActiveDatasetForDownload(new long[] { clinicalEvt.Patient.Id }, 0);
            }
            else
            {
                var sourceInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().Single(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                path = _artefactService.CreateDatasetInstanceForDownload(sourceInstance.Id);
            }

            // Create patient summary and link to event
            Attachment att;
            AttachmentType attType = _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xlsx");
            FileStream tempFile = File.OpenRead(path.FullPath);

            if (tempFile.Length > 0)
            {
                BinaryReader rdr = new BinaryReader(tempFile);
                byte[] buffer = rdr.ReadBytes((int)tempFile.Length);

                // Create the attachment
                att = new Attachment
                {
                    ActivityExecutionStatusEvent = newEvent,
                    Description = "PatientExtract",
                    FileName = Path.GetFileName(path.FileName),
                    AttachmentType = attType,
                    Size = tempFile.Length,
                    Content = buffer
                };
                newEvent.Attachments.Add(att);

                _unitOfWork.Repository<Attachment>().Save(att);
            }
            tempFile.Close();
            tempFile = null;
        }

        private void CreateE2BExtractAndLink(ReportInstance reportInstance, ActivityExecutionStatusEvent newEvent)
        {
            ArtefactInfoModel path = null;

            var activityInstance = reportInstance.CurrentActivity;

            DatasetInstance datasetInstance = null;
            var evt = activityInstance.ExecutionEvents.OrderByDescending(ee => ee.EventDateTime).First(ee => ee.ExecutionStatus.Description == "E2BINITIATED");
            var tag = (reportInstance.WorkFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

            datasetInstance
                = _unitOfWork.Repository<DatasetInstance>()
                .Queryable()
                .Where(di => di.Tag == tag
                    && di.ContextID == evt.Id).SingleOrDefault();

            path = _artefactService.CreateE2B(datasetInstance.Id);

            Attachment att;
            AttachmentType attType = _unitOfWork.Repository<AttachmentType>().Queryable().Single(at => at.Key == "xml");
            FileStream tempFile = File.OpenRead(path.FullPath);

            if (tempFile.Length > 0)
            {
                BinaryReader rdr = new BinaryReader(tempFile);
                byte[] buffer = rdr.ReadBytes((int)tempFile.Length);

                // Create the attachment
                att = new Attachment
                {
                    ActivityExecutionStatusEvent = newEvent,
                    Description = "E2b",
                    FileName = Path.GetFileName(path.FileName),
                    AttachmentType = attType,
                    Size = tempFile.Length,
                    Content = buffer
                };
                newEvent.Attachments.Add(att);

                _unitOfWork.Repository<Attachment>().Save(att);
            }
            tempFile.Close();
            tempFile = null;
        }

        private ActivityExecutionStatus GetExecutionStatusForActivity(ReportInstance reportInstance, string getStatus)
        {
            var activityInstance = reportInstance.Activities
                .Single(a => a.Current == true);

            var activity = _unitOfWork.Repository<Activity>()
                .Queryable()
                .Single(a => a.QualifiedName == activityInstance.QualifiedName && a.WorkFlow.Id == reportInstance.WorkFlow.Id);

            return _unitOfWork.Repository<ActivityExecutionStatus>()
                .Queryable()
                .Single(aes => aes.Activity.Id == activity.Id && aes.Description == getStatus);
        }
    }
}
