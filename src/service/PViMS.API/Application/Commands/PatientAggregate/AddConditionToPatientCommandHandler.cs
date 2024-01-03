using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
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
    public class AddConditionToPatientCommandHandler
        : IRequestHandler<AddConditionToPatientCommand, PatientConditionIdentifierDto>
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Outcome> _outcomeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientStatus> _patientStatusRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IRepositoryInt<TreatmentOutcome> _treatmentOutcomeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IMapper _mapper;
        private readonly ILogger<AddConditionToPatientCommandHandler> _logger;

        public AddConditionToPatientCommandHandler(
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Outcome> outcomeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientStatus> patientStatusRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IRepositoryInt<TreatmentOutcome> treatmentOutcomeRepository,
            IUnitOfWorkInt unitOfWork,
            ILinkGeneratorService linkGeneratorService,
            IMapper mapper,
            ILogger<AddConditionToPatientCommandHandler> logger)
        {
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _outcomeRepository = outcomeRepository ?? throw new ArgumentNullException(nameof(outcomeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientStatusRepository = patientStatusRepository ?? throw new ArgumentNullException(nameof(patientStatusRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _treatmentOutcomeRepository = treatmentOutcomeRepository ?? throw new ArgumentNullException(nameof(treatmentOutcomeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PatientConditionIdentifierDto> Handle(AddConditionToPatientCommand message, CancellationToken cancellationToken)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == message.PatientId, new string[] {
                "PatientConditions.Condition",
                "PatientConditions.Outcome",
                "PatientConditions.TreatmentOutcome",
            });
            if (patientFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate patient {message.PatientId}");
            }

            var sourceTermFromRepo = await _terminologyMeddraRepository.GetAsync(tm => tm.Id == message.SourceTerminologyMedDraId);
            if (sourceTermFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate meddra terminology with an id of {message.SourceTerminologyMedDraId}");
            }

            Outcome outcomeFromRepo = null;
            if (!String.IsNullOrWhiteSpace(message.Outcome))
            {
                outcomeFromRepo = await _outcomeRepository.GetAsync(o => o.Description == message.Outcome);
                if (outcomeFromRepo == null)
                {
                    throw new KeyNotFoundException($"Unable to locate outcome with a description of {message.Outcome}");
                }
            }

            TreatmentOutcome treatmentOutcomeFromRepo = null;
            if (!String.IsNullOrWhiteSpace(message.TreatmentOutcome))
            {
                treatmentOutcomeFromRepo = await _treatmentOutcomeRepository.GetAsync(to => to.Description == message.TreatmentOutcome);
                if (treatmentOutcomeFromRepo == null)
                {
                    throw new KeyNotFoundException($"Unable to locate treatment outcome with a description of {message.TreatmentOutcome}");
                }
            }

            if (outcomeFromRepo != null && treatmentOutcomeFromRepo != null)
            {
                if (outcomeFromRepo.Description == "Fatal" && treatmentOutcomeFromRepo.Description != "Died")
                {
                    throw new DomainException($"Treatment Outcome not consistent with Condition Outcome");
                }
                if (outcomeFromRepo.Description != "Fatal" && treatmentOutcomeFromRepo.Description == "Died")
                {
                    throw new DomainException($"Condition Outcome not consistent with Treatment Outcome");
                }
            }

            var conditionDetail = await PrepareConditionDetailAsync(message.Attributes);
            if (!conditionDetail.IsValid())
            {
                conditionDetail.InvalidAttributes.ForEach(element => throw new DomainException(element));
            }

            var patientCondition = patientFromRepo.AddOrUpdatePatientCondition(0,
                        sourceTermFromRepo,
                        message.StartDate,
                        message.OutcomeDate,
                        outcomeFromRepo,
                        treatmentOutcomeFromRepo,
                        message.CaseNumber,
                        message.Comments,
                        message.SourceDescription,
                        _patientStatusRepository.Get(ps => ps.Description == "Died"));

            _modelExtensionBuilder.UpdateExtendable(patientCondition, conditionDetail.CustomAttributes, "Admin");

            _patientRepository.Update(patientFromRepo);

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"----- Condition {message.SourceDescription} created");

            var mappedPatientCondition = _mapper.Map<PatientConditionIdentifierDto>(patientCondition);

            CreateLinks(mappedPatientCondition);

            return mappedPatientCondition;
        }

        private async Task<ConditionDetail> PrepareConditionDetailAsync(IDictionary<int, string> attributes)
        {
            var ConditionDetail = new ConditionDetail();
            ConditionDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientCondition>();

            foreach (var newAttribute in attributes)
            {
                var customAttribute = await _customAttributeRepository.GetAsync(ca => ca.Id == newAttribute.Key);
                if (customAttribute == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute {newAttribute.Key}");
                }

                var attributeDetail = ConditionDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);

                if (attributeDetail == null)
                {
                    throw new KeyNotFoundException($"Unable to locate custom attribute on patient lab test {newAttribute.Key}");
                }

                attributeDetail.Value = newAttribute.Value;
            }

            return ConditionDetail;
        }

        private void CreateLinks(PatientConditionIdentifierDto dto)
        {
            dto.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientCondition", dto.Id), "self", "GET"));
        }
    }
}
