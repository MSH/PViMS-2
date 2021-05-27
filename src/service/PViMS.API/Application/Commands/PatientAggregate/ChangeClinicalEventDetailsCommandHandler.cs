using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.Core.Entities;
using PVIMS.Core.Exceptions;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ChangeClinicalEventDetailsCommandHandler
        : IRequestHandler<ChangeClinicalEventDetailsCommand, bool>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IWorkFlowService _workFlowService;
        private readonly ILogger<ChangeMedicationDetailsCommandHandler> _logger;

        public ChangeClinicalEventDetailsCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IUnitOfWorkInt unitOfWork,
            IWorkFlowService workFlowService,
            ILogger<ChangeMedicationDetailsCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ChangeClinicalEventDetailsCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] { "PatientClinicalEvents.SourceTerminologyMedDra" });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException("Unable to locate patient");
            }

            TerminologyMedDra sourceTermFromRepo = null;
            if (message.SourceTerminologyMedDraId.HasValue)
            {
                sourceTermFromRepo = await _terminologyMeddraRepository.GetAsync(message.SourceTerminologyMedDraId); ;
                if (sourceTermFromRepo == null)
                {
                    throw new KeyNotFoundException("Unable to locate terminology for MedDRA");
                }
            }

            var clinicalEventDetail = await PrepareClinicalEventDetailAsync(message.Attributes);
            if (!clinicalEventDetail.IsValid())
            {
                clinicalEventDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            patientFromRepo.ChangeClinicalEventDetails(message.PatientClinicalEventId, message.OnsetDate, message.ResolutionDate, sourceTermFromRepo, message.SourceDescription);
            var clinicalEvent = patientFromRepo.PatientClinicalEvents.Single(pce => pce.Id == message.PatientClinicalEventId);
            _modelExtensionBuilder.UpdateExtendable(clinicalEvent, clinicalEventDetail.CustomAttributes, "Admin");

            _patientRepository.Update(patientFromRepo);

            _workFlowService.UpdateIdentifiersForWorkFlowInstance(clinicalEvent.PatientClinicalEventGuid, patientFromRepo.FullName, clinicalEvent.SourceTerminologyMedDra.DisplayName);

            _logger.LogInformation($"----- Clinical Event {message.PatientClinicalEventId} details updated");

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<ClinicalEventDetail> PrepareClinicalEventDetailAsync(IDictionary<int, string> attributes)
        {
            var clinicalEventDetail = new ClinicalEventDetail();
            clinicalEventDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientClinicalEvent>();

            //clinicalEventDetail = _mapper.Map<ClinicalEventDetail>(clinicalEventForUpdate);
            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = clinicalEventDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient clinical event {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }

            return clinicalEventDetail;
        }
    }
}
