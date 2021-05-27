using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.Queries.ReportInstanceAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientClinicalEventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IReportInstanceQueries _reportInstanceQueries;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly ILogger<PatientClinicalEventsController> _logger;

        public PatientClinicalEventsController(IMediator mediator, 
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IReportInstanceQueries reportInstanceQueries,
            ICustomAttributeService customAttributeService,
            ILogger<PatientClinicalEventsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _reportInstanceQueries = reportInstanceQueries ?? throw new ArgumentNullException(nameof(reportInstanceQueries));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventIdentifierDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientClinicalEventIdentifierDto>> GetPatientClinicalEventByIdentifier(long patientId, long id)
        {
            var mappedPatientClinicalEvent = await GetPatientClinicalEventAsync<PatientClinicalEventIdentifierDto>(patientId, id);
            if (mappedPatientClinicalEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientClinicalEvent<PatientClinicalEventIdentifierDto>(mappedPatientClinicalEvent));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventDetailDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientClinicalEventDetailDto>> GetPatientClinicalEventByDetail(long patientId, long id)
        {
            var mappedPatientClinicalEvent = await GetPatientClinicalEventAsync<PatientClinicalEventDetailDto>(patientId, id);
            if (mappedPatientClinicalEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientClinicalEvent<PatientClinicalEventDetailDto>(CustomPatientClinicalEventMap(mappedPatientClinicalEvent)));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventExpandedDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientClinicalEventExpandedDto>> GetPatientClinicalEventByExpanded(long patientId, long id)
        {
            var mappedPatientClinicalEvent = await GetPatientClinicalEventAsync<PatientClinicalEventExpandedDto>(patientId, id);
            if (mappedPatientClinicalEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientClinicalEvent<PatientClinicalEventExpandedDto>(await CustomPatientClinicalEventMapAsync(mappedPatientClinicalEvent)));
        }

        /// <summary>
        /// Create a new patient clinical event record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="clinicalEventForUpdate">The clinical event payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/clinicalevents", Name = "CreatePatientClinicalEvent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientClinicalEvent(int patientId, 
            [FromBody] PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            if (clinicalEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new clinical event");
                return BadRequest(ModelState);
            }

            var command = new AddClinicalEventToPatientCommand(patientId,
                clinicalEventForUpdate.SourceDescription, clinicalEventForUpdate.SourceTerminologyMedDraId, clinicalEventForUpdate.OnsetDate, clinicalEventForUpdate.ResolutionDate, clinicalEventForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddClinicalEventToPatientCommand - {sourceDescription}",
                command.SourceDescription);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientClinicalEventByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);

        }

        /// <summary>
        /// Update an existing patient clinical event
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the clinical event</param>
        /// <param name="clinicalEventForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/clinicalevents/{id}", Name = "UpdatePatientClinicalEvent")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientClinicalEvent(int patientId, int id,
            [FromBody] PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            if (clinicalEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeClinicalEventDetailsCommand(patientId, id, clinicalEventForUpdate.SourceDescription, clinicalEventForUpdate.SourceTerminologyMedDraId, clinicalEventForUpdate.OnsetDate, clinicalEventForUpdate.ResolutionDate, clinicalEventForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangeClinicalEventDetailsCommand - {patientId}: {patientClinicalEventId}",
                command.PatientId,
                command.PatientClinicalEventId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient clinical event
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the clinical event</param>
        /// <param name="conditionForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/clinicalevents/{id}/archive", Name = "ArchivePatientClinicalEvent")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientClinicalEvent(int patientId, int id,
            [FromBody] ArchiveDto conditionForDelete)
        {
            if (conditionForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ArchivePatientClinicalEventCommand(patientId, id, conditionForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientClinicalEventCommand - {patientId}: {patientClinicalEventId}",
                command.PatientId,
                command.PatientClinicalEventId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get single patient clinical event from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientClinicalEventAsync<T>(long patientId, long id) where T : class
        {
            var patientClinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientClinicalEventFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientClinicalEvent = _mapper.Map<T>(patientClinicalEventFromRepo);

                return mappedPatientClinicalEvent;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientClinicalEventIdentifierDto CreateLinksForPatientClinicalEvent<T>(T dto)
        {
            PatientClinicalEventIdentifierDto identifier = (PatientClinicalEventIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientClinicalEvent", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientClinicalEventDetailDto CustomPatientClinicalEventMap(PatientClinicalEventDetailDto dto)
        {
            var patientClinicalEvent = _patientClinicalEventRepository.Get(p => p.Id == dto.Id);
            if (patientClinicalEvent == null)
            {
                return dto;
            }
            IExtendable patientClinicalEventExtended = patientClinicalEvent;

            // Map all custom attributes
            dto.ClinicalEventAttributes = _modelExtensionBuilder.BuildModelExtension(patientClinicalEventExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.ReportDate = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Date of Report", patientClinicalEventExtended);
            dto.IsSerious = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Is the adverse event serious?", patientClinicalEventExtended);

            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private async Task<PatientClinicalEventExpandedDto> CustomPatientClinicalEventMapAsync(PatientClinicalEventExpandedDto dto)
        {
            var patientClinicalEventFromRepo = _patientClinicalEventRepository.Get(p => p.Id == dto.Id);
            if (patientClinicalEventFromRepo == null)
            {
                return dto;
            }
            IExtendable patientClinicalEventExtended = patientClinicalEventFromRepo;

            // Map all custom attributes
            dto.ClinicalEventAttributes = _modelExtensionBuilder.BuildModelExtension(patientClinicalEventExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.ReportDate = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Date of Report", patientClinicalEventExtended);
            dto.IsSerious = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Is the adverse event serious?", patientClinicalEventExtended);

            var activity = await _reportInstanceQueries.GetExecutionStatusEventsForEventViewAsync(patientClinicalEventFromRepo.Id);
            dto.Activity = activity.ToList();

            var reportInstanceFromRepo = _reportInstanceRepository.Get(ri => ri.ContextGuid == patientClinicalEventFromRepo.PatientClinicalEventGuid);
            if(reportInstanceFromRepo == null)
            {
                return dto;
            }

            // Meddra term
            dto.SetMedDraTerm = reportInstanceFromRepo.TerminologyMedDra?.DisplayName;

            // Meddra medications
            dto.Medications = _mapper.Map<ICollection<ReportInstanceMedicationDetailDto>>(reportInstanceFromRepo.Medications.Where(m => !String.IsNullOrWhiteSpace(m.WhoCausality) || (!String.IsNullOrWhiteSpace(m.NaranjoCausality))));

            return dto;
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
