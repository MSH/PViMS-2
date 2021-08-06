using MediatR;
using PViMS.Core.Events;
using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.DomainEventHandlers.E2BGenerated
{
    public class GenerateArtefactsWhenE2BGeneratedDomainEventHandler
                            : INotificationHandler<E2BGeneratedDomainEvent>
    {
        private readonly IRepositoryInt<ActivityExecutionStatusEvent> _activityExecutionStatusEventRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IArtefactService _artefactService;
        private readonly IUnitOfWorkInt _unitOfWork;

        public GenerateArtefactsWhenE2BGeneratedDomainEventHandler(
            IRepositoryInt<ActivityExecutionStatusEvent> activityExecutionStatusEventRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IArtefactService artefactService,
            IUnitOfWorkInt unitOfWork)
        {
            _activityExecutionStatusEventRepository = activityExecutionStatusEventRepository ?? throw new ArgumentNullException(nameof(activityExecutionStatusEventRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(E2BGeneratedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            await CreatePatientSummaryAndLinkToExecutionEventAsync(domainEvent.ReportInstance, domainEvent.ActivityExecutionStatusEvent);
            await CreatePatientExtractAndLinkToExecutionEventAsync(domainEvent.ReportInstance, domainEvent.ActivityExecutionStatusEvent);
            await CreateE2BExtractAndLinkToExecutionEventAsync(domainEvent.ReportInstance, domainEvent.ActivityExecutionStatusEvent);
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
                if (clinicalEvent == null)
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
    }
}
