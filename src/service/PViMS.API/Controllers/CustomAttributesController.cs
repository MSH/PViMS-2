using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PVIMS.API.Attributes;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Services;
using PVIMS.Core.Entities;
using VPS.Common.Collections;
using VPS.Common.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;

namespace PVIMS.API.Controllers
{
    /// <summary>
    /// A representation of all custom attributes.
    /// A custom attribute is configured to represent core entity additional values
    /// </summary>
    [Route("api/customattributes")]
    [ApiController]
    [Authorize]
    public class CustomAttributesController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public CustomAttributesController(ITypeHelperService typeHelperService,
                IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
                IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
                IMapper mapper,
                IUrlHelper urlHelper)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
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
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>> GetCustomAttributesByIdentifier(
            [FromQuery] CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CustomAttributeIdentifierDto>
                (customAttributeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedCustomAttributesWithLinks = GetCustomAttributes<CustomAttributeIdentifierDto>(customAttributeResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<CustomAttributeIdentifierDto>(mappedCustomAttributesWithLinks.TotalCount, mappedCustomAttributesWithLinks);
            var wrapperWithLinks = CreateLinksForCustomAttributes(wrapper, customAttributeResourceParameters,
                mappedCustomAttributesWithLinks.HasNext, mappedCustomAttributesWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
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
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>> GetCustomAttributesByDetail(
            [FromQuery] CustomAttributeResourceParameters customAttributeResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CustomAttributeIdentifierDto>
                (customAttributeResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedCustomAttributesWithLinks = GetCustomAttributes<CustomAttributeDetailDto>(customAttributeResourceParameters);

            // For each selection attribute, populate the list of selection values
            mappedCustomAttributesWithLinks.ForEach(dto => CreateSelectionValues(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<CustomAttributeDetailDto>(mappedCustomAttributesWithLinks.TotalCount, mappedCustomAttributesWithLinks);
            var wrapperWithLinks = CreateLinksForCustomAttributes(wrapper, customAttributeResourceParameters,
                mappedCustomAttributesWithLinks.HasNext, mappedCustomAttributesWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
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
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
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
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CustomAttributeDetailDto>> GetCustomAttributeByDetail(long id)
        {
            var mappedCustomAttribute = await GetCustomAttributeAsync<CustomAttributeDetailDto>(id);
            if (mappedCustomAttribute == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCustomAttribute<CustomAttributeDetailDto>(CreateSelectionValues(mappedCustomAttribute)));
        }

        /// <summary>
        /// Get custom attributes from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="customAttributeResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetCustomAttributes<T>(CustomAttributeResourceParameters customAttributeResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = customAttributeResourceParameters.PageNumber,
                PageSize = customAttributeResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<CustomAttributeConfiguration>(customAttributeResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<CustomAttributeConfiguration>(true);
            if (customAttributeResourceParameters.ExtendableTypeName != ExtendableTypeNames.All)
            {
                predicate = predicate.And(f => f.ExtendableTypeName == customAttributeResourceParameters.ExtendableTypeName.ToString());
            }
            if (customAttributeResourceParameters.CustomAttributeType != CustomAttributeTypes.All)
            {
                predicate = predicate.And(f => f.CustomAttributeType.ToString() == customAttributeResourceParameters.CustomAttributeType.ToString());
            }
            if (customAttributeResourceParameters.IsSearchable.HasValue)
            {
                predicate = predicate.And(f => f.IsSearchable == customAttributeResourceParameters.IsSearchable);
            }

            var pagedCustomAttributesFromRepo = _customAttributeRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedCustomAttributesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCustomAttributes = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedCustomAttributesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCustomAttributesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedCustomAttributes.TotalCount,
                    pageSize = mappedCustomAttributes.PageSize,
                    currentPage = mappedCustomAttributes.CurrentPage,
                    totalPages = mappedCustomAttributes.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedCustomAttributes.ForEach(dto => CreateLinksForCustomAttribute(dto));

                return mappedCustomAttributes;
            }

            return null;
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
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="customAttributeResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForCustomAttributes(
            LinkedResourceBaseDto wrapper,
            CustomAttributeResourceParameters customAttributeResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateCustomAttributesResourceUri(customAttributeResourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateCustomAttributesResourceUri(customAttributeResourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateCustomAttributesResourceUri(customAttributeResourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CustomAttributeIdentifierDto CreateLinksForCustomAttribute<T>(T dto)
        {
            CustomAttributeIdentifierDto identifier = (CustomAttributeIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateCustomAttributeResourceUri(identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Create a Uri for a single representation
        /// </summary>
        private string CreateCustomAttributeResourceUri(long customAttributeConfigurationId)
        {
            return _urlHelper.Link("GetCustomAttributeByIdentifier",
              new { id = customAttributeConfigurationId });
        }

        /// <summary>
        /// Create a Uri for a collection based representation
        /// </summary>
        private string CreateCustomAttributesResourceUri(
            CustomAttributeResourceParameters customAttributeResourceParameters,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber - 1,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetCustomAttributesByIdentifier",
                      new
                      {
                          orderBy = customAttributeResourceParameters.OrderBy,
                          ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                          CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                          isSearchable = customAttributeResourceParameters.IsSearchable,
                          pageNumber = customAttributeResourceParameters.PageNumber + 1,
                          pageSize = customAttributeResourceParameters.PageSize
                      });
                case ResourceUriType.Current:
                default:
                    return _urlHelper.Link("GetCustomAttributesByIdentifier",
                    new
                    {
                        orderBy = customAttributeResourceParameters.OrderBy,
                        ExtendableTypeName = customAttributeResourceParameters.ExtendableTypeName.ToString(),
                        CustomAttributeType = customAttributeResourceParameters.CustomAttributeType.ToString(),
                        isSearchable = customAttributeResourceParameters.IsSearchable,
                        pageNumber = customAttributeResourceParameters.PageNumber,
                        pageSize = customAttributeResourceParameters.PageSize
                    });
            }
        }

        /// <summary>
        /// Prepare the list of selection reference values for look up
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CustomAttributeDetailDto CreateSelectionValues(CustomAttributeDetailDto dto)
        {
            if (dto.CustomAttributeType != "Selection") { return dto; };

            dto.SelectionDataItems = _selectionDataItemRepository.List(s => s.AttributeKey == dto.AttributeKey, null, "")
                .Select(ss => new SelectionDataItemDto()
                {
                    SelectionKey = ss.SelectionKey,
                    Value = ss.Value
                })
                .ToList();

            return dto;
        }

    }
}
