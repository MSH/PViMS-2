using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientMedicationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly ILogger<PatientMedicationsController> _logger;

        public PatientMedicationsController(IMediator mediator,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            ICustomAttributeService customAttributeService,
            ILogger<PatientMedicationsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient medication using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient medication</param>
        /// <returns>An ActionResult of type PatientMedicationIdentifierDto</returns>
        [HttpGet("{patientId}/medications/{id}", Name = "GetPatientMedicationByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientMedicationIdentifierDto>> GetPatientMedicationByIdentifier(long patientId, long id)
        {
            var mappedPatientMedication = await GetPatientMedicationAsync<PatientMedicationIdentifierDto>(patientId, id);
            if (mappedPatientMedication == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientMedication<PatientMedicationIdentifierDto>(mappedPatientMedication));
        }

        /// <summary>
        /// Get a single patient medication using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient medication</param>
        /// <returns>An ActionResult of type PatientMedicationDetailDto</returns>
        [HttpGet("{patientId}/medications/{id}", Name = "GetPatientMedicationByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientMedicationDetailDto>> GetPatientMedicationByDetail(long patientId, long id)
        {
            var mappedPatientMedication = await GetPatientMedicationAsync<PatientMedicationDetailDto>(patientId, id);
            if (mappedPatientMedication == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientMedication<PatientMedicationDetailDto>(CustomPatientMedicationMap(mappedPatientMedication)));
        }

        /// <summary>
        /// Create a new patient medication record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="medicationForUpdate">The medication payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/medications", Name = "CreatePatientMedication")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientMedication(int patientId, 
            [FromBody] PatientMedicationForUpdateDto medicationForUpdate)
        {
            if (medicationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new medication");
                return BadRequest(ModelState);
            }

            var command = new AddMedicationToPatientCommand(patientId, 
                medicationForUpdate.SourceDescription, medicationForUpdate.ConceptId, medicationForUpdate.ProductId, medicationForUpdate.StartDate, medicationForUpdate.EndDate, medicationForUpdate.Dose,
                medicationForUpdate.DoseFrequency, medicationForUpdate.DoseUnit, medicationForUpdate.Attributes);

            _logger.LogInformation(
                "----- Sending command: AddMedicationToPatientCommand - {conceptId}",
                command.ConceptId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtRoute("GetPatientMedicationByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing patient medication
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="medicationForUpdate">The medication payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/medications/{id}", Name = "UpdatePatientMedication")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientMedication(int patientId, int id,
            [FromBody] PatientMedicationForUpdateDto medicationForUpdate)
        {
            if (medicationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeMedicationDetailsCommand(patientId, id, medicationForUpdate.StartDate, medicationForUpdate.EndDate, medicationForUpdate.Dose, medicationForUpdate.DoseFrequency, medicationForUpdate.DoseUnit, medicationForUpdate.Attributes);

            _logger.LogInformation(
                "----- Sending command: ChangeMedicationDetailsCommand - {patientId}: {patientMedicationId}",
                command.PatientId,
                command.PatientMedicationId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient medication
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="medicationForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/medications/{id}/archive", Name = "ArchivePatientMedication")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientMedication(int patientId, int id,
            [FromBody] ArchiveDto medicationForDelete)
        {
            if (medicationForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ArchivePatientMedicationCommand(patientId, id, medicationForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientMedicationCommand - {patientId}: {patientMedicationId}",
                command.PatientId,
                command.PatientMedicationId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get single patient medication from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientMedicationAsync<T>(long patientId, long id) where T : class
        {
            var patientMedicationFromRepo = await _patientMedicationRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientMedicationFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientMedication = _mapper.Map<T>(patientMedicationFromRepo);

                return mappedPatientMedication;
            }

            return null;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientMedicationDetailDto CustomPatientMedicationMap(PatientMedicationDetailDto dto)
        {
            var patientMedication = _patientMedicationRepository.Get(p => p.Id == dto.Id);
            if (patientMedication == null)
            {
                return dto;
            }
            IExtendable patientMedicationExtended = patientMedication;

            // Map all custom attributes
            dto.MedicationAttributes = _modelExtensionBuilder.BuildModelExtension(patientMedicationExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.IndicationType = _customAttributeService.GetCustomAttributeValue("PatientMedication", "Type of Indication", patientMedicationExtended);

            return dto;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientMedicationIdentifierDto CreateLinksForPatientMedication<T>(T dto)
        {
            PatientMedicationIdentifierDto identifier = (PatientMedicationIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientMedication", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Get the corresponding selection value
        /// </summary>
        /// <param name="attributeKey">The custom attribute key look up value</param>
        /// <param name="selectionKey">The selection key look up value</param>
        /// <returns></returns>
        private string GetSelectionValue(string attributeKey, string selectionKey)
        {
            var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

            return (selectionitem == null) ? string.Empty : selectionitem.Value;
        }
    }
}
