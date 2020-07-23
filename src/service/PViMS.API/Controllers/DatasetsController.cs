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
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VPS.Common.Collections;
using VPS.Common.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/datasets")]
    public class DatasetsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<DatasetCategory> _datasetCategoryRepository;
        private readonly IRepositoryInt<ContextType> _contextTypeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FormHandler _formHandler;
        private readonly IUrlHelper _urlHelper;

        public DatasetsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<DatasetCategory> datasetCategoryRepository,
            IRepositoryInt<ContextType> contextTypeRepository,
            FormHandler formHandler,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _datasetCategoryRepository = datasetCategoryRepository ?? throw new ArgumentNullException(nameof(datasetCategoryRepository));
            _contextTypeRepository = contextTypeRepository ?? throw new ArgumentNullException(nameof(contextTypeRepository));
            _formHandler = formHandler ?? throw new ArgumentNullException(nameof(formHandler));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all datasets using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetIdentifierDto</returns>
        [HttpGet(Name = "GetDatasetsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<DatasetIdentifierDto>> GetDatasetsByIdentifier(
            [FromQuery] IdResourceParameters datasetResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetIdentifierDto>
                (datasetResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedDatasetsWithLinks = GetDatasets<DatasetIdentifierDto>(datasetResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetIdentifierDto>(mappedDatasetsWithLinks.TotalCount, mappedDatasetsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all datasets using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetDetailDto</returns>
        [HttpGet(Name = "GetDatasetsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<DatasetDetailDto>> GetDatasetsByDetail(
            [FromQuery] IdResourceParameters datasetResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetDetailDto>
                (datasetResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedDatasetsWithLinks = GetDatasets<DatasetDetailDto>(datasetResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetDetailDto>(mappedDatasetsWithLinks.TotalCount, mappedDatasetsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get all dataset categories using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DatasetCategoryDetailDto</returns>
        [HttpGet("{datasetId}/categories", Name = "GetDatasetCategoriesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<DatasetCategoryDetailDto>>> GetDatasetCategoriesByDetail(long datasetId,
            [FromQuery] IdResourceParameters datasetCategoryResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DatasetCategoryDetailDto>
                (datasetCategoryResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            var mappedDatasetCategoriesWithLinks = GetDatasetCategories<DatasetCategoryDetailDto>(datasetId, datasetCategoryResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<DatasetCategoryDetailDto>(mappedDatasetCategoriesWithLinks.TotalCount, mappedDatasetCategoriesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, datasetResourceParameters,
            //    mappedDatasetsWithLinks.HasNext, mappedDatasetsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single dataset using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the dataset</param>
        /// <returns>An ActionResult of type DatasetIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetDatasetByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetIdentifierDto>> GetDatasetByIdentifier(long id)
        {
            var mappedDataset = await GetDatasetAsync<DatasetIdentifierDto>(id);
            if (mappedDataset == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDataset<DatasetIdentifierDto>(mappedDataset));
        }

        /// <summary>
        /// Get a single dataset using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique detail for the dataset</param>
        /// <returns>An ActionResult of type DatasetDetailDto</returns>
        [HttpGet("{id}", Name = "GetDatasetByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetDetailDto>> GetDatasetByDetail(long id)
        {
            var mappedDataset = await GetDatasetAsync<DatasetDetailDto>(id);
            if (mappedDataset == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDataset<DatasetDetailDto>(mappedDataset));
        }

        /// <summary>
        /// Get a single dataset category using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the dataset category</param>
        /// <returns>An ActionResult of type DatasetCategoryIdentifierDto</returns>
        [HttpGet("{datasetId}/categories/{id}", Name = "GetDatasetCategoryByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetCategoryIdentifierDto>> GetDatasetCategoryByIdentifier(long datasetId, long id)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            var mappedDatasetCategory = await GetDatasetCategoryAsync<DatasetCategoryIdentifierDto>(datasetId, id);
            if (mappedDatasetCategory == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetCategory<DatasetCategoryIdentifierDto>(datasetId, mappedDatasetCategory));
        }

        /// <summary>
        /// Get a single dataset instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the instance</param>
        /// <returns>An ActionResult of type DatasetInstanceIdentifierDto</returns>
        [HttpGet("{datasetId}/instances/{id}", Name = "GetDatasetInstanceByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<DatasetInstanceIdentifierDto>> GetDatasetInstanceByIdentifier(long datasetId, long id)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return BadRequest();
            }

            var mappedDatasetInstance = await GetDatasetInstanceAsync<DatasetInstanceIdentifierDto>(datasetId, id);
            if (mappedDatasetInstance == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetInstance<DatasetInstanceIdentifierDto>(datasetFromRepo.Id, mappedDatasetInstance));
        }

        /// <summary>
        /// Get a single dataset instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="id">The unique identifier for the instance</param>
        /// <returns>An ActionResult of type DatasetInstanceDetailDto</returns>
        [HttpGet("{datasetId}/instances/{id}", Name = "GetDatasetInstanceByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<DatasetInstanceDetailDto>> GetDatasetInstanceByDetail(long datasetId, long id)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return BadRequest();
            }

            var mappedDatasetInstance = await GetDatasetInstanceAsync<DatasetInstanceDetailDto>(datasetId, id);
            if (mappedDatasetInstance == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForDatasetInstance<DatasetInstanceDetailDto>(datasetFromRepo.Id, CustomDatasetInstanceMap(mappedDatasetInstance)));
        }

        /// <summary>
        /// Create a new dataset
        /// </summary>
        /// <param name="datasetForUpdate">The dataset payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateDataset")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDataset(
            [FromBody] DatasetForUpdateDto datasetForUpdate)
        {
            if (datasetForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetForUpdate.DatasetName, @"[a-zA-Z ']").Count < datasetForUpdate.DatasetName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(datasetForUpdate.Help))
            {
                if (Regex.Matches(datasetForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            var contextType = await _contextTypeRepository.GetAsync(ct => ct.Description == "Encounter");
            if (contextType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate context type");
            }

            if (_unitOfWork.Repository<Dataset>().Queryable().
                Where(l => l.DatasetName == datasetForUpdate.DatasetName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newDataset = new Dataset()
                {
                    DatasetName = datasetForUpdate.DatasetName,
                    Help = datasetForUpdate.Help,
                    ContextType = contextType,
                    Active = true
                };

                _datasetRepository.Save(newDataset);
                id = newDataset.Id;

                var mappedDataset = await GetDatasetAsync<DatasetIdentifierDto>(id);
                if (mappedDataset == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtRoute("GetDatasetByIdentifier",
                    new
                    {
                        id = mappedDataset.Id
                    }, CreateLinksForDataset<DatasetIdentifierDto>(mappedDataset));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new dataset category
        /// </summary>
        /// <param name="datasetId">The unique identifier for the dataset</param>
        /// <param name="datasetCategoryForUpdate">The dataset category payload</param>
        /// <returns></returns>
        [HttpPost("{datasetId}/categories", Name = "CreateDatasetCategory")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDatasetCategory(long datasetId, 
            [FromBody] DatasetCategoryForUpdateDto datasetCategoryForUpdate)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            if (datasetCategoryForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetCategoryForUpdate.DatasetCategoryName, @"[a-zA-Z ']").Count < datasetCategoryForUpdate.DatasetCategoryName.Length)
            {
                ModelState.AddModelError("Message", "Category name contains invalid characters (Enter A-Z, a-z)");
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryForUpdate.FriendlyName))
            {
                if (Regex.Matches(datasetCategoryForUpdate.FriendlyName, @"[a-zA-Z0-9. ']").Count < datasetCategoryForUpdate.FriendlyName.Length)
                {
                    ModelState.AddModelError("Message", "Friendly name contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (!String.IsNullOrWhiteSpace(datasetCategoryForUpdate.Help))
            {
                if (Regex.Matches(datasetCategoryForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetCategoryForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<DatasetCategory>().Queryable().
                Where(l => l.Dataset.Id == datasetId && l.DatasetCategoryName == datasetCategoryForUpdate.DatasetCategoryName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newDatasetCategory = new DatasetCategory()
                {
                    CategoryOrder = (short)GetNextCategoryOrder(datasetFromRepo),
                    Dataset = datasetFromRepo,
                    DatasetCategoryName = datasetCategoryForUpdate.DatasetCategoryName,
                    System = false,
                    Acute = (datasetCategoryForUpdate.Acute == Models.ValueTypes.YesNoValueType.Yes),
                    Chronic = (datasetCategoryForUpdate.Chronic == Models.ValueTypes.YesNoValueType.Yes),
                    Public = false,
                    FriendlyName = datasetCategoryForUpdate.FriendlyName,
                    Help = datasetCategoryForUpdate.Help
                };

                _datasetCategoryRepository.Save(newDatasetCategory);
                id = newDatasetCategory.Id;

                var mappedDatasetCategory = await GetDatasetCategoryAsync<DatasetCategoryIdentifierDto>(datasetId, id);
                if (mappedDatasetCategory == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtRoute("GetDatasetCategoryByIdentifier",
                    new
                    {
                        datasetId,
                        id = mappedDatasetCategory.Id
                    }, CreateLinksForDatasetCategory<DatasetCategoryIdentifierDto>(datasetId, mappedDatasetCategory));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing dataset
        /// </summary>
        /// <param name="id">The unique id of the dataset</param>
        /// <param name="datasetForUpdate">The dataset payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateDataset")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateDataset(long id,
            [FromBody] DatasetForUpdateDto datasetForUpdate)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == id);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            if (datasetForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(datasetForUpdate.DatasetName, @"[a-zA-Z ']").Count < datasetForUpdate.DatasetName.Length)
            {
                ModelState.AddModelError("Message", "Dataset name contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (!String.IsNullOrWhiteSpace(datasetForUpdate.Help))
            {
                if (Regex.Matches(datasetForUpdate.Help, @"[a-zA-Z0-9. ']").Count < datasetForUpdate.Help.Length)
                {
                    ModelState.AddModelError("Message", "Help contains invalid characters (Enter A-Z, a-z, 0-9, period)");
                }
            }

            if (_unitOfWork.Repository<Dataset>().Queryable().
                Where(l => l.DatasetName == datasetForUpdate.DatasetName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                datasetFromRepo.DatasetName = datasetForUpdate.DatasetName;
                datasetFromRepo.Help = datasetForUpdate.Help;

                _datasetRepository.Update(datasetFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }
            
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing dataset
        /// </summary>
        /// <param name="id">The unique id of the dataset</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteDataset")]
        public async Task<IActionResult> DeleteDataset(long id)
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == id);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            //if (datasetFromRepo.WorkPlanDatasets.Count > 0)
            //{
            //    ModelState.AddModelError("Message", "Unable to delete as item is in use.");
            //    return BadRequest(ModelState);
            //}

            if (ModelState.IsValid)
            {
                _datasetRepository.Delete(datasetFromRepo);
                _unitOfWork.Complete();
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new dataset instance
        /// </summary>
        /// <param name="datasetId">The unique identifier of the dataset that the instance is being created for</param>
        /// <param name="formValues">The dataset instance payload</param>
        /// <returns></returns>
        [HttpPut("{datasetId}/instances", Name = "CreateDatasetInstance")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateDatasetInstance(long datasetId, [FromBody] Object[] formValues)
        {
            if (formValues == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new form");
                return BadRequest(ModelState);
            }

            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == datasetId);
            if (datasetFromRepo == null)
            {
                return NotFound();
            }

            if(datasetFromRepo.DatasetName != "Spontaneous Report")
            {
                ModelState.AddModelError("Message", "Can only generate instance of type spontaneous report");
                return BadRequest(ModelState);
            }

            _formHandler.SetSpontaneousForm(formValues);

            if (ModelState.IsValid)
            {
                _formHandler.ProcessSpontaneousFormForCreation(datasetFromRepo);

                return Ok();
            }

            return BadRequest(ModelState);
        }


        /// <summary>
        /// Get datasets from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetDatasets<T>(IdResourceParameters datasetResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = datasetResourceParameters.PageNumber,
                PageSize = datasetResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Dataset>(datasetResourceParameters.OrderBy, "asc");

            var pagedDatasetsFromRepo = _datasetRepository.List(pagingInfo, null, orderby, "");
            if (pagedDatasetsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasets = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedDatasetsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDatasetsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedDatasets.TotalCount,
                    pageSize = mappedDatasets.PageSize,
                    currentPage = mappedDatasets.CurrentPage,
                    totalPages = mappedDatasets.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedDatasets.ForEach(dto => CreateLinksForDataset(dto));

                return mappedDatasets;
            }

            return null;
        }

        /// <summary>
        /// Get dataset categories from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="datasetCategoryResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetDatasetCategories<T>(long datasetId, IdResourceParameters datasetCategoryResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = datasetCategoryResourceParameters.PageNumber,
                PageSize = datasetCategoryResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<DatasetCategory>("CategoryOrder", "asc");

            var predicate = PredicateBuilder.New<DatasetCategory>(true);
            predicate = predicate.And(f => f.Dataset.Id == datasetId);

            var pagedDatasetCategoriesFromRepo = _datasetCategoryRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedDatasetCategoriesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetCategories = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedDatasetCategoriesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedDatasetCategoriesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedDatasetCategories.TotalCount,
                    pageSize = mappedDatasetCategories.PageSize,
                    currentPage = mappedDatasetCategories.CurrentPage,
                    totalPages = mappedDatasetCategories.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedDatasetCategories.ForEach(dto => CreateLinksForDatasetCategory(datasetId, dto));

                return mappedDatasetCategories;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetAsync<T>(long id) where T : class
        {
            var datasetFromRepo = await _datasetRepository.GetAsync(f => f.Id == id);

            if (datasetFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDataset = _mapper.Map<T>(datasetFromRepo);

                return mappedDataset;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset instance from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetInstanceAsync<T>(long datasetId, long id) where T : class
        {
            var datasetInstanceFromRepo = await _datasetInstanceRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == id);

            if (datasetInstanceFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetInstance = _mapper.Map<T>(datasetInstanceFromRepo);

                return mappedDatasetInstance;
            }

            return null;
        }

        /// <summary>
        /// Get single dataset category from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetDatasetCategoryAsync<T>(long datasetId, long id) where T : class
        {
            var datasetCategoryFromRepo = await _datasetCategoryRepository.GetAsync(f => f.Dataset.Id == datasetId && f.Id == id);

            if (datasetCategoryFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedDatasetCategory = _mapper.Map<T>(datasetCategoryFromRepo);

                return mappedDatasetCategory;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetIdentifierDto CreateLinksForDataset<T>(T dto)
        {
            DatasetIdentifierDto identifier = (DatasetIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Dataset", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="datasetId">Unique id of the dataset being queried for instances</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetInstanceIdentifierDto CreateLinksForDatasetInstance<T>(long datasetId, T dto)
        {
            DatasetInstanceIdentifierDto identifier = (DatasetInstanceIdentifierDto)(object)dto;

            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "DatasetInstance", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="datasetId">Unique id of the dataset being queried for categories</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetCategoryIdentifierDto CreateLinksForDatasetCategory<T>(long datasetId, T dto)
        {
            DatasetCategoryIdentifierDto identifier = (DatasetCategoryIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateDatasetCategoryResourceUri(_urlHelper, "DatasetCategory", datasetId, identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Get the next category order number
        /// </summary>
        /// <param name="dataset">The dataset that is being queried to determine the order number for the category</param>
        /// <returns></returns>
        private int GetNextCategoryOrder(Dataset dataset)
        {
            return dataset.DatasetCategories.Count == 0 ? 1 : _unitOfWork.Repository<DatasetCategory>().Queryable().Where(dc => dc.Dataset.Id == dataset.Id).OrderByDescending(dc => dc.CategoryOrder).First().CategoryOrder + 1;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private DatasetInstanceDetailDto CustomDatasetInstanceMap(DatasetInstanceDetailDto dto)
        {
            var datasetInstanceFromRepo = _datasetInstanceRepository.Get(di => di.Id == dto.Id);
            if (datasetInstanceFromRepo == null)
            {
                return dto;
            }

            var groupedDatasetCategories = datasetInstanceFromRepo.Dataset.DatasetCategories
                .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                .GroupBy(dce => dce.DatasetCategory)
                .ToList();

            dto.DatasetCategories = groupedDatasetCategories
                .Select(dsc => new DatasetCategoryViewDto
                {
                    DatasetCategoryId = dsc.Key.Id,
                    DatasetCategoryName = dsc.Key.DatasetCategoryName,
                    DatasetCategoryDisplayed = true,
                    DatasetElements = dsc.Select(element => new DatasetElementViewDto
                    {
                        DatasetElementId = element.DatasetElement.Id,
                        DatasetElementName = element.DatasetElement.ElementName,
                        DatasetElementDisplayName = element.FriendlyName ?? element.DatasetElement.ElementName,
                        DatasetElementHelp = element.Help,
                        DatasetElementDisplayed = true,
                        DatasetElementChronic = false,
                        DatasetElementSystem = element.DatasetElement.System,
                        DatasetElementType = element.DatasetElement.Field.FieldType.Description,
                        DatasetElementValue = datasetInstanceFromRepo.GetInstanceValue(element.DatasetElement.ElementName),
                        StringMaxLength = element.DatasetElement.Field.MaxLength,
                        NumericMinValue = element.DatasetElement.Field.MinSize,
                        NumericMaxValue = element.DatasetElement.Field.MaxSize,
                        Required = element.DatasetElement.Field.Mandatory,
                        SelectionDataItems = element.DatasetElement.Field.FieldValues.Select(fv => new SelectionDataItemDto() { SelectionKey = fv.Value, Value = fv.Value }).ToList(),
                        DatasetElementSubs = element.DatasetElement.DatasetElementSubs.Select(elementSub => new DatasetElementSubViewDto
                        {
                            DatasetElementSubId = elementSub.Id,
                            DatasetElementSubName = elementSub.ElementName,
                            DatasetElementSubType = elementSub.Field.FieldType.Description
                        }).ToArray()
                    })
                    .ToArray()
                })
                .ToArray();

            return dto;
        }
    }
}
