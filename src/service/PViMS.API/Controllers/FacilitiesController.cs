using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Commands.FacilityAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class FacilitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<FacilitiesController> _logger;

        public FacilitiesController(IMediator mediator, 
            IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<FacilityType> facilityTypeRepository,
            ILogger<FacilitiesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all facilities using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of FacilityIdentifierDto</returns>
        [HttpGet("facilities", Name = "GetFacilitiesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>> GetFacilitiesByIdentifier(
            [FromQuery] FacilityResourceParameters facilityResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<FacilityIdentifierDto>
                (facilityResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedFacilitiesWithLinks = GetFacilities<FacilityIdentifierDto>(facilityResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<FacilityIdentifierDto>(mappedFacilitiesWithLinks.TotalCount, mappedFacilitiesWithLinks);
            var wrapperWithLinks = CreateLinksForFacilities(wrapper, facilityResourceParameters,
                mappedFacilitiesWithLinks.HasNext, mappedFacilitiesWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all facilities using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of FacilityDetailDto</returns>
        [HttpGet("facilities", Name = "GetFacilitiesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<FacilityDetailDto>> GetFacilitiesByDetail(
            [FromQuery] FacilityResourceParameters facilityResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<FacilityDetailDto>
                (facilityResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedFacilitiesWithLinks = GetFacilities<FacilityDetailDto>(facilityResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<FacilityDetailDto>(mappedFacilitiesWithLinks.TotalCount, mappedFacilitiesWithLinks);
            var wrapperWithLinks = CreateLinksForFacilities(wrapper, facilityResourceParameters,
                mappedFacilitiesWithLinks.HasNext, mappedFacilitiesWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single facility using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Facility</param>
        /// <returns>An ActionResult of type FacilityIdentifierDto</returns>
        [HttpGet("facilities/{id}", Name = "GetFacilityByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<FacilityIdentifierDto>> GetFacilityByIdentifier(long id)
        {
            var mappedFacility = await GetFacilityAsync<FacilityIdentifierDto>(id);
            if (mappedFacility == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForFacility<FacilityIdentifierDto>(mappedFacility));
        }

        /// <summary>
        /// Get a single facility using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Facility</param>
        /// <returns>An ActionResult of type FacilityDetailDto</returns>
        [HttpGet("facilities/{id}", Name = "GetFacilityByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<FacilityDetailDto>> GetFacilityByDetail(long id)
        {
            var mappedFacility = await GetFacilityAsync<FacilityDetailDto>(id);
            if (mappedFacility == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForFacility<FacilityDetailDto>(mappedFacility));
        }

        /// <summary>
        /// Get all form types using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of FacilityTypeIdentifierDto</returns>
        [HttpGet("facilitytypes", Name = "GetFacilityTypesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>> GetFacilityTypesByIdentifier(
            [FromQuery] FacilityTypeResourceParameters facilityTypeResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<FacilityTypeIdentifierDto, FacilityType>
               (facilityTypeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedFacilityTypesWithLinks = GetFacilityTypes<FacilityTypeIdentifierDto>(facilityTypeResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<FacilityTypeIdentifierDto>(mappedFacilityTypesWithLinks.TotalCount, mappedFacilityTypesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, medicationResourceParameters,
            //    mappedMedicationsWithLinks.HasNext, mappedMedicationsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Create a new facility
        /// </summary>
        /// <param name="facilityForUpdate">The facility payload</param>
        /// <returns></returns>
        [HttpPost("facilities", Name = "CreateFacility")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateFacility(
            [FromBody] FacilityForUpdateDto facilityForUpdate)
        {
            if (facilityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new facility");
                return BadRequest(ModelState);
            }

            var command = new AddFacilityCommand(facilityForUpdate.FacilityName, facilityForUpdate.FacilityCode, facilityForUpdate.FacilityType, facilityForUpdate.TelNumber, facilityForUpdate.MobileNumber, facilityForUpdate.FaxNumber);

            _logger.LogInformation(
                "----- Sending command: AddFacilityCommand - {facilityName}",
                command.FacilityName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtRoute("GetFacilityByDetail",
                new
                {
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing facility
        /// </summary>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="facilityForUpdate">The facility payload</param>
        /// <returns></returns>
        [HttpPut("facilities/{id}", Name = "UpdateFacility")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateFacility(int id,
            [FromBody] FacilityForUpdateDto facilityForUpdate)
        {
            if (facilityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for facility");
                return BadRequest(ModelState);
            }

            var command = new ChangeFacilityDetailsCommand(id, facilityForUpdate.FacilityName, facilityForUpdate.FacilityCode, facilityForUpdate.FacilityType, facilityForUpdate.TelNumber, facilityForUpdate.MobileNumber, facilityForUpdate.FaxNumber);

            _logger.LogInformation(
                "----- Sending command: ChangeFacilityDetailsCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing facility
        /// </summary>
        /// <param name="id">The unique id of the facility</param>
        /// <returns></returns>
        [HttpDelete("facilities/{id}", Name = "DeleteFacility")]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            var command = new DeleteFacilityCommand(id);

            _logger.LogInformation(
                "----- Sending command: DeleteFacilityCommand - {Id}",
                command.Id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return NoContent();
        }

        /// <summary>
        /// Get facilities from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="facilityResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetFacilities<T>(FacilityResourceParameters facilityResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = facilityResourceParameters.PageNumber,
                PageSize = facilityResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Facility>(facilityResourceParameters.OrderBy, "asc");

            var pagedFacilitiesFromRepo = _facilityRepository.List(pagingInfo, null, orderby, "");
            if (pagedFacilitiesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedFacilities = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedFacilitiesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedFacilitiesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedFacilities.TotalCount,
                    pageSize = mappedFacilities.PageSize,
                    currentPage = mappedFacilities.CurrentPage,
                    totalPages = mappedFacilities.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedFacilities.ForEach(dto => CreateLinksForFacility(dto));

                return mappedFacilities;
            }

            return null;
        }

        /// <summary>
        /// Get single facility from repository and auto map to Dto
        /// </summary>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetFacilityAsync<T>(long id) where T : class
        {
            var predicate = PredicateBuilder.New<Facility>(true);

            // Build remaining expressions
            predicate = predicate.And(f => f.Id == id);

            var facilityFromRepo = await _facilityRepository.GetAsync(predicate);

            if (facilityFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedFacility = _mapper.Map<T>(facilityFromRepo);

                return mappedFacility;
            }

            return null;
        }

        /// <summary>
        /// Get facility types from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="facilityTypeResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetFacilityTypes<T>(FacilityTypeResourceParameters facilityTypeResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = facilityTypeResourceParameters.PageNumber,
                PageSize = facilityTypeResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<FacilityType>(facilityTypeResourceParameters.OrderBy, "asc");

            var pagedFacilityTypesFromRepo = _facilityTypeRepository.List(pagingInfo, null, orderby, "");
            if (pagedFacilityTypesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedFacilityTypes = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedFacilityTypesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedFacilityTypesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedFacilityTypes.TotalCount,
                    pageSize = mappedFacilityTypes.PageSize,
                    currentPage = mappedFacilityTypes.CurrentPage,
                    totalPages = mappedFacilityTypes.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMedications.ForEach(dto => CreateLinksForFacility(dto));

                return mappedFacilityTypes;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="facilityResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForFacilities(
            LinkedResourceBaseDto wrapper,
            FacilityResourceParameters facilityResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateFacilitiesResourceUri(ResourceUriType.Current, facilityResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateFacilitiesResourceUri(ResourceUriType.NextPage, facilityResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateFacilitiesResourceUri(ResourceUriType.PreviousPage, facilityResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private FacilityIdentifierDto CreateLinksForFacility<T>(T dto)
        {
            FacilityIdentifierDto identifier = (FacilityIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Facility", identifier.Id), "self", "GET"));
            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateDeleteResourceUri("Facility", identifier.Id), "self", "DELETE"));

            return identifier;
        }
    }
}
