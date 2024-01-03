using MediatR;
using Microsoft.Extensions.Logging;
using PViMS.Services.FileHandler;
using PVIMS.API.Application.Queries.PatientAggregate;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Commands.PatientAggregate
{
    public class ProcessFollowUpFormCommandHandler
        : IRequestHandler<ProcessFollowUpFormCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly ILogger<ProcessFollowUpFormCommandHandler> _logger;

        public ProcessFollowUpFormCommandHandler(
            IMediator mediator,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IUnitOfWorkInt unitOfWork,
            ILogger<ProcessFollowUpFormCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessFollowUpFormCommand message, CancellationToken cancellationToken)
        {
            var filePaths = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\source", "followUp_form.xlsx");

            if (filePaths.Length > 0)
            {
                var filePath = filePaths.First();

                _logger.LogInformation($"----- Processing file {filePath}");

                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var importFile = new ImportFollowUpSheet(file);

                foreach (var row in importFile)
                {
                    var patientId = 0;

                    var query = new PatientExpandedByConditionTermQuery(
                        row.ASMCode);

                    _logger.LogInformation(
                        "----- Sending query: PatientExpandedByConditionTermQuery");

                    var queryResult = await _mediator.Send(query);

                    if(queryResult != null)
                    {
                        patientId = queryResult.Id;

                        await ChangePatientName(row, patientId);
                        await AddMedicines(patientId, row);
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"----- unable to locate patient using case number {row.ASMCode}");
                    }
                }
            }

            return await _unitOfWork.CompleteAsync();
        }

        private async Task ChangePatientName(ImportFollowUpSheet.ImportFollowUpSheetRow row, int patientId)
        {
            var command = new ChangePatientNameCommand(
                patientId,
                firstName: row.FirstName,
                middleName: string.Empty,
                lastName: row.LastName);

            _logger.LogInformation(
                "----- Sending command: ChangePatientNameCommand - {patientId}",
                patientId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                throw new Exception("Command not created");
            }
        }

        private async Task AddMedicines(int patientId, ImportFollowUpSheet.ImportFollowUpSheetRow row)
        {
            List<AttributeValueForPostDto> attributes = new List<AttributeValueForPostDto>();

            if (!String.IsNullOrWhiteSpace(row.MedName1))
            {
                var command = new AddMedicationToPatientCommand(
                    patientId,
                    sourceDescription: row.MedName1,
                    conceptId: 1,
                    productId: null,
                    startDate: row.MedStart1.Value,
                    endDate: row.MedEnd1,
                    dose: row.MedDose1,
                    doseFrequency: string.Empty,
                    doseUnit: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddMedicationToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (!String.IsNullOrWhiteSpace(row.MedName2))
            {
                var command = new AddMedicationToPatientCommand(
                    patientId,
                    sourceDescription: row.MedName2,
                    conceptId: 1,
                    productId: null,
                    startDate: row.MedStart2.Value,
                    endDate: row.MedEnd2,
                    dose: row.MedDose2,
                    doseFrequency: string.Empty,
                    doseUnit: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddMedicationToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (!String.IsNullOrWhiteSpace(row.MedName3))
            {
                var command = new AddMedicationToPatientCommand(
                    patientId,
                    sourceDescription: row.MedName3,
                    conceptId: 1,
                    productId: null,
                    startDate: row.MedStart3.Value,
                    endDate: row.MedEnd3,
                    dose: row.MedDose3,
                    doseFrequency: string.Empty,
                    doseUnit: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddMedicationToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }

            if (!String.IsNullOrWhiteSpace(row.MedName4))
            {
                var command = new AddMedicationToPatientCommand(
                    patientId,
                    sourceDescription: row.MedName4,
                    conceptId: 1,
                    productId: null,
                    startDate: row.MedStart4.Value,
                    endDate: row.MedEnd4,
                    dose: row.MedDose4,
                    doseFrequency: string.Empty,
                    doseUnit: string.Empty,
                    attributes: attributes.ToDictionary(x => x.Id, x => x.Value));

                _logger.LogInformation(
                    "----- Sending command: AddMedicationToPatientCommand - {SourceDescription}",
                    command.SourceDescription);

                var commandResult = await _mediator.Send(command);

                if (commandResult == null)
                {
                    throw new Exception("Command not created");
                }
            }
        }
    }
}
