using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Role> _roleRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public RolesController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<Role> roleRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all roles using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of RoleIdentifierDto</returns>
        [HttpGet("roles", Name = "GetRolesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<RoleIdentifierDto>> GetRolesByIdentifier(
            [FromQuery] IdResourceParameters baseResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<RoleIdentifierDto>
                (baseResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedRolesWithLinks = GetRoles<RoleIdentifierDto>(baseResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<RoleIdentifierDto>(mappedRolesWithLinks.TotalCount, mappedRolesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, roleResourceParameters,
            //    mappedRolesWithLinks.HasNext, mappedRolesWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single role using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab result</param>
        /// <returns>An ActionResult of type RoleIdentifierDto</returns>
        [HttpGet("roles/{id}", Name = "GetRoleByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<RoleIdentifierDto>> GetRoleByIdentifier(long id)
        {
            var mappedRole = await GetRoleAsync<RoleIdentifierDto>(id);
            if (mappedRole == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForRole<RoleIdentifierDto>(mappedRole));
        }

        /// <summary>
        /// Get roles from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="baseResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetRoles<T>(IdResourceParameters baseResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = baseResourceParameters.PageNumber,
                PageSize = baseResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Role>(baseResourceParameters.OrderBy, "asc");

            var pagedRolesFromRepo = _roleRepository.List(pagingInfo, null, orderby, "");
            if (pagedRolesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedRoles = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedRolesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedRolesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedRoles.TotalCount,
                    pageSize = mappedRoles.PageSize,
                    currentPage = mappedRoles.CurrentPage,
                    totalPages = mappedRoles.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedRoles.ForEach(dto => CreateLinksForRole(dto));

                return mappedRoles;
            }

            return null;
        }

        /// <summary>
        /// Get single role from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetRoleAsync<T>(long id) where T : class
        {
            var roleFromRepo = await _roleRepository.GetAsync(f => f.Id == id);

            if (roleFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedRole = _mapper.Map<T>(roleFromRepo);

                return mappedRole;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private RoleIdentifierDto CreateLinksForRole<T>(T dto)
        {
            RoleIdentifierDto identifier = (RoleIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Role", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
