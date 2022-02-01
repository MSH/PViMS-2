using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Application.Queries.CustomAttributeAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Repositories;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    /// <summary>
    /// A representation of all custom attributes.
    /// A custom attribute is configured to represent core entity additional values
    /// </summary>
    [Route("api/customattributes")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class CustomAttributesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ILogger<CustomAttributesController> _logger;

        public CustomAttributesController(
            IMediator mediator, 
            ITypeHelperService typeHelperService,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ILogger<CustomAttributesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all custom attributes using a valid media type 
        /// </summary>
        /// <param name="customAttributeResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CustomAttributeIdentifierDto</returns>
        [HttpGet(Name = "GetCustomAttributesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>>> GetCustomAttributesByIdentifier(
            [FromQuery] CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CustomAttributeIdentifierDto>
                (customAttributeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CustomAttributesIdentifierQuery(
                customAttributeResourceParameters.OrderBy,
                customAttributeResourceParameters.ExtendableTypeName,
                customAttributeResourceParameters.CustomAttributeType,
                customAttributeResourceParameters.IsSearchable,
                customAttributeResourceParameters.PageNumber,
                customAttributeResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CustomAttributesIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = customAttributeResourceParameters.PageSize,
                currentPage = customAttributeResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all custom attributes using a valid media type 
        /// </summary>
        /// <param name="customAttributeResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CustomAttributeDetailDto</returns>
        [HttpGet(Name = "GetCustomAttributesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>>> GetCustomAttributesByDetail(
            [FromQuery] CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CustomAttributeIdentifierDto>
                (customAttributeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new CustomAttributesDetailQuery(
                customAttributeResourceParameters.OrderBy,
                customAttributeResourceParameters.ExtendableTypeName,
                customAttributeResourceParameters.CustomAttributeType,
                customAttributeResourceParameters.IsSearchable,
                customAttributeResourceParameters.PageNumber,
                customAttributeResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: CustomAttributesDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = customAttributeResourceParameters.PageSize,
                currentPage = customAttributeResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single custom attribute using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the custom attribute</param>
        /// <returns>An ActionResult of type CustomAttributeIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetCustomAttributeByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<CustomAttributeIdentifierDto>> GetCustomAttributeByIdentifier(long id)
        {
            var mappedCustomAttribute = await GetCustomAttributeAsync<CustomAttributeIdentifierDto>(id);
            if (mappedCustomAttribute == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCustomAttribute<CustomAttributeIdentifierDto>(mappedCustomAttribute));
        }

        /// <summary>
        /// Get a single custom attribute using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the custom attribute</param>
        /// <returns>An ActionResult of type CustomAttributeDetailDto</returns>
        [HttpGet("{id}", Name = "GetCustomAttributeByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CustomAttributeDetailDto>> GetCustomAttributeByDetail(long id)
        {
            var mappedCustomAttribute = await GetCustomAttributeAsync<CustomAttributeDetailDto>(id);
            if (mappedCustomAttribute == null)
            {
                return NotFound();
            }

            //return Ok(CreateLinksForCustomAttribute<CustomAttributeDetailDto>(CreateSelectionValues(mappedCustomAttribute)));
            return Ok();
        }

        /// <summary>
        /// Get single custom attribute from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource guid to search by</param>
        /// <returns></returns>
        private async Task<T> GetCustomAttributeAsync<T>(long id) where T : class
        {
            var customAttributeFromRepo = await _customAttributeRepository.GetAsync(f => f.Id == id);

            if (customAttributeFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCustomAttribute = _mapper.Map<T>(customAttributeFromRepo);

                return mappedCustomAttribute;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CustomAttributeIdentifierDto CreateLinksForCustomAttribute<T>(T dto)
        {
            CustomAttributeIdentifierDto identifier = (CustomAttributeIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("CustomAttribute", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
