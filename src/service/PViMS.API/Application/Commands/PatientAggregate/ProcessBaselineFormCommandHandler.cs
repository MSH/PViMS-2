using MediatR;
using Microsoft.Extensions.Logging;
using PViMS.Services.FileHandler;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ProcessBaselineFormCommandHandler
        : IRequestHandler<ProcessBaselineFormCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ProcessBaselineFormCommandHandler> _logger;

        public ProcessBaselineFormCommandHandler(
            IMediator mediator,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ProcessBaselineFormCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessBaselineFormCommand message, CancellationToken cancellationToken)
        {
            var filePaths = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\source", "baseline_form.xlsx");

            if (filePaths.Length > 0)
            {
                var filePath = filePaths.First();

                _logger.LogInformation($"----- Processing file {filePath}");

                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var importFile = new ImportBaselineSheet(file);

                foreach (var row in importFile)
                {
                    var patientId = await AddPatient(row);

                    await AddLabTests(patientId, row);
                    await AddConditions(patientId, row);
                }
            }

            return await _unitOfWork.CompleteAsync();
        }

        private async Task<int> AddPatient(ImportBaselineSheet.ImportBaselineSheetRow row)
        {
            var attributes = await PrepareAttributesAsync(row);

            var command = new AddPatientCommand(
                firstName: "Not Matched",
                lastName: "Not Matched",
                middleName: String.Empty,
                dateOfBirth: row.DateOfBirth.Value,
                facilityName: row.TreatmentSite,
                conditionGroupId: 1,
                meddraTermId: 28879,
                cohortGroupId: 1,
                enroledDate: null,
                startDate: DateTime.Today,
                outcomeDate: null,
                caseNumber: row.ASMCode,
                comments: String.Empty,
                encounterTypeId: 1,
                priorityId: 1,
                encounterDate: DateTime.Today,
                attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                $"----- Sending command: AddPatientCommand - {row.ASMCode} ({command})");

            var errorMessage = string.Empty;

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                throw new Exception("Command not created");
            }

            return commandResult.Id;
        }

        private async Task AddLabTests(int patientId, ImportBaselineSheet.ImportBaselineSheetRow row)
        {
            List<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();

            if (row.ALTTestDate.HasValue)
            {
                var command = new AddLabTestToPatientCommand(
                    patientId: patientId,
                    labTest: "ALT (SGPT)",
                    testDate: row.ALTTestDate.Value,
                    testResultCoded: string.Empty,
                    testResultValue: row.ALTResult,
                    testUnit: string.Empty,
                    referenceLower: string.Empty,
                    referenceUpper: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddLabTestToPatientCommand - {LabTest} - ALT",
                    command.LabTest);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.ASTTestDate.HasValue)
            {
                var command = new AddLabTestToPatientCommand(
                    patientId: patientId,
                    labTest: "AST (SGOT)",
                    testDate: row.ASTTestDate.Value,
                    testResultCoded: string.Empty,
                    testResultValue: row.ASTResult,
                    testUnit: string.Empty,
                    referenceLower: string.Empty,
                    referenceUpper: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddLabTestToPatientCommand - {LabTest} - AST",
                    command.LabTest);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.CD4TestDate.HasValue)
            {
                var command = new AddLabTestToPatientCommand(
                    patientId: patientId,
                    labTest: "CD4 Count",
                    testDate: row.CD4TestDate.Value,
                    testResultCoded: string.Empty,
                    testResultValue: row.CD4Result,
                    testUnit: string.Empty,
                    referenceLower: string.Empty,
                    referenceUpper: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddLabTestToPatientCommand - {LabTest} - CD4",
                    command.LabTest);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.GlycemiaTestDate.HasValue)
            {
                var command = new AddLabTestToPatientCommand(
                    patientId: patientId,
                    labTest: "Glycemia",
                    testDate: row.GlycemiaTestDate.Value,
                    testResultCoded: string.Empty,
                    testResultValue: row.GlycemiaResult,
                    testUnit: string.Empty,
                    referenceLower: string.Empty,
                    referenceUpper: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddLabTestToPatientCommand - {LabTest} - Glycemia",
                    command.LabTest);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.TriglyceridesTestDate.HasValue)
            {
                var command = new AddLabTestToPatientCommand(
                    patientId: patientId,
                    labTest: "Triglycerides",
                    testDate: row.TriglyceridesTestDate.Value,
                    testResultCoded: string.Empty,
                    testResultValue: row.TriglyceridesResult,
                    testUnit: string.Empty,
                    referenceLower: string.Empty,
                    referenceUpper: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddLabTestToPatientCommand - {LabTest} - Triglycerides",
                    command.LabTest);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }
        }

        private async Task AddConditions(int patientId, ImportBaselineSheet.ImportBaselineSheetRow row)
        {
            List<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();

            if (row.Diabetes.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "Diabetes",
                    sourceTerminologyMedDraId: 76577,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.CardioVascularDisease.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "Cardiovascular Disease",
                    sourceTerminologyMedDraId: 69086,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.HepBCo.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "Hepatitis B coinfection",
                    sourceTerminologyMedDraId: 91491,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.HepCCo.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "Hepatitis C coinfection",
                    sourceTerminologyMedDraId: 77185,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.HepaticInsufficiency.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "Hepatic Insufficiency",
                    sourceTerminologyMedDraId: 74342,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.RenalInsufficiency.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "Renal Insufficiency",
                    sourceTerminologyMedDraId: 46639,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.Tuberculosis.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "DS Tuberculosis (TB)",
                    sourceTerminologyMedDraId: 52627,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (row.MDR.ToLowerInvariant() == "yes")
            {
                var command = new AddConditionToPatientCommand(
                    patientId: patientId,
                    sourceDescription: "MDR Tuberculosis (TB)",
                    sourceTerminologyMedDraId: 52627,
                    startDate: DateTime.Today,
                    outcomeDate: null,
                    outcome: string.Empty,
                    treatmentOutcome: string.Empty,
                    caseNumber: string.Empty,
                    comments: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

        }

        private async Task<ICollection<AttributeValueForPostDto>> PrepareAttributesAsync(ImportBaselineSheet.ImportBaselineSheetRow row)
        {
            ICollection<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();

            var genderCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Gender");
            var selectionDataItem = await _selectionDataItemRepository.GetAsync(s => s.AttributeKey == "Gender" && s.Value == row.Sex);
            if (genderCustomAttribute != null && selectionDataItem != null)
            {
                attributes.Add(new AttributeValueForPostDto() { Id = genderCustomAttribute.Id, Value = selectionDataItem.SelectionKey });
            }

            var medicalRecordNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Medical Record Number");
            if (medicalRecordNumberCustomAttribute != null)
            {
                attributes.Add(new AttributeValueForPostDto() { Id = medicalRecordNumberCustomAttribute.Id, Value = row.ASMCode });
            }

            var addressCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Address");
            if (addressCustomAttribute != null)
            {
                attributes.Add(new AttributeValueForPostDto() { Id = addressCustomAttribute.Id, Value = row.Address });
            }

            var contactNumberCustomAttribute = await _customAttributeRepository.GetAsync(ca => ca.AttributeKey == "Patient Contact Number");
            if (contactNumberCustomAttribute != null)
            {
                attributes.Add(new AttributeValueForPostDto() { Id = contactNumberCustomAttribute.Id, Value = row.PhoneNumber });
            }

            return attributes;
        }
    }
}
