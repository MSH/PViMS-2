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
    public class PatientLabTestsController : ControllerBase
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<PatientLabTestsController> _logger;
        private readonly ILinkGeneratorService _linkGeneratorService;

        public PatientLabTestsController(
            IMapper mapper,
            IMediator mediator,
            ILogger<PatientLabTestsController> logger,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
        }

        /// <summary>
        /// Get a single patient lab test using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient lab test</param>
        /// <returns>An ActionResult of type PatientLabTestIdentifierDto</returns>
        [HttpGet("{patientId}/labtests/{id}", Name = "GetPatientLabTestByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientLabTestIdentifierDto>> GetPatientLabTestByIdentifier(long patientId, long id)
        {
            var mappedPatientLabTest = await GetPatientLabTestAsync<PatientLabTestIdentifierDto>(patientId, id);
            if (mappedPatientLabTest == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientLabTest<PatientLabTestIdentifierDto>(mappedPatientLabTest));
        }

        /// <summary>
        /// Get a single patient lab test using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient labTest</param>
        /// <returns>An ActionResult of type PatientLabTestDetailDto</returns>
        [HttpGet("{patientId}/labtests/{id}", Name = "GetPatientLabTestByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientLabTestDetailDto>> GetPatientLabTestByDetail(long patientId, long id)
        {
            var mappedPatientLabTest = await GetPatientLabTestAsync<PatientLabTestDetailDto>(patientId, id);
            if (mappedPatientLabTest == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientLabTest<PatientLabTestDetailDto>(CustomPatientLabTestMap(mappedPatientLabTest)));
        }

        /// <summary>
        /// Create a new patient lab test record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/labtests", Name = "CreatePatientLabTest")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientLabTest(int patientId, 
            [FromBody] PatientLabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new labTest");
                return BadRequest(ModelState);
            }

            var command = new AddLabTestToPatientCommand(patientId,
                labTestForUpdate.LabTest,
                labTestForUpdate.TestDate,
                labTestForUpdate.TestResultCoded,
                labTestForUpdate.TestResultValue,
                labTestForUpdate.TestUnit,
                labTestForUpdate.ReferenceLower,
                labTestForUpdate.ReferenceUpper,
                labTestForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: AddLabTestToPatientCommand - {LabTest}",
                command.LabTest);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetPatientLabTestByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing patient lab test
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForUpdate">The lab test payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/labTests/{id}", Name = "UpdatePatientLabTest")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientLabTest(int patientId, int id,
            [FromBody] PatientLabTestForUpdateDto labTestForUpdate)
        {
            if (labTestForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeLabTestDetailsCommand(patientId, 
                id,
                labTestForUpdate.LabTest,
                labTestForUpdate.TestDate,
                labTestForUpdate.TestResultCoded,
                labTestForUpdate.TestResultValue,
                labTestForUpdate.TestUnit,
                labTestForUpdate.ReferenceLower,
                labTestForUpdate.ReferenceUpper,
                labTestForUpdate.Attributes.ToDictionary(x => x.Id, x => x.Value));

            _logger.LogInformation(
                "----- Sending command: ChangeLabTestDetailsCommand - {patientId}: {patientLabTestId}",
                command.PatientId,
                command.PatientLabTestId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing patient lab test
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the lab test</param>
        /// <param name="labTestForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/labTests/{id}/archive", Name = "ArchivePatientLabTest")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientLabTest(int patientId, int id,
            [FromBody] ArchiveDto labTestForDelete)
        {
            if (labTestForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ArchivePatientLabTestCommand(patientId, id, labTestForDelete.Reason);

            _logger.LogInformation(
                "----- Sending command: ArchivePatientLabTestCommand - {patientId}: {patientLabTestId}",
                command.PatientId,
                command.PatientLabTestId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get single patient lab test from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientLabTestAsync<T>(long patientId, long id) where T : class
        {
            var patientLabTestFromRepo = await _patientLabTestRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientLabTestFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientLabTest = _mapper.Map<T>(patientLabTestFromRepo);

                return mappedPatientLabTest;
            }

            return null;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientLabTestDetailDto CustomPatientLabTestMap(PatientLabTestDetailDto dto)
        {
            var patientLabTest = _patientLabTestRepository.Get(p => p.Id == dto.Id);
            if (patientLabTest == null)
            {
                return dto;
            }
            IExtendable patientLabTestExtended = patientLabTest;

            // Map all custom attributes
            dto.LabTestAttributes = _modelExtensionBuilder.BuildModelExtension(patientLabTestExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            return dto;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientLabTestIdentifierDto CreateLinksForPatientLabTest<T>(T dto)
        {
            PatientLabTestIdentifierDto identifier = (PatientLabTestIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientLabTest", identifier.Id), "self", "GET"));

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
