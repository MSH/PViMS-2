using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ChangeMedicationDetailsCommandHandler
        : IRequestHandler<ChangeMedicationDetailsCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly ILogger<ChangeMedicationDetailsCommandHandler> _logger;

        public ChangeMedicationDetailsCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            ILogger<ChangeMedicationDetailsCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeMedicationDetailsCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "PatientClinicalEvents", "PatientMedications.Concept.MedicationForm", "PatientMedications.Product" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            var medicationDetail = await PrepareMedicationDetailAsync(message.Attributes);
            if (!medicationDetail.IsValid())
            {
                medicationDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            patientFromRepo.ChangeMedicationDetails(message.PatientMedicationId, message.StartDate, message.EndDate, message.Dose, message.DoseFrequency, message.DoseUnit);
            _modelExtensionBuilder.UpdateExtendable(patientFromRepo.PatientMedications.Single(pm => pm.Id == message.PatientMedicationId), medicationDetail.CustomAttributes, "Admin");

            _patientRepository.Update(patientFromRepo);

            await AddOrUpdateMedicationsOnReportInstanceAsync(patientFromRepo, message.StartDate, message.EndDate, patientFromRepo.PatientMedications.Single(pm => pm.Id == message.PatientMedicationId).DisplayName, patientFromRepo.PatientMedications.Single(pm => pm.Id == message.PatientMedicationId).PatientMedicationGuid);

            _logger.LogInformation($"----- Medication {message.PatientMedicationId} details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<MedicationDetail> PrepareMedicationDetailAsync(IDictionary<int, string> attributes)
        {
            var medicationDetail = new MedicationDetail();
            medicationDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientMedication>();

            //medicationDetail = _mapper.Map<MedicationDetail>(medicationForUpdate);
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = medicationDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient medication {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value.Trim();
            }

            return medicationDetail;
        }

        private async Task AddOrUpdateMedicationsOnReportInstanceAsync(Patient patientFromRepo, DateTime medicationStartDate, DateTime? medicationEndDate, string medicationDisplayName, Guid medicationGuid)
        {
            var weeks = await GetNumberOfWeeksToCheckAsync();
            var clinicalEvents = GetClinicalEventsWhichOccuredDuringMedicationPeriod(patientFromRepo, medicationStartDate, medicationEndDate, weeks);
            var medications = PrepareMedicationsForLinkingToReport(medicationDisplayName, medicationGuid);

            foreach (var clinicalEvent in clinicalEvents)
            {
                await _workFlowService.AddOrUpdateMedicationsForWorkFlowInstanceAsync(clinicalEvent.PatientClinicalEventGuid, medications);
            }
        }

        private async Task<int> GetNumberOfWeeksToCheckAsync()
        {
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks);
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    return Convert.ToInt32(config.ConfigValue);
                }
            }
            return 0;
        }

        private IEnumerable<PatientClinicalEvent> GetClinicalEventsWhichOccuredDuringMedicationPeriod(Patient patientFromRepo, DateTime medicationStartDate, DateTime? medicationEndDate, int weekCount)
        {
            if (!medicationEndDate.HasValue)
            {
                return patientFromRepo.PatientClinicalEvents.Where(pce => pce.OnsetDate >= medicationStartDate.AddDays(weekCount * -7) && pce.Archived == false);
            }

            return patientFromRepo.PatientClinicalEvents.Where(pce => pce.OnsetDate >= medicationStartDate.AddDays(weekCount * -7) && pce.OnsetDate <= medicationEndDate.Value.AddDays(weekCount * 7) && pce.Archived == false);
        }

        private List<ReportInstanceMedicationListItem> PrepareMedicationsForLinkingToReport(string displayName, Guid patientMedicationGuid)
        {
            var instanceMedications = new List<ReportInstanceMedicationListItem>();
            instanceMedications.Add(new ReportInstanceMedicationListItem()
            {
                MedicationIdentifier = displayName,
                ReportInstanceMedicationGuid = patientMedicationGuid
            });
            return instanceMedications;
        }
    }
}
