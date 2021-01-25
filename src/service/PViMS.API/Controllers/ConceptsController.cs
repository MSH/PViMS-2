using System;
using System.Linq;
using System.Text.RegularExpressions;
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
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ConceptsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<MedicationForm> _medicationFormRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public ConceptsController(IPropertyMappingService propertyMappingService, 
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<Product> productRepository,
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<MedicationForm> medicationFormRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _medicationFormRepository = medicationFormRepository ?? throw new ArgumentNullException(nameof(medicationFormRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all concepts using a valid media type 
        /// </summary>
        /// <param name="conceptResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConceptIdentifierDto</returns>
        [HttpGet("concepts", Name = "GetConceptsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<ConceptIdentifierDto>> GetConceptsByIdentifier(
            [FromQuery] ConceptResourceParameters conceptResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConceptIdentifierDto>
                (conceptResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedConceptsWithLinks = GetConcepts<ConceptIdentifierDto>(conceptResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ConceptIdentifierDto>(mappedConceptsWithLinks.TotalCount, mappedConceptsWithLinks);
            var wrapperWithLinks = CreateLinksForConcepts(wrapper, conceptResourceParameters,
                mappedConceptsWithLinks.HasNext, mappedConceptsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all concepts using a valid media type 
        /// </summary>
        /// <param name="conceptResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ConceptDetailDto</returns>
        [HttpGet("concepts", Name = "GetConceptsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<ConceptDetailDto>> GetConceptsByDetail(
            [FromQuery] ConceptResourceParameters conceptResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ConceptDetailDto>
                (conceptResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedConceptsWithLinks = GetConcepts<ConceptDetailDto>(conceptResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ConceptDetailDto>(mappedConceptsWithLinks.TotalCount, mappedConceptsWithLinks);
            var wrapperWithLinks = CreateLinksForConcepts(wrapper, conceptResourceParameters,
                mappedConceptsWithLinks.HasNext, mappedConceptsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all products using a valid media type 
        /// </summary>
        /// <param name="productResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ProductIdentifierDto</returns>
        [HttpGet("products", Name = "GetProductsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<ProductIdentifierDto>> GetProductsByIdentifier(
            [FromQuery] ProductResourceParameters productResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ProductIdentifierDto>
                (productResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedProductsWithLinks = GetProducts<ProductIdentifierDto>(productResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ProductIdentifierDto>(mappedProductsWithLinks.TotalCount, mappedProductsWithLinks);
            var wrapperWithLinks = CreateLinksForProducts(wrapper, productResourceParameters,
                mappedProductsWithLinks.HasNext, mappedProductsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all products using a valid media type 
        /// </summary>
        /// <param name="productResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ProductDetailDto</returns>
        [HttpGet("products", Name = "GetProductsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<ProductDetailDto>> GetProductsByDetail(
            [FromQuery] ProductResourceParameters productResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ProductDetailDto>
                (productResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedProductsWithLinks = GetProducts<ProductDetailDto>(productResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ProductDetailDto>(mappedProductsWithLinks.TotalCount, mappedProductsWithLinks);
            var wrapperWithLinks = CreateLinksForProducts(wrapper, productResourceParameters,
                mappedProductsWithLinks.HasNext, mappedProductsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get all medication forms using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of MedicationFormIdentifierDto</returns>
        [HttpGet("medicationforms", Name = "GetMedicationFormsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>> GetMedicationFormsByIdentifier(
            [FromQuery] MedicationFormResourceParameters medicationFormResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<MedicationFormIdentifierDto, MedicationForm>
               (medicationFormResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedMedicationFormsWithLinks = GetMedicationForms<MedicationFormIdentifierDto>(medicationFormResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<MedicationFormIdentifierDto>(mappedMedicationFormsWithLinks.TotalCount, mappedMedicationFormsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, medicationResourceParameters,
            //    mappedMedicationsWithLinks.HasNext, mappedMedicationsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single concept using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the concept</param>
        /// <returns>An ActionResult of type ConceptIdentifierDto</returns>
        [HttpGet("concepts/{id}", Name = "GetConceptByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ConceptIdentifierDto>> GetConceptByIdentifier(long id)
        {
            var mappedConcept = await GetConceptAsync<ConceptIdentifierDto>(id);
            if (mappedConcept == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForConcept<ConceptIdentifierDto>(mappedConcept));
        }

        /// <summary>
        /// Get a single product using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the product</param>
        /// <returns>An ActionResult of type ProductIdentifierDto</returns>
        [HttpGet("products/{id}", Name = "GetProductByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ProductIdentifierDto>> GetProductByIdentifier(long id)
        {
            var mappedProduct = await GetProductAsync<ProductIdentifierDto>(id);
            if (mappedProduct == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForProduct<ProductIdentifierDto>(mappedProduct));
        }

        /// <summary>
        /// Get a single product using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the product</param>
        /// <returns>An ActionResult of type ProgramDetailDto</returns>
        [HttpGet("products/{id}", Name = "GetProductByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ProductDetailDto>> GetProductByDetail(long id)
        {
            var mappedProduct = await GetProductAsync<ProductDetailDto>(id);
            if (mappedProduct == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForProduct<ProductDetailDto>(mappedProduct));
        }

        /// <summary>
        /// Create a new concept
        /// </summary>
        /// <param name="conceptForUpdate">The concept payload</param>
        /// <returns></returns>
        [HttpPost("concepts", Name = "CreateConcept")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateConcept(
            [FromBody] ConceptForUpdateDto conceptForUpdate)
        {
            if (conceptForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(conceptForUpdate.ConceptName, @"[a-zA-Z0-9 .,()%/]").Count < conceptForUpdate.ConceptName.Length)
            {
                ModelState.AddModelError("Message", "Concept name contains invalid characters (Enter A-Z, a-z, 0-9, space, period, comma, brackets, percentage, forward slash)");
            }

            if (Regex.Matches(conceptForUpdate.MedicationForm, @"[-a-zA-Z0-9 ,]").Count < conceptForUpdate.MedicationForm.Length)
            {
                ModelState.AddModelError("Message", "Medication form contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, comma)");
            }

            var medicationFormFromRepo = await _medicationFormRepository.GetAsync(f => f.Description == conceptForUpdate.MedicationForm);
            if (medicationFormFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate medication form");
            }

            if (_unitOfWork.Repository<Concept>().Queryable().
                Where(l => l.ConceptName == conceptForUpdate.ConceptName && l.MedicationForm.Id == medicationFormFromRepo.Id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var newConcept = new Concept()
                {
                    ConceptName = conceptForUpdate.ConceptName,
                    MedicationForm = medicationFormFromRepo,
                    Active = (conceptForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes)
                };

                _conceptRepository.Save(newConcept);

                var mappedConcept = await GetConceptAsync<ConceptIdentifierDto>(newConcept.Id);
                if (mappedConcept == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtRoute("GetConceptByIdentifier",
                    new
                    {
                        id = mappedConcept.Id
                    }, CreateLinksForConcept<ConceptIdentifierDto>(mappedConcept));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing concept
        /// </summary>
        /// <param name="id">The unique id of the concept</param>
        /// <param name="conceptForUpdate">The concept payload</param>
        /// <returns></returns>
        [HttpPut("concepts/{id}", Name = "UpdateConcept")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateConcept(long id,
            [FromBody] ConceptForUpdateDto conceptForUpdate)
        {
            if (conceptForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(conceptForUpdate.ConceptName, @"[-a-zA-Z0-9 .,()%/]").Count < conceptForUpdate.ConceptName.Length)
            {
                ModelState.AddModelError("Message", "Concept name contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage, forward slash)");
            }

            if (Regex.Matches(conceptForUpdate.MedicationForm, @"[-a-zA-Z0-9 ,]").Count < conceptForUpdate.MedicationForm.Length)
            {
                ModelState.AddModelError("Message", "Medication form contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, comma)");
            }

            var medicationFormFromRepo = await _medicationFormRepository.GetAsync(f => f.Description == conceptForUpdate.MedicationForm);
            if (medicationFormFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate medication form");
            }

            if (_unitOfWork.Repository<Concept>().Queryable().
                Where(l => l.ConceptName == conceptForUpdate.ConceptName && l.MedicationForm.Id == medicationFormFromRepo.Id && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var conceptFromRepo = await _conceptRepository.GetAsync(f => f.Id == id);
            if (conceptFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                conceptFromRepo.ConceptName = conceptForUpdate.ConceptName;
                conceptFromRepo.MedicationForm = medicationFormFromRepo;
                conceptFromRepo.Active = (conceptForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

                _conceptRepository.Update(conceptFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing concept
        /// </summary>
        /// <param name="id">The unique id of the concept</param>
        /// <returns></returns>
        [HttpDelete("concepts/{id}", Name = "DeleteConcept")]
        public async Task<IActionResult> DeleteConcept(long id)
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(f => f.Id == id);
            if (conceptFromRepo == null)
            {
                return NotFound();
            }

            if (_unitOfWork.Repository<Product>().Queryable().Any(p => p.Concept.Id == id))
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
            }

            if (ModelState.IsValid)
            {
                _conceptRepository.Delete(conceptFromRepo);
                _unitOfWork.Complete();

                return NoContent();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="productForUpdate">The product payload</param>
        /// <returns></returns>
        [HttpPost("products", Name = "CreateProduct")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] ProductForUpdateDto productForUpdate)
        {
            if (productForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(productForUpdate.ConceptName, @"[-a-zA-Z0-9 .,()%/]").Count < productForUpdate.ConceptName.Length)
            {
                ModelState.AddModelError("Message", "Concept name contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage, forward slash)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(productForUpdate.MedicationForm, @"[-a-zA-Z0-9 ,]").Count < productForUpdate.MedicationForm.Length)
            {
                ModelState.AddModelError("Message", "Medication form contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, comma)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(productForUpdate.ProductName, @"[-a-zA-Z0-9 .,()%]").Count < productForUpdate.ProductName.Length)
            {
                ModelState.AddModelError("Message", "Product name contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(productForUpdate.Manufacturer, @"[-a-zA-Z0-9 .,()%]").Count < productForUpdate.Manufacturer.Length)
            {
                ModelState.AddModelError("Message", "Manufacturer contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<Product>().Queryable().
                Where(l => l.Concept.ConceptName == productForUpdate.ConceptName && l.ProductName == productForUpdate.ProductName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var medicationFormFromRepo = await _medicationFormRepository.GetAsync(f => f.Description == productForUpdate.MedicationForm);
            if (medicationFormFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate medication form");
                return BadRequest(ModelState);
            }

            var conceptFromRepo = await _conceptRepository.GetAsync(f => f.ConceptName == productForUpdate.ConceptName && f.MedicationForm.Id == medicationFormFromRepo.Id);

            if (ModelState.IsValid)
            {
                // Create concept first if necessary
                if (conceptFromRepo == null)
                {
                    conceptFromRepo = new Concept()
                    {
                        ConceptName = productForUpdate.ConceptName,
                        MedicationForm = medicationFormFromRepo,
                        Active = (productForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes)
                    };
                    _conceptRepository.Save(conceptFromRepo);
                }

                var newProduct = new Product()
                {
                    ProductName = productForUpdate.ProductName,
                    Concept = conceptFromRepo,
                    Manufacturer = productForUpdate.Manufacturer,
                    Description = productForUpdate.Description,
                    Active = (productForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes)
                };

                _productRepository.Save(newProduct);

                var mappedProduct = await GetProductAsync<ProductIdentifierDto>(newProduct.Id);
                if (mappedProduct == null)
                {
                    return StatusCode(500, "Unable to locate newly added item");
                }

                return CreatedAtRoute("GetProductByIdentifier",
                    new
                    {
                        id = mappedProduct.Id
                    }, CreateLinksForProduct<ProductIdentifierDto>(mappedProduct));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id">The unique id of the product</param>
        /// <param name="productForUpdate">The product payload</param>
        /// <returns></returns>
        [HttpPut("products/{id}", Name = "UpdateProduct")]
        [Consumes("application/json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(long id,
            [FromBody] ProductForUpdateDto productForUpdate)
        {
            if (productForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(productForUpdate.ConceptName, @"[-a-zA-Z0-9 .,()%/]").Count < productForUpdate.ConceptName.Length)
            {
                ModelState.AddModelError("Message", "Concept name contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage, forward slash)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(productForUpdate.MedicationForm, @"[-a-zA-Z0-9 ,]").Count < productForUpdate.MedicationForm.Length)
            {
                ModelState.AddModelError("Message", "Medication form contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, comma)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(productForUpdate.ProductName, @"[-a-zA-Z0-9 .,()%]").Count < productForUpdate.ProductName.Length)
            {
                ModelState.AddModelError("Message", "Product name contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage)");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(productForUpdate.Manufacturer, @"[-a-zA-Z0-9 .,()%]").Count < productForUpdate.Manufacturer.Length)
            {
                ModelState.AddModelError("Message", "Manufacturer contains invalid characters (Enter A-Z, a-z, 0-9, space, hyphen, period, comma, brackets, percentage)");
                return BadRequest(ModelState);
            }

            var medicationFormFromRepo = await _medicationFormRepository.GetAsync(f => f.Description == productForUpdate.MedicationForm);
            if (medicationFormFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate medication form");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<Product>().Queryable().
                Where(l => l.Concept.ConceptName == productForUpdate.ConceptName && l.ProductName == productForUpdate.ProductName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            var conceptFromRepo = await _conceptRepository.GetAsync(f => f.ConceptName == productForUpdate.ConceptName && f.MedicationForm.Id == medicationFormFromRepo.Id);

            var productFromRepo = await _productRepository.GetAsync(f => f.Id == id);
            if (productFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Create concept first if necessary
                if (conceptFromRepo == null)
                {
                    conceptFromRepo = new Concept()
                    {
                        ConceptName = productForUpdate.ConceptName,
                        MedicationForm = medicationFormFromRepo,
                        Active = (productForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes)
                    };
                    _conceptRepository.Save(conceptFromRepo);
                }

                productFromRepo.ProductName = productForUpdate.ProductName;
                productFromRepo.Concept = conceptFromRepo;
                productFromRepo.Manufacturer = productForUpdate.Manufacturer;
                productFromRepo.Description = productForUpdate.Description;
                productFromRepo.Active = (productForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);

                _productRepository.Update(productFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing product
        /// </summary>
        /// <param name="id">The unique id of the product</param>
        /// <returns></returns>
        [HttpDelete("products/{id}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            var productFromRepo = await _productRepository.GetAsync(f => f.Id == id);
            if (productFromRepo == null)
            {
                return NotFound();
            }

            if (_unitOfWork.Repository<ConditionMedication>().Queryable().Any(cm => cm.Product.Id == id) ||
                _unitOfWork.Repository<PatientMedication>().Queryable().Any(pm => pm.Product.Id == id))
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                _productRepository.Delete(productFromRepo);
                _unitOfWork.Complete();
            }

            return NoContent();
        }

        /// <summary>
        /// Get single product from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetProductAsync<T>(long id) where T : class
        {
            var productFromRepo = await _productRepository.GetAsync(f => f.Id == id);

            if (productFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedProduct = _mapper.Map<T>(productFromRepo);

                return mappedProduct;
            }

            return null;
        }

        /// <summary>
        /// Get single concept from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetConceptAsync<T>(long id) where T : class
        {
            var conceptFromRepo = await _conceptRepository.GetAsync(f => f.Id == id);

            if (conceptFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConcept = _mapper.Map<T>(conceptFromRepo);

                return mappedConcept;
            }

            return null;
        }

        /// <summary>
        /// Get concepts from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="conceptResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetConcepts<T>(ConceptResourceParameters conceptResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = conceptResourceParameters.PageNumber,
                PageSize = conceptResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Concept>(conceptResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<Concept>(true);
            if (!String.IsNullOrWhiteSpace(conceptResourceParameters.SearchTerm))
            {
                predicate = predicate.And(f => f.ConceptName.Contains(conceptResourceParameters.SearchTerm.Trim()));
            }
            if (conceptResourceParameters.Active != Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (conceptResourceParameters.Active == Models.ValueTypes.YesNoBothValueType.Yes));
            }

            var pagedConceptsFromRepo = _conceptRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedConceptsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedConcepts = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedConceptsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedConceptsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedConcepts.TotalCount,
                    pageSize = mappedConcepts.PageSize,
                    currentPage = mappedConcepts.CurrentPage,
                    totalPages = mappedConcepts.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedConcepts.ForEach(dto => CreateLinksForConcept(dto));

                return mappedConcepts;
            }

            return null;
        }

        /// <summary>
        /// Get products from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="productResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetProducts<T>(ProductResourceParameters productResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = productResourceParameters.PageNumber,
                PageSize = productResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Product>(productResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<Product>(true);
            if(productResourceParameters.Active != Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (productResourceParameters.Active == Models.ValueTypes.YesNoBothValueType.Yes));
            }

            if (!String.IsNullOrWhiteSpace(productResourceParameters.SearchTerm))
            {
                predicate = predicate.And(f => f.ProductName.Contains(productResourceParameters.SearchTerm.Trim()) ||
                        f.Concept.ConceptName.Contains(productResourceParameters.SearchTerm.Trim()));
            }
            if (productResourceParameters.Active != Models.ValueTypes.YesNoBothValueType.Both)
            {
                predicate = predicate.And(f => f.Active == (productResourceParameters.Active == Models.ValueTypes.YesNoBothValueType.Yes));
            }

            var pagedProductsFromRepo = _productRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedProductsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedProducts = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedProductsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedProductsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedProducts.TotalCount,
                    pageSize = mappedProducts.PageSize,
                    currentPage = mappedProducts.CurrentPage,
                    totalPages = mappedProducts.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedProducts.ForEach(dto => CreateLinksForProduct(dto));

                return mappedProducts;
            }

            return null;
        }

        /// <summary>
        /// Get medication forms from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="medicationFormResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetMedicationForms<T>(MedicationFormResourceParameters medicationFormResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = medicationFormResourceParameters.PageNumber,
                PageSize = medicationFormResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<MedicationForm>(medicationFormResourceParameters.OrderBy, "asc");

            var pagedMedicationFormsFromRepo = _medicationFormRepository.List(pagingInfo, null, orderby, "");
            if (pagedMedicationFormsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedMedicationForms = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedMedicationFormsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedMedicationFormsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedMedicationForms.TotalCount,
                    pageSize = mappedMedicationForms.PageSize,
                    currentPage = mappedMedicationForms.CurrentPage,
                    totalPages = mappedMedicationForms.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedMedications.ForEach(dto => CreateLinksForFacility(dto));

                return mappedMedicationForms;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="conceptResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForConcepts(
            LinkedResourceBaseDto wrapper,
            ConceptResourceParameters conceptResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateResourceUriHelper.CreateConceptsResourceUri(_urlHelper, ResourceUriType.Current, conceptResourceParameters),
               "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateResourceUriHelper.CreateConceptsResourceUri(_urlHelper, ResourceUriType.NextPage, conceptResourceParameters),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateResourceUriHelper.CreateConceptsResourceUri(_urlHelper, ResourceUriType.PreviousPage, conceptResourceParameters),
                    "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="productResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForProducts(
            LinkedResourceBaseDto wrapper,
            ProductResourceParameters productResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateResourceUriHelper.CreateProductsResourceUri(_urlHelper, ResourceUriType.Current, productResourceParameters),
               "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateResourceUriHelper.CreateProductsResourceUri(_urlHelper, ResourceUriType.NextPage, productResourceParameters),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateResourceUriHelper.CreateProductsResourceUri(_urlHelper, ResourceUriType.PreviousPage, productResourceParameters),
                    "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private ProductIdentifierDto CreateLinksForProduct<T>(T dto)
        {
            ProductIdentifierDto identifier = (ProductIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Product", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private ConceptIdentifierDto CreateLinksForConcept<T>(T dto)
        {
            ConceptIdentifierDto identifier = (ConceptIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Concept", identifier.Id), "self", "GET"));

            return identifier;
        }

    }
}
