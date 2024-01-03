using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.PatientAggregate;
using PVIMS.API.Application.Queries.PatientAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PVIMS.Core.Aggregates.UserAggregate;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientConditionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PatientConditionsController> _logger;

        public PatientConditionsController(IMediator mediator,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PatientConditionsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single patient condition using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient condition</param>
        /// <returns>An ActionResult of type PatientConditionIdentifierDto</returns>
        [HttpGet("{patientId}/conditions/{id}", Name = "GetPatientConditionByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientConditionIdentifierDto>> GetPatientConditionByIdentifier(long patientId, long id)
        {
            var mappedPatientCondition = await GetPatientConditionAsync<PatientConditionIdentifierDto>(patientId, id);
            if (mappedPatientCondition == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientCondition<PatientConditionIdentifierDto>(mappedPatientCondition));
        }

        /// <summary>
        /// Get a single patient condition using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient condition</param>
        /// <returns>An ActionResult of type PatientConditionDetailDto</returns>
        [HttpGet("{patientId}/conditions/{id}", Name = "GetPatientConditionByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientConditionDetailDto>> GetPatientConditionByDetail(int patientId, int id)
        {
            var query = new PatientConditionDetailQuery(patientId, id);

            _logger.LogInformation(
                "----- Sending query: PatientConditionDetailQuery - {patientId} : {id}",
                patientId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Create a new patient condition record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="conditionForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/conditions", Name = "CreatePatientCondition")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientCondition(int patientId, 
            [FromBody] PatientConditionForUpdateDto conditionForUpdate)
        {
            if (conditionForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new condition");
                return BadRequest(ModelState);
            }

            var command = new AddConditionToPatientCommand(patientId,
                conditionForUpdate.SourceDescription,
                conditionForUpdate.SourceTerminologyMedDraId,
                conditionForUpdate.StartDate,
                conditionForUpdate.OutcomeDate,
                conditionForUpdate.Outcome,
                conditionForUpdate.TreatmentOutcome,
                conditionForUpdate.CaseNumber,
                conditionForUpdate.Comments,
                conditionForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddConditionToPatientCommand - {SourceDescription}",
                command.SourceDescription);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientConditionByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing patient condition
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the condition</param>
        /// <param name="conditionForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/conditions/{id}", Name = "UpdatePatientCondition")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientCondition(int patientId, int id,
            [FromBody] PatientConditionForUpdateDto conditionForUpdate)
        {
            if (conditionForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeConditionDetailsCommand(patientId, 
                id, 
                conditionForUpdate.SourceTerminologyMedDraId, 
                conditionForUpdate.StartDate, 
                conditionForUpdate.OutcomeDate, 
                conditionForUpdate.Outcome, 
                conditionForUpdate.TreatmentOutcome, 
                conditionForUpdate.CaseNumber, 
                conditionForUpdate.Comments,
                conditionForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangeConditionDetailsCommand - {patientId}: {patientConditionId}",
                command.PatientId,
                command.PatientConditionId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient condition
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the condition</param>
        /// <param name="conditionForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/conditions/{id}/archive", Name = "ArchivePatientCondition")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientCondition(int patientId, int id,
            [FromBody] ArchiveDto conditionForDelete)
        {
            if (conditionForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ArchivePatientConditionCommand(patientId, id, conditionForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientConditionCommand - {patientId}: {patientConditionId}",
                command.PatientId,
                command.PatientConditionId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get single patient condition from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientConditionAsync<T>(long patientId, long id) where T : class
        {
            var patientConditionFromRepo = await _patientConditionRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientConditionFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientCondition = _mapper.Map<T>(patientConditionFromRepo);

                return mappedPatientCondition;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientConditionIdentifierDto CreateLinksForPatientCondition<T>(T dto)
        {
            PatientConditionIdentifierDto identifier = (PatientConditionIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientCondition", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
