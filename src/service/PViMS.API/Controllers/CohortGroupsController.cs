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
    [Route("api/cohortgroups")]
    [Authorize]
    public class CohortGroupsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<Condition> _conditionRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public CohortGroupsController(ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<Condition> conditionRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _conditionRepository = conditionRepository ?? throw new ArgumentNullException(nameof(conditionRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all cohort groups using a valid media type 
        /// </summary>
        /// <param name="cohortGroupResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CohortGroupIdentifierDto</returns>
        [HttpGet(Name = "GetCohortGroupsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>> GetCohortGroupsByIdentifier(
            [FromQuery] CohortGroupResourceParameters cohortGroupResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CohortGroupIdentifierDto>
                (cohortGroupResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedCohortGroupsWithLinks = GetCohortGroups<CohortGroupIdentifierDto>(cohortGroupResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<CohortGroupIdentifierDto>(mappedCohortGroupsWithLinks.TotalCount, mappedCohortGroupsWithLinks);
            var wrapperWithLinks = CreateLinksForCohortGroups(wrapper, cohortGroupResourceParameters,
                mappedCohortGroupsWithLinks.HasNext, mappedCohortGroupsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all cohort groups using a valid media type 
        /// </summary>
        /// <param name="cohortGroupResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CohortGroupDetailDto</returns>
        [HttpGet(Name = "GetCohortGroupsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>> GetCohortGroupsByDetail(
            [FromQuery] CohortGroupResourceParameters cohortGroupResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CohortGroupDetailDto>
                (cohortGroupResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedCohortGroupsWithLinks = GetCohortGroups<CohortGroupDetailDto>(cohortGroupResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<CohortGroupDetailDto>(mappedCohortGroupsWithLinks.TotalCount, mappedCohortGroupsWithLinks);
            var wrapperWithLinks = CreateLinksForCohortGroups(wrapper, cohortGroupResourceParameters,
                mappedCohortGroupsWithLinks.HasNext, mappedCohortGroupsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single cohort group unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the cohort group</param>
        /// <returns>An ActionResult of type CohortGroupIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetCohortGroupByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<CohortGroupIdentifierDto>> GetCohortGroupByIdentifier(int id)
        {
            var mappedCohortGroup = await GetCohortGroupAsync<CohortGroupIdentifierDto>(id);
            if (mappedCohortGroup == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCohortGroup<CohortGroupIdentifierDto>(mappedCohortGroup));
        }

        /// <summary>
        /// Get a single facility using it's unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the cohort group</param>
        /// <returns>An ActionResult of type CohortGroupDetailDto</returns>
        [HttpGet("{id}", Name = "GetCohortGroupByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<CohortGroupDetailDto>> GetCohortGroupByDetail(int id)
        {
            var mappedCohortGroup = await GetCohortGroupAsync<CohortGroupDetailDto>(id);
            if (mappedCohortGroup == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCohortGroup<CohortGroupDetailDto>(mappedCohortGroup));
        }

        /// <summary>
        /// Create a new cohort group
        /// </summary>
        /// <param name="cohortGroupForUpdate">The cohort group payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateCohortGroup")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCohortGroup(
            [FromBody] CohortGroupForUpdateDto cohortGroupForUpdate)
        {
            if (cohortGroupForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(cohortGroupForUpdate.CohortName, @"[a-zA-Z0-9 ']").Count < cohortGroupForUpdate.CohortName.Length)
            {
                ModelState.AddModelError("Message", "Cohort name contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(cohortGroupForUpdate.CohortCode, @"[-a-zA-Z0-9 ]").Count < cohortGroupForUpdate.CohortCode.Length)
            {
                ModelState.AddModelError("Message", "Cohort code contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen)");
                return BadRequest(ModelState);
            }

            if (cohortGroupForUpdate.StartDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Start Date should be before current date");
            }

            if (cohortGroupForUpdate.FinishDate.HasValue)
            {
                if (cohortGroupForUpdate.FinishDate < cohortGroupForUpdate.StartDate)
                {
                    ModelState.AddModelError("Message", "Finish Date should be after Start Date");
                }
            }

            Condition conditionFromRepo = null;
            conditionFromRepo = _conditionRepository.Get(c => c.Description == cohortGroupForUpdate.ConditionName);
            if (conditionFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate primary condition group");
            }

            if (_unitOfWork.Repository<CohortGroup>().Queryable().
                Where(l => l.CohortName == cohortGroupForUpdate.CohortName || l.CohortCode == cohortGroupForUpdate.CohortCode)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var newCohortGroup = new CohortGroup()
                {
                    CohortName = cohortGroupForUpdate.CohortName,
                    CohortCode = cohortGroupForUpdate.CohortCode,
                    Condition = conditionFromRepo,
                    StartDate = cohortGroupForUpdate.StartDate,
                    FinishDate = cohortGroupForUpdate.FinishDate,
                    MaxEnrolment = 0,
                    MinEnrolment = 0, 
                    LastPatientNo = 0
                };

                _cohortGroupRepository.Save(newCohortGroup);

                var mappedCohortGroup = await GetCohortGroupAsync<CohortGroupIdentifierDto>(newCohortGroup.Id);
                if (mappedCohortGroup == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtRoute("GetCohortGroupByIdentifier",
                    new
                    {
                        id = mappedCohortGroup.Id
                    }, CreateLinksForCohortGroup<CohortGroupIdentifierDto>(mappedCohortGroup));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing cohort group
        /// </summary>
        /// <param name="id">The unique id of the cohort group</param>
        /// <param name="cohortGroupForUpdate">The cohort group payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateCohortGroup")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCohortGroup(long id,
            [FromBody] CohortGroupForUpdateDto cohortGroupForUpdate)
        {
            if (cohortGroupForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(f => f.Id == id);
            if (cohortGroupFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(cohortGroupForUpdate.CohortName, @"[a-zA-Z0-9 ']").Count < cohortGroupForUpdate.CohortName.Length)
            {
                ModelState.AddModelError("Message", "Cohort name contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(cohortGroupForUpdate.CohortCode, @"[-a-zA-Z0-9 ]").Count < cohortGroupForUpdate.CohortCode.Length)
            {
                ModelState.AddModelError("Message", "Cohort code contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen)");
                return BadRequest(ModelState);
            }

            if (cohortGroupForUpdate.StartDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Start Date should be before current date");
            }

            if (cohortGroupForUpdate.FinishDate.HasValue)
            {
                if (cohortGroupForUpdate.FinishDate < cohortGroupForUpdate.StartDate)
                {
                    ModelState.AddModelError("Message", "Finish Date should be after Start Date");
                }
            }

            Condition conditionFromRepo = null;
            conditionFromRepo = _conditionRepository.Get(c => c.Description == cohortGroupForUpdate.ConditionName);
            if (conditionFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate primary condition group");
            }

            if (_unitOfWork.Repository<CohortGroup>().Queryable().
                Where(l => (l.CohortName == cohortGroupForUpdate.CohortName || l.CohortCode == cohortGroupForUpdate.CohortCode) && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                cohortGroupFromRepo.CohortName = cohortGroupForUpdate.CohortName;
                cohortGroupFromRepo.CohortCode = cohortGroupForUpdate.CohortCode;
                cohortGroupFromRepo.StartDate = cohortGroupForUpdate.StartDate;
                cohortGroupFromRepo.FinishDate = cohortGroupForUpdate.FinishDate;
                cohortGroupFromRepo.Condition = conditionFromRepo;

                _cohortGroupRepository.Update(cohortGroupFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing cohort group
        /// </summary>
        /// <param name="id">The unique id of the cohort group</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteCohortGroup")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCohortGroup(long id)
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(f => f.Id == id);
            if (cohortGroupFromRepo == null)
            {
                return NotFound();
            }

            if (_cohortGroupEnrolmentRepository.Exists(cge => cge.CohortGroup.Id == id))
            {
                ModelState.AddModelError("Message", "Unable to delete the Cohort Group as it is currently in use");
            }

            if (ModelState.IsValid)
            {
                _cohortGroupRepository.Delete(cohortGroupFromRepo);
                _unitOfWork.Complete();
            }

            return NoContent();
        }

        /// <summary>
        /// Get single cohort group from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetCohortGroupAsync<T>(int id) where T : class
        {
            var cohortGroupFromRepo = await _cohortGroupRepository.GetAsync(f => f.Id == id);

            if (cohortGroupFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroup = _mapper.Map<T>(cohortGroupFromRepo);

                return mappedCohortGroup;
            }

            return null;
        }

        /// <summary>
        /// Get cohort groups from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="cohortGroupResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetCohortGroups<T>(CohortGroupResourceParameters cohortGroupResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = cohortGroupResourceParameters.PageNumber,
                PageSize = cohortGroupResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<CohortGroup>(cohortGroupResourceParameters.OrderBy, "asc");

            var pagedCohortGroupsFromRepo = _cohortGroupRepository.List(pagingInfo, null, orderby, "");
            if (pagedCohortGroupsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCohortGroups = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedCohortGroupsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCohortGroupsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedCohortGroups.TotalCount,
                    pageSize = mappedCohortGroups.PageSize,
                    currentPage = mappedCohortGroups.CurrentPage,
                    totalPages = mappedCohortGroups.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedCohortGroups.ForEach(dto => CreateLinksForCohortGroup(dto));

                return mappedCohortGroups;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CohortGroupIdentifierDto CreateLinksForCohortGroup<T>(T dto)
        {
            CohortGroupIdentifierDto identifier = (CohortGroupIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "CohortGroup", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="cohortGroupResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForCohortGroups(
            LinkedResourceBaseDto wrapper,
            CohortGroupResourceParameters cohortGroupResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateResourceUriHelper.CreateCohortGroupsResourceUri(_urlHelper, ResourceUriType.Current, cohortGroupResourceParameters),
               "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateResourceUriHelper.CreateCohortGroupsResourceUri(_urlHelper, ResourceUriType.NextPage, cohortGroupResourceParameters),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateResourceUriHelper.CreateCohortGroupsResourceUri(_urlHelper, ResourceUriType.PreviousPage, cohortGroupResourceParameters),
                    "previousPage", "GET"));
            }

            return wrapper;
        }

    }
}
