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
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/conditions")]
    [Authorize]
    public class ConditionsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Condition> _conditionRepository;
        private readonly IRepositoryInt<ConditionLabTest> _conditionLabTestRepository;
        private readonly IRepositoryInt<ConditionMedication> _conditionMedicationRepository;
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<LabTest> _labTestRepository;
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IUnitOfWorkInt _unitOfWork;

        public ConditionsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<Condition> conditionRepository,
            IRepositoryInt<ConditionLabTest> conditionLabTestRepository,
            IRepositoryInt<ConditionMedication> conditionMedicationRepository,
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<LabTest> labTestRepository,
            IRepositoryInt<Product> productRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _conditionRepository = conditionRepository ?? throw new ArgumentNullException(nameof(conditionRepository));
            _conditionLabTestRepository = conditionLabTestRepository ?? throw new ArgumentNullException(nameof(conditionLabTestRepository));
            _conditionMedicationRepository = conditionMedicationRepository ?? throw new ArgumentNullException(nameof(conditionMedicationRepository));
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _labTestRepository = labTestRepository ?? throw new ArgumentNullException(nameof(labTestRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all conditions using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConditionIdentifierDto</returns>
        [HttpGet(Name = "GetConditionsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<ConditionIdentifierDto>> GetConditionsByIdentifier(
            [FromQuery] ConditionResourceParameters conditionResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConditionIdentifierDto>
                (conditionResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedConditionsWithLinks = GetConditions<ConditionIdentifierDto>(conditionResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ConditionIdentifierDto>(mappedConditionsWithLinks.TotalCount, mappedConditionsWithLinks);
            var wrapperWithLinks = CreateLinksForConditions(wrapper, conditionResourceParameters,
                mappedConditionsWithLinks.HasNext, mappedConditionsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all conditions using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConditionDetailDto</returns>
        [HttpGet(Name = "GetConditionsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<ConditionDetailDto>> GetConditionsByDetail(
            [FromQuery] ConditionResourceParameters conditionResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConditionDetailDto>
                (conditionResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedConditionsWithLinks = GetConditions<ConditionDetailDto>(conditionResourceParameters);

            // Add custom mappings to conditions
            mappedConditionsWithLinks.ForEach(dto => CustomConditionMap(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<ConditionDetailDto>(mappedConditionsWithLinks.TotalCount, mappedConditionsWithLinks);
            var wrapperWithLinks = CreateLinksForConditions(wrapper, conditionResourceParameters,
                mappedConditionsWithLinks.HasNext, mappedConditionsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single condition unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the condition</param>
        /// <returns>An ActionResult of type ConditionIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetConditionByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ConditionIdentifierDto>> GetConditionByIdentifier(int id)
        {
            var mappedCondition = await GetConditionAsync<ConditionIdentifierDto>(id);
            if (mappedCondition == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCondition<ConditionIdentifierDto>(mappedCondition));
        }

        /// <summary>
        /// Get a single condition unique ID and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the condition</param>
        /// <returns>An ActionResult of type ConditionDetailDto</returns>
        [HttpGet("{id}", Name = "GetConditionByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ConditionDetailDto>> GetConditionByDetail(int id)
        {
            var mappedCondition = await GetConditionAsync<ConditionDetailDto>(id);
            if (mappedCondition == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCondition<ConditionDetailDto>(mappedCondition));
        }

        /// <summary>
        /// Create a new condition
        /// </summary>
        /// <param name="conditionForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateCondition")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCondition(
            [FromBody] ConditionForUpdateDto conditionForUpdate)
        {
            if (conditionForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(conditionForUpdate.ConditionName, @"[a-zA-Z0-9 ]").Count < conditionForUpdate.ConditionName.Length)
            {
                ModelState.AddModelError("Message", "Condition contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if(conditionForUpdate.ConditionMedDras.Count == 0)
            {
                ModelState.AddModelError("Message", "Condition must contain at least one MedDra term");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<Condition>().Queryable().
                Where(l => l.Description == conditionForUpdate.ConditionName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var newCondition = new Condition()
                {
                    Description = conditionForUpdate.ConditionName,
                    Chronic = (conditionForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes),
                    Active = (conditionForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes)
                };

                // ensure lab tests are linked to condition
                foreach (var labTestId in conditionForUpdate.ConditionLabTests)
                {
                    newCondition.ConditionLabTests.Add(new ConditionLabTest() { LabTest = _labTestRepository.Get(f => f.Id == labTestId) });
                }

                // ensure products are linked to condition
                foreach (var productId in conditionForUpdate.ConditionMedications)
                {
                    var product = _productRepository.Get(f => f.Id == productId);
                    newCondition.ConditionMedications.Add(new ConditionMedication() { Product = product, Concept = product.Concept });
                }

                // ensure meddra terms are linked to condition
                foreach (var terminologyMeddraId in conditionForUpdate.ConditionMedDras)
                {
                    newCondition.ConditionMedDras.Add(new ConditionMedDra() { TerminologyMedDra = _terminologyMeddraRepository.Get(f => f.Id == terminologyMeddraId) });
                }

                _conditionRepository.Save(newCondition);
                _unitOfWork.Complete();

                var mappedCondition = await GetConditionAsync<ConditionIdentifierDto>(newCondition.Id);
                if (mappedCondition == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtRoute("GetConditionByIdentifier",
                    new
                    {
                        id = mappedCondition.Id
                    }, CreateLinksForCondition<ConditionIdentifierDto>(mappedCondition));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing condition
        /// </summary>
        /// <param name="id">The unique id of the condition</param>
        /// <param name="conditionForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateCondition")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCondition(int id,
            [FromBody] ConditionForUpdateDto conditionForUpdate)
        {
            if (conditionForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(conditionForUpdate.ConditionName, @"[a-zA-Z0-9 ]").Count < conditionForUpdate.ConditionName.Length)
            {
                ModelState.AddModelError("Message", "Condition contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                return BadRequest(ModelState);
            }

            if (conditionForUpdate.ConditionMedDras.Count == 0)
            {
                ModelState.AddModelError("Message", "Condition must contain at least one MedDra term");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<Condition>().Queryable().
                Where(l => l.Description == conditionForUpdate.ConditionName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var conditionFromRepo = await _conditionRepository.GetAsync(f => f.Id == id);
            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                conditionFromRepo.Description = conditionForUpdate.ConditionName;
                conditionFromRepo.Chronic = (conditionForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes);
                conditionFromRepo.Active = (conditionForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

                _conditionRepository.Update(conditionFromRepo);

                AddOrUpdateConditionLabTests(conditionForUpdate, conditionFromRepo);
                AddOrUpdateConditionMeddras(conditionForUpdate, conditionFromRepo);
                AddOrUpdateConditionMedications(conditionForUpdate, conditionFromRepo);

                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing condition group
        /// </summary>
        /// <param name="id">The unique id of the condition group</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteCondition")]
        public async Task<IActionResult> DeleteCondition(int id)
        {
            var conditionFromRepo = await _conditionRepository.GetAsync(f => f.Id == id);
            if (conditionFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var labTestValues = _unitOfWork.Repository<ConditionLabTest>().Queryable().Where(c => c.Condition.Id == id).ToList();
                labTestValues.ForEach(conditionLabTest => _conditionLabTestRepository.Delete(conditionLabTest));

                var medicationValues = _unitOfWork.Repository<ConditionMedication>().Queryable().Where(c => c.Condition.Id == id).ToList();
                medicationValues.ForEach(conditionMedication => _conditionMedicationRepository.Delete(conditionMedication));

                var meddraValues = _unitOfWork.Repository<ConditionMedDra>().Queryable().Where(c => c.Condition.Id == id).ToList();
                meddraValues.ForEach(conditionMedication => _conditionMeddraRepository.Delete(conditionMedication));

                _conditionRepository.Delete(conditionFromRepo);
                _unitOfWork.Complete();
            }

            return NoContent();
        }

        /// <summary>
        /// Get conditions from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="conditionResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetConditions<T>(ConditionResourceParameters conditionResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = conditionResourceParameters.PageNumber,
                PageSize = conditionResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Condition>(conditionResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<Condition>(true);
            if (conditionResourceParameters.Active != Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (conditionResourceParameters.Active == Models.ValueTypes.YesNoBothValueType.Yes));
            }

            var pagedConditionsFromRepo = _conditionRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedConditionsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConditions = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedConditionsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedConditionsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedConditions.TotalCount,
                    pageSize = mappedConditions.PageSize,
                    currentPage = mappedConditions.CurrentPage,
                    totalPages = mappedConditions.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedConditions.ForEach(dto => CreateLinksForCondition(dto));

                return mappedConditions;
            }

            return null;
        }

        /// <summary>
        /// Get single condition from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetConditionAsync<T>(int id) where T : class
        {
            var conditionFromRepo = await _conditionRepository.GetAsync(f => f.Id == id);

            if (conditionFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCondition = _mapper.Map<T>(conditionFromRepo);

                return mappedCondition;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="conditionResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForConditions(
            LinkedResourceBaseDto wrapper,
            ConditionResourceParameters conditionResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateResourceUriHelper.CreateConditionsResourceUri(_urlHelper, ResourceUriType.Current, conditionResourceParameters),
               "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateResourceUriHelper.CreateConditionsResourceUri(_urlHelper, ResourceUriType.NextPage, conditionResourceParameters),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateResourceUriHelper.CreateConditionsResourceUri(_urlHelper, ResourceUriType.PreviousPage, conditionResourceParameters),
                    "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private ConditionIdentifierDto CreateLinksForCondition<T>(T dto)
        {
            ConditionIdentifierDto identifier = (ConditionIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Condition", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private ConditionDetailDto CustomConditionMap(ConditionDetailDto dto)
        {
            var cohortGroupsFromRepo = _cohortGroupRepository.List(cg => cg.Condition.Id == dto.Id);
            dto.CohortGroups = _mapper.Map<PagedCollection<CohortGroupIdentifierDto>>(cohortGroupsFromRepo);

            return dto;
        }

        /// <summary>
        ///  Handle the updating of lab tests for an existing condition
        /// </summary>
        /// <param name="conditionForUpdate">The payload containing the list of lab tests</param>
        /// <param name="conditionFromRepo">The condition entity that is being updated</param>
        /// <returns></returns>
        private void AddOrUpdateConditionLabTests(ConditionForUpdateDto conditionForUpdate, Condition conditionFromRepo)
        {
            // Determine what has been removed
            ArrayList deleteCollection = new ArrayList();
            foreach (var conditionLabTest in conditionFromRepo.ConditionLabTests)
            {
                if (!conditionForUpdate.ConditionLabTests.Contains(conditionLabTest.LabTest.Id))
                {
                    deleteCollection.Add(conditionLabTest);
                }
            }

            // Process deletes
            foreach (var conditionLabTest in deleteCollection)
            {
                _conditionLabTestRepository.Delete(conditionLabTest);
            }

            // Determine what needs to be added
            foreach (var labTestId in conditionForUpdate.ConditionLabTests)
            {
                if (!conditionFromRepo.ConditionLabTests.Any(c => c.LabTest.Id == labTestId))
                {
                    var newConditionLabTest = new ConditionLabTest() { Condition = conditionFromRepo, LabTest = _labTestRepository.Get(f => f.Id == labTestId) };
                    _conditionLabTestRepository.Save(newConditionLabTest);
                }
            }
        }

        /// <summary>
        ///  Handle the updating of medications for an existing condition
        /// </summary>
        /// <param name="conditionForUpdate">The payload containing the list of lab tests</param>
        /// <param name="conditionFromRepo">The condition entity that is being updated</param>
        /// <returns></returns>
        private void AddOrUpdateConditionMedications(ConditionForUpdateDto conditionForUpdate, Condition conditionFromRepo)
        {
            // Determine what has been removed
            ArrayList deleteCollection = new ArrayList();
            foreach (var conditionMedication in conditionFromRepo.ConditionMedications)
            {
                if (!conditionForUpdate.ConditionMedications.Contains(conditionMedication.Product.Id))
                {
                    deleteCollection.Add(conditionMedication);
                }
            }
            // Process deletes
            foreach (var conditionMedication in deleteCollection)
            {
                _conditionMedicationRepository.Delete(conditionMedication);
            }

            // Determine what needs to be added
            foreach (var productId in conditionForUpdate.ConditionMedications)
            {
                if (!conditionFromRepo.ConditionMedications.Any(c => c.Product.Id == productId))
                {
                    var product = _productRepository.Get(f => f.Id == productId);
                    var newConditionMedication = new ConditionMedication() { Condition = conditionFromRepo, Product = product, Concept = product.Concept };
                    _conditionMedicationRepository.Save(newConditionMedication);
                }
            }
        }

        /// <summary>
        ///  Handle the updating of terminology for an existing condition
        /// </summary>
        /// <param name="conditionForUpdate">The payload containing the list of lab tests</param>
        /// <param name="conditionFromRepo">The condition entity that is being updated</param>
        /// <returns></returns>
        private void AddOrUpdateConditionMeddras(ConditionForUpdateDto conditionForUpdate, Condition conditionFromRepo)
        {
            // Determine what has been removed
            ArrayList deleteCollection = new ArrayList();
            foreach (var conditionMeddra in conditionFromRepo.ConditionMedDras)
            {
                if (!conditionForUpdate.ConditionMedDras.Contains(conditionMeddra.TerminologyMedDra.Id))
                {
                    deleteCollection.Add(conditionMeddra);
                }
            }
            // Process deletes
            foreach (var conditionMeddra in deleteCollection)
            {
                _conditionMeddraRepository.Delete(conditionMeddra);
            }

            // Determine what needs to be added
            foreach (var meddraId in conditionForUpdate.ConditionMedDras)
            {
                if (!conditionFromRepo.ConditionMedDras.Any(c => c.TerminologyMedDra.Id == meddraId))
                {
                    var newConditionMedra = new ConditionMedDra() { Condition = conditionFromRepo, TerminologyMedDra = _terminologyMeddraRepository.Get(f => f.Id == meddraId) };
                    _conditionMeddraRepository.Save(newConditionMedra);
                }
            }
        }
    }
}
