using MediatR;
using PViMS.Core.Events;
using PVIMS.API.Infrastructure.Services;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.E2BGenerated
{
    public class GeneratePatientExtractArtefactWhenE2BGeneratedDomainEventHandler
                            : INotificationHandler<E2BGeneratedDomainEvent>
    {
        private readonly IRepositoryInt<ActivityExecutionStatusEvent> _activityExecutionStatusEventRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IExcelDocumentService _excelDocumentService;
        private readonly IUnitOfWorkInt _unitOfWork;

        public GeneratePatientExtractArtefactWhenE2BGeneratedDomainEventHandler(
            IRepositoryInt<ActivityExecutionStatusEvent> activityExecutionStatusEventRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IExcelDocumentService excelDocumentService,
            IUnitOfWorkInt unitOfWork)
        {
            _activityExecutionStatusEventRepository = activityExecutionStatusEventRepository ?? throw new ArgumentNullException(nameof(activityExecutionStatusEventRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _excelDocumentService = excelDocumentService ?? throw new ArgumentNullException(nameof(excelDocumentService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(E2BGeneratedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var executionEvent = domainEvent.ReportInstance.CurrentActivity.GetLatestEvent();
            //await CreatePatientExtractAndLinkToExecutionEventAsync(domainEvent.ReportInstance, executionEvent);
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

            var artefactModel = reportInstance.WorkFlow.Description == "New Active Surveilliance Report" ?
                await CreatePatientExtractForActiveReportAsync(reportInstance.ContextGuid) :
                await CreatePatientExtractForSpontaneousReportAsync(reportInstance.ContextGuid);

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
                }
            }
        }

        private async Task<ArtefactInfoModel> CreatePatientExtractForActiveReportAsync(Guid contextGuid)
        {
            var patientClinicalEvent = await GetPatientClinicalEventAsync(contextGuid);
            return _excelDocumentService.CreateActiveDatasetForDownload(new long[] { patientClinicalEvent.Patient.Id }, 0);
        }

        private async Task<ArtefactInfoModel> CreatePatientExtractForSpontaneousReportAsync(Guid contextGuid)
        {
            var datasetInstance = await GetDatasetInstanceAsync(contextGuid);
            return await _excelDocumentService.CreateDatasetInstanceForDownloadAsync(datasetInstance.Id);
        }

        private async Task<PatientClinicalEvent> GetPatientClinicalEventAsync(Guid contextGuid)
        {
            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(pce => pce.PatientClinicalEventGuid == contextGuid,
                new string[] { "SourceTerminologyMedDra",
                    "Patient.PatientFacilities.Facility",
                    "Patient.PatientConditions.TerminologyMedDra", "Patient.PatientConditions.Outcome", "Patient.PatientConditions.TreatmentOutcome",
                    "Patient.PatientMedications.Concept.MedicationForm", "Patient.PatientMedications.Product",
                    "Patient.PatientLabTests.LabTest", "Patient.PatientLabTests.TestUnit",
                    "Patient.PatientFacilities.Facility"});
            if (patientClinicalEvent == null)
            {
                throw new KeyNotFoundException(nameof(patientClinicalEvent));
            }

            return patientClinicalEvent;
        }

        private async Task<DatasetInstance> GetDatasetInstanceAsync(Guid contextGuid)
        {
            var datasetInstance = await _datasetInstanceRepository.GetAsync(di => di.DatasetInstanceGuid == contextGuid, new string[] { 
                "DatasetInstanceValues.DatasetElement", 
                "DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub" });
            if (datasetInstance == null)
            {
                throw new KeyNotFoundException(nameof(datasetInstance));
            }

            return datasetInstance;
        }
    }
}
