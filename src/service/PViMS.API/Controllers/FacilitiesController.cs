using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class FacilitiesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<FacilityType> _facilityTypeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public FacilitiesController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<FacilityType> facilityTypeRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _facilityTypeRepository = facilityTypeRepository ?? throw new ArgumentNullException(nameof(facilityTypeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

            // Custom validation
            if (_facilityRepository.Queryable().Any(f => f.FacilityName == facilityForUpdate.FacilityName))
            {
                ModelState.AddModelError("Message", "A facility with the specified facility name already exists.");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(facilityForUpdate.FacilityName, @"[-a-zA-Z0-9. '()]").Count < facilityForUpdate.FacilityName.Length)
            {
                ModelState.AddModelError("Message", "Facility name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, apostrophe, round brackets)");
                return BadRequest(ModelState);
            }

            if (_facilityRepository.Queryable().Any(f => f.FacilityCode == facilityForUpdate.FacilityCode))
            {
                ModelState.AddModelError("Message", "A facility with the specified facility code already exists.");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(facilityForUpdate.FacilityCode, @"[-a-zA-Z0-9]").Count < facilityForUpdate.FacilityCode.Length)
            {
                ModelState.AddModelError("Message", "Facility code contains invalid characters (Enter A-Z, a-z, 0-9)");
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrEmpty(facilityForUpdate.ContactNumber))
            {
                if (Regex.Matches(facilityForUpdate.ContactNumber, @"[-a-zA-Z0-9]").Count < facilityForUpdate.ContactNumber.Length)
                {
                    ModelState.AddModelError("Message", "Telephone number contains invalid characters (Enter A-Z, a-z, 0-9)");
                    return BadRequest(ModelState);
                }
            }

            if (!String.IsNullOrEmpty(facilityForUpdate.MobileNumber))
            {
                if (Regex.Matches(facilityForUpdate.MobileNumber, @"[-a-zA-Z0-9]").Count < facilityForUpdate.MobileNumber.Length)
                {
                    ModelState.AddModelError("Message", "Mobile number contains invalid characters (Enter A-Z, a-z, 0-9)");
                    return BadRequest(ModelState);
                }
            }

            if (!String.IsNullOrEmpty(facilityForUpdate.FaxNumber))
            {
                if (Regex.Matches(facilityForUpdate.FaxNumber, @"[a-zA-Z0-9]").Count < facilityForUpdate.FaxNumber.Length)
                {
                    ModelState.AddModelError("Message", "Fax number contains invalid characters (Enter A-Z, a-z, 0-9)");
                    return BadRequest(ModelState);
                }
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var facilityType = _facilityTypeRepository.Get(ft => ft.Description == facilityForUpdate.FacilityType);

                var newFacility = new Facility()
                {
                    FacilityName = facilityForUpdate.FacilityName,
                    FacilityCode = facilityForUpdate.FacilityCode,
                    FacilityType = facilityType,
                    FaxNumber = facilityForUpdate.FaxNumber,
                    MobileNumber = facilityForUpdate.MobileNumber,
                    TelNumber = facilityForUpdate.ContactNumber
                };

                _facilityRepository.Save(newFacility);
                id = newFacility.Id;
            }

            var mappedFacility = await GetFacilityAsync<FacilityIdentifierDto>(id);
            if (mappedFacility == null)
            {
                return StatusCode(500, "Unable to locate newly added facility");
            }

            return CreatedAtRoute("GetFacilityByIdentifier",
                new
                {
                    id = mappedFacility.Id
                }, CreateLinksForFacility<FacilityIdentifierDto>(mappedFacility));
        }

        /// <summary>
        /// Update an existing facility
        /// </summary>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="facilityForUpdate">The facility payload</param>
        /// <returns></returns>
        [HttpPut("facilities/{id}", Name = "UpdateFacility")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateFacility(long id,
            [FromBody] FacilityForUpdateDto facilityForUpdate)
        {
            if (facilityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for facility");
                return BadRequest(ModelState);
            }

            var facilityFromRepo = await _facilityRepository.GetAsync(f => f.Id == id);
            if (facilityFromRepo == null)
            {
                return NotFound();
            }

            var type = _facilityTypeRepository.Get(ft => ft.Description == facilityForUpdate.FacilityType);

            // Custom validation
            if (_facilityRepository.Queryable().Any(f => f.Id != id && f.FacilityName == facilityForUpdate.FacilityName))
            {
                ModelState.AddModelError("Message", "Another facility with the specified name already exists.");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(facilityForUpdate.FacilityName, @"[-a-zA-Z0-9. '()]").Count < facilityForUpdate.FacilityName.Length)
            {
                ModelState.AddModelError("Message", "Facility name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, apostrophe, round brackets)");
                return BadRequest(ModelState);
            }

            if (_facilityRepository.Queryable().Any(f => f.Id != id && f.FacilityCode == facilityForUpdate.FacilityCode))
            {
                ModelState.AddModelError("Message", "Another facility with the specified code already exists.");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(facilityForUpdate.FacilityCode, @"[-a-zA-Z0-9]").Count < facilityForUpdate.FacilityCode.Length)
            {
                ModelState.AddModelError("Message", "Facility code contains invalid characters (Enter A-Z, a-z, 0-9)");
                return BadRequest(ModelState);
            }

            if (!String.IsNullOrEmpty(facilityForUpdate.ContactNumber))
            {
                if (Regex.Matches(facilityForUpdate.ContactNumber, @"[-a-zA-Z0-9]").Count < facilityForUpdate.ContactNumber.Length)
                {
                    ModelState.AddModelError("Message", "Telephone number contains invalid characters (Enter A-Z, a-z, 0-9)");
                    return BadRequest(ModelState);
                }
            }

            if (!String.IsNullOrEmpty(facilityForUpdate.MobileNumber))
            {
                if (Regex.Matches(facilityForUpdate.MobileNumber, @"[-a-zA-Z0-9]").Count < facilityForUpdate.MobileNumber.Length)
                {
                    ModelState.AddModelError("Message", "Mobile number contains invalid characters (Enter A-Z, a-z, 0-9)");
                    return BadRequest(ModelState);
                }
            }

            if (!String.IsNullOrEmpty(facilityForUpdate.FaxNumber))
            {
                if (Regex.Matches(facilityForUpdate.FaxNumber, @"[a-zA-Z0-9]").Count < facilityForUpdate.FaxNumber.Length)
                {
                    ModelState.AddModelError("Message", "Fax number contains invalid characters (Enter A-Z, a-z, 0-9)");
                    return BadRequest(ModelState);
                }
            }

            if (ModelState.IsValid)
            {
                facilityFromRepo.FacilityName = facilityForUpdate.FacilityName;
                facilityFromRepo.FacilityCode = facilityForUpdate.FacilityCode;
                facilityFromRepo.FacilityType = type;
                facilityFromRepo.FaxNumber = facilityForUpdate.FaxNumber;
                facilityFromRepo.MobileNumber = facilityForUpdate.MobileNumber;
                facilityFromRepo.TelNumber = facilityForUpdate.ContactNumber;

                _facilityRepository.Update(facilityFromRepo);
                _unitOfWork.Complete();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing facility
        /// </summary>
        /// <param name="id">The unique id of the facility</param>
        /// <returns></returns>
        [HttpDelete("facilities/{id}", Name = "DeleteFacility")]
        public async Task<IActionResult> DeleteFacility(long id)
        {
            var facilityFromRepo = await _facilityRepository.GetAsync(f => f.Id == id);
            if (facilityFromRepo == null)
            {
                return NotFound();
            }

            if (facilityFromRepo.PatientFacilities.Count > 0 || facilityFromRepo.UserFacilities.Count > 0)
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                _facilityRepository.Delete(facilityFromRepo);
                _unitOfWork.Complete();
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
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateResourceUriHelper.CreateFacilitiesResourceUri(_urlHelper, ResourceUriType.Current, facilityResourceParameters),
               "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateResourceUriHelper.CreateFacilitiesResourceUri(_urlHelper, ResourceUriType.NextPage, facilityResourceParameters),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateResourceUriHelper.CreateFacilitiesResourceUri(_urlHelper, ResourceUriType.PreviousPage, facilityResourceParameters),
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

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Facility", identifier.Id), "self", "GET"));

            // Add delete ink if available on this facility
            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateDeleteResourceUri(_urlHelper, "Facility", identifier.Id), "self", "DELETE"));

            return identifier;
        }
    }
}
