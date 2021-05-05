using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.Models;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using PVIMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class EncountersController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<DatasetElement> _datasetElementRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IPatientService _patientService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PVIMSDbContext _context;

        public EncountersController(IPropertyMappingService propertyMappingService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<DatasetElement> datasetElementRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<Attachment> attachmentRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IUnitOfWorkInt unitOfWork,
            IPatientService patientService,
            IHttpContextAccessor httpContextAccessor,
            PVIMSDbContext dbContext)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _datasetElementRepository = datasetElementRepository ?? throw new ArgumentNullException(nameof(datasetElementRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Get all encounters using a valid media type 
        /// </summary>
        /// <param name="encounterResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of EncounterDetailDto</returns>
        [HttpGet("encounters", Name = "GetEncountersByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<EncounterDetailDto>> GetEncountersByDetail(
            [FromQuery] EncounterResourceParameters encounterResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<EncounterDetailDto, Encounter>
               (encounterResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if(!String.IsNullOrWhiteSpace(encounterResourceParameters.FirstName))
            {
                if (Regex.Matches(encounterResourceParameters.FirstName, @"[-a-zA-Z ']").Count < encounterResourceParameters.FirstName.Length)
                {
                    ModelState.AddModelError("Message", "First name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
                }
            }

            if (!String.IsNullOrWhiteSpace(encounterResourceParameters.LastName))
            {
                if (Regex.Matches(encounterResourceParameters.LastName, @"[-a-zA-Z ']").Count < encounterResourceParameters.LastName.Length)
                {
                    ModelState.AddModelError("Message", "Last name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
                }
            }

            if (!String.IsNullOrWhiteSpace(encounterResourceParameters.CustomAttributeValue))
            {
                if (Regex.Matches(encounterResourceParameters.CustomAttributeValue, @"[-a-zA-Z ']").Count < encounterResourceParameters.CustomAttributeValue.Length)
                {
                    ModelState.AddModelError("Message", "Custom attribute value contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");
                }
            }

            var mappedEncountersWithLinks = GetEncounters<EncounterDetailDto>(encounterResourceParameters);

            // Add custom mappings to encounters
            mappedEncountersWithLinks.ForEach(dto => CustomEncounterMap(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<EncounterDetailDto>(mappedEncountersWithLinks.TotalCount, mappedEncountersWithLinks);
            var wrapperWithLinks = CreateLinksForEncounters(wrapper, encounterResourceParameters,
                mappedEncountersWithLinks.HasNext, mappedEncountersWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single Encounter using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Encounter</param>
        /// <returns>An ActionResult of type EncounterIdentifierDto</returns>
        [HttpGet("patients/{patientId}/encounters/{id}", Name = "GetEncounterByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<EncounterIdentifierDto>> GetEncounterByIdentifier(long patientId, long id)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return BadRequest();
            }

            var mappedEncounter = await GetEncounterAsync<EncounterIdentifierDto>(patientFromRepo.Id, id);
            if (mappedEncounter == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForEncounter<EncounterIdentifierDto>(patientFromRepo.Id, mappedEncounter));
        }

        /// <summary>
        /// Get a single Encounter using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Encounter</param>
        /// <returns>An ActionResult of type EncounterDetailDto</returns>
        [HttpGet("patients/{patientId}/encounters/{id}", Name = "GetEncounterByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<EncounterDetailDto>> GetEncounterByDetail(long patientId, long id)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return BadRequest();
            }

            var mappedEncounter = await GetEncounterAsync<EncounterDetailDto>(patientFromRepo.Id, id);
            if (mappedEncounter == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForEncounter<EncounterDetailDto>(patientFromRepo.Id, CustomEncounterMap(mappedEncounter)));
        }

        /// <summary>
        /// Get a single Encounter using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Encounter</param>
        /// <returns>An ActionResult of type EncounterExpandedDto</returns>
        [HttpGet("patients/{patientId}/encounters/{id}", Name = "GetEncounterByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<EncounterExpandedDto>> GetEncounterByExpanded(long patientId, long id)
        {
            var mappedEncounter = await GetEncounterAsync<EncounterExpandedDto>(patientId, id);
            if (mappedEncounter == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForEncounter<EncounterExpandedDto>(patientId, CustomEncounterMap(mappedEncounter)));
        }

        /// <summary>
        /// Create a new encounter record
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="encounterForCreation">The encounter payload</param>
        /// <returns></returns>
        [HttpPost("patients/{patientId}/encounters", Name = "CreateEncounter")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateEncounter(long patientId, [FromBody] EncounterForCreationDto encounterForCreation)
        {
            if (encounterForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new encounter");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(p => p.Id == patientId);
            if (patientFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate patient");
            }

            var encounterType = _encounterTypeRepository.Get(et => et.Id == encounterForCreation.EncounterTypeId);
            if (encounterType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate encounter type");
            }

            if (encounterForCreation.EncounterDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Encounter date should be before current date");
            }
            if (encounterForCreation.EncounterDate < patientFromRepo.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Encounter date should be after date of birth");
            }

            if (!String.IsNullOrEmpty(encounterForCreation.Notes))
            {
                if (Regex.Matches(encounterForCreation.Notes, @"[-a-zA-Z0-9<>/ '.]").Count < encounterForCreation.Notes.Length)
                {
                    ModelState.AddModelError("Message", "Notes contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe, period, hyphen)");
                }
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var encounterDetail = PrepareEncounterDetail(encounterForCreation);
                id = _patientService.AddEncounter(patientFromRepo, encounterDetail);
                _unitOfWork.Complete();

                var mappedEncounter = await GetEncounterAsync<EncounterIdentifierDto>(patientId, id);
                if (mappedEncounter == null)
                {
                    return StatusCode(500, "Unable to locate newly added encounter");
                }

                return CreatedAtRoute("GetEncounterByIdentifier",
                    new
                    {
                        id = mappedEncounter.Id
                    }, CreateLinksForEncounter<EncounterIdentifierDto>(patientId, mappedEncounter));
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing encounter
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the encounter</param>
        /// <param name="encounterForUpdate">The encounter payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/encounters/{id}", Name = "UpdateEncounter")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateEncounter(long patientId, long id,
            [FromBody] EncounterForUpdateDto encounterForUpdate)
        {
            if (encounterForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var encounterFromRepo = await _encounterRepository.GetAsync(e => e.Patient.Id == patientId && e.Id == id);
            if (encounterFromRepo == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(encounterForUpdate.Notes))
            {
                if (Regex.Matches(encounterForUpdate.Notes, @"[-a-zA-Z0-9<>/ '.]").Count < encounterForUpdate.Notes.Length)
                {
                    ModelState.AddModelError("Message", "Notes contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe, period, hyphen)");
                }
            }

            foreach (var element in encounterForUpdate.Elements)
            {
                var datasetElement = _datasetElementRepository.Get(de => de.Id == element.Key);
                if(datasetElement == null)
                {
                    ModelState.AddModelError("Message", $"Unable to locate dataset element {datasetElement.Id}");
                }
                else
                {
                    if(!datasetElement.System)
                    {
                        try
                        {
                            datasetElement.Validate(element.Value);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("Message", ex.Message);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                encounterFromRepo.Notes = encounterForUpdate.Notes;
                _encounterRepository.Update(encounterFromRepo);

                var contextTypeId = (int)ContextTypes.Encounter;
                var datasetInstanceFromRepo = _datasetInstanceRepository
                .Queryable()
                .Include(di => di.Dataset)
                .Include("DatasetInstanceValues.DatasetInstanceSubValues.DatasetElementSub")
                .Include("DatasetInstanceValues.DatasetElement")
                .SingleOrDefault(di => di.Dataset.ContextType.Id == contextTypeId
                        && di.ContextId == id
                        && di.EncounterTypeWorkPlan.EncounterType.Id == encounterFromRepo.EncounterType.Id);

                if (datasetInstanceFromRepo != null)
                {
                    foreach (var element in encounterForUpdate.Elements)
                    {
                        var datasetElement = _datasetElementRepository.Get(de => de.Id == element.Key);
                        if (!datasetElement.System)
                        {
                            datasetInstanceFromRepo.SetInstanceValue(datasetElement, element.Value);
                        }
                    }

                    _datasetInstanceRepository.Update(datasetInstanceFromRepo);
                }

                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Archive an existing encounter
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the encounter</param>
        /// <param name="encounterForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/encounters/{id}/archive", Name = "ArchiveEncounter")]
        [Authorize(Roles = "Clinician")]
        public async Task<IActionResult> ArchiveEncounter(long patientId, long id,
            [FromBody] ArchiveDto encounterForDelete)
        {
            var encounterFromRepo = await _encounterRepository.GetAsync(e => e.Patient.Id == patientId && e.Id == id);
            if (encounterFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(encounterForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < encounterForDelete.Reason.Length)
            {
                ModelState.AddModelError("Message", "Reason contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("Message", "Unable to locate user");
            }

            if (ModelState.IsValid)
            {
                foreach (var attachment in encounterFromRepo.Attachments.Where(x => !x.Archived))
                {
                    attachment.Archived = true;
                    attachment.ArchivedDate = DateTime.Now;
                    attachment.ArchivedReason = encounterForDelete.Reason;
                    attachment.AuditUser = user;
                    _attachmentRepository.Update(attachment);
                }

                foreach (var patientClinicalEvent in encounterFromRepo.PatientClinicalEvents.Where(x => !x.Archived))
                {
                    patientClinicalEvent.Archived = true;
                    patientClinicalEvent.ArchivedDate = DateTime.Now;
                    patientClinicalEvent.ArchivedReason = encounterForDelete.Reason;
                    patientClinicalEvent.AuditUser = user;
                    _patientClinicalEventRepository.Update(patientClinicalEvent);
                }

                encounterFromRepo.Archived = true;
                encounterFromRepo.ArchivedDate = DateTime.Now;
                encounterFromRepo.ArchivedReason = encounterForDelete.Reason;
                encounterFromRepo.AuditUser = user;
                _encounterRepository.Update(encounterFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get single Encounter from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">unique identifier of the patient </param>
        /// <param name="id">Resource guid to search by</param>
        /// <returns></returns>
        private async Task<T> GetEncounterAsync<T>(long patientId, long id) where T : class
        {
            var predicate = PredicateBuilder.New<Encounter>(true);

            // Build remaining expressions
            predicate = predicate.And(f => f.Patient.Id == patientId);
            predicate = predicate.And(f => f.Archived == false);
            predicate = predicate.And(f => f.Id == id);

            var encounterFromRepo = await _encounterRepository.GetAsync(predicate);

            if (encounterFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedEncounter = _mapper.Map<T>(encounterFromRepo);

                return mappedEncounter;
            }

            return null;
        }

        /// <summary>
        /// Get encounters from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="encounterResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetEncounters<T>(EncounterResourceParameters encounterResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = encounterResourceParameters.PageNumber,
                PageSize = encounterResourceParameters.PageSize
            };

            var facility = !String.IsNullOrWhiteSpace(encounterResourceParameters.FacilityName) ? _facilityRepository.Get(f => f.FacilityName == encounterResourceParameters.FacilityName) : null;
            var customAttribute = _customAttributeRepository.Get(ca => ca.Id == encounterResourceParameters.CustomAttributeId);
            var path = customAttribute?.CustomAttributeType == CustomAttributeType.Selection ? "CustomSelectionAttribute" : "CustomStringAttribute";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@FacilityId", facility != null ? facility.Id : 0));
            parameters.Add(new SqlParameter("@PatientId", encounterResourceParameters.PatientId.ToString()));
            parameters.Add(new SqlParameter("@FirstName", !String.IsNullOrWhiteSpace(encounterResourceParameters.FirstName) ? (Object)encounterResourceParameters.FirstName : DBNull.Value));
            parameters.Add(new SqlParameter("@LastName", !String.IsNullOrWhiteSpace(encounterResourceParameters.LastName) ? (Object)encounterResourceParameters.LastName : DBNull.Value));
            parameters.Add(new SqlParameter("@SearchFrom", encounterResourceParameters.SearchFrom > DateTime.MinValue ? (Object)encounterResourceParameters.SearchFrom : DBNull.Value));
            parameters.Add(new SqlParameter("@SearchTo", encounterResourceParameters.SearchTo < DateTime.MaxValue ? (Object)encounterResourceParameters.SearchTo : DBNull.Value));
            parameters.Add(new SqlParameter("@CustomAttributeKey", !String.IsNullOrWhiteSpace(encounterResourceParameters.CustomAttributeValue) ? (Object)customAttribute?.AttributeKey : DBNull.Value));
            parameters.Add(new SqlParameter("@CustomPath", !String.IsNullOrWhiteSpace(encounterResourceParameters.CustomAttributeValue) ? (Object)path : DBNull.Value));
            parameters.Add(new SqlParameter("@CustomValue", !String.IsNullOrWhiteSpace(encounterResourceParameters.CustomAttributeValue) ? (Object)encounterResourceParameters.CustomAttributeValue : DBNull.Value));

            var resultsFromService = PagedCollection<EncounterList>.Create(_context.EncounterLists
                .FromSqlRaw("spSearchEncounters @FacilityId, @PatientId, @FirstName, @LastName, @SearchFrom, @SearchTo, @CustomAttributeKey, @CustomPath, @CustomValue",
                        parameters.ToArray()), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedEncounters = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedEncounters.TotalCount,
                    pageSize = mappedEncounters.PageSize,
                    currentPage = mappedEncounters.CurrentPage,
                    totalPages = mappedEncounters.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedEncounters.ForEach(dto => CreateLinksForEncounter(encounterResourceParameters.PatientId, dto));

                return mappedEncounters;
            }

            return null;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private EncounterDetailDto CustomEncounterMap(EncounterDetailDto dto)
        {
            var encounterFromRepo = _encounterRepository.Get(p => p.Id == dto.Id);
            if (encounterFromRepo == null)
            {
                return dto;
            }

            dto.Patient = _mapper.Map<PatientDetailDto>(encounterFromRepo.Patient);

            // Encounter information
            var datasetInstanceFromRepo = _datasetInstanceRepository.Get(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter
                    && di.ContextId == dto.Id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounterFromRepo.EncounterType.Id);

            if (datasetInstanceFromRepo != null)
            {
                var groupedDatasetCategories = datasetInstanceFromRepo.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                dto.DatasetCategories = groupedDatasetCategories
                    .Select(dsc => new DatasetCategoryViewDto
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryName = dsc.Key.DatasetCategoryName,
                        DatasetCategoryDisplayed = ShouldCategoryBeDisplayed(encounterFromRepo, dsc.Key),
                        DatasetElements = dsc.Select(element => new DatasetElementViewDto
                        {
                            DatasetElementId = element.DatasetElement.Id,
                            DatasetElementName = element.DatasetElement.ElementName,
                            DatasetElementDisplayName = element.FriendlyName ?? element.DatasetElement.ElementName,
                            DatasetElementHelp = element.Help,
                            DatasetElementDisplayed = ShouldElementBeDisplayed(encounterFromRepo, element),
                            DatasetElementChronic = IsElementChronic(encounterFromRepo, element),
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
            }

            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private EncounterExpandedDto CustomEncounterMap(EncounterExpandedDto dto)
        {
            var encounterFromRepo = _encounterRepository.Get(p => p.Id == dto.Id);
            if (encounterFromRepo == null)
            {
                return dto;
            }

            // Encounter information
            var datasetInstanceFromRepo = _datasetInstanceRepository.Get(di => di.Dataset.ContextType.Id == (int)ContextTypes.Encounter
                    && di.ContextId == dto.Id
                    && di.EncounterTypeWorkPlan.EncounterType.Id == encounterFromRepo.EncounterType.Id);

            if (datasetInstanceFromRepo != null)
            {
                var groupedDatasetCategories = datasetInstanceFromRepo.Dataset.DatasetCategories
                    .SelectMany(dc => dc.DatasetCategoryElements).OrderBy(dc => dc.FieldOrder)
                    .GroupBy(dce => dce.DatasetCategory)
                    .ToList();

                dto.DatasetCategories = groupedDatasetCategories
                    .Select(dsc => new DatasetCategoryViewDto
                    {
                        DatasetCategoryId = dsc.Key.Id,
                        DatasetCategoryName = dsc.Key.DatasetCategoryName,
                        DatasetCategoryDisplayed = ShouldCategoryBeDisplayed(encounterFromRepo, dsc.Key),
                        DatasetElements = dsc.Select(element => new DatasetElementViewDto
                        {
                            DatasetElementId = element.DatasetElement.Id,
                            DatasetElementName = element.DatasetElement.ElementName,
                            DatasetElementDisplayName = element.FriendlyName ?? element.DatasetElement.ElementName,
                            DatasetElementHelp = element.Help,
                            DatasetElementDisplayed = ShouldElementBeDisplayed(encounterFromRepo, element),
                            DatasetElementChronic = IsElementChronic(encounterFromRepo, element),
                            DatasetElementSystem = element.DatasetElement.System,
                            DatasetElementType = element.DatasetElement.Field.FieldType.Description,
                            DatasetElementValue = datasetInstanceFromRepo.GetInstanceValue(element.DatasetElement.ElementName),
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
            }

            // Condition groups
            int[] terms = _patientConditionRepository.List(pc => pc.Patient.Id == encounterFromRepo.Patient.Id && !pc.Archived && !pc.Patient.Archived)
                .Select(p => p.TerminologyMedDra.Id)
                .ToArray();

            List<PatientConditionGroupDto> groupArray = new List<PatientConditionGroupDto>();
            foreach (var cm in _conditionMeddraRepository.List(cm => terms.Contains(cm.TerminologyMedDra.Id))
                .ToList())
            {
                var tempCondition = cm.GetConditionForPatient(encounterFromRepo.Patient);
                if (tempCondition != null)
                {
                    var group = new PatientConditionGroupDto()
                    {
                        ConditionGroup = cm.Condition.Description,
                        Status = tempCondition.OutcomeDate != null ? "Case Closed" : "Case Open",
                        PatientConditionId = tempCondition.Id,
                        StartDate = tempCondition.OnsetDate.ToString("yyyy-MM-dd"),
                        Detail = $"{tempCondition.TerminologyMedDra.DisplayName} started on {tempCondition.OnsetDate.ToString("yyyy-MM-dd")}"
                    };
                    groupArray.Add(group);
                }
            }
            dto.ConditionGroups = groupArray;

            // Weight history
            dto.WeightSeries = _patientService.GetElementValues(encounterFromRepo.Patient.Id, "Weight (kg)", 5);

            // patient custom mapping
            IExtendable patientExtended = encounterFromRepo.Patient;
            var attribute = patientExtended.GetAttributeValue("Medical Record Number");
            dto.Patient.MedicalRecordNumber = attribute != null ? attribute.ToString() : "";

            return dto;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="encounterResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForEncounters(
            LinkedResourceBaseDto wrapper,
            EncounterResourceParameters encounterResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateEncountersResourceUri(ResourceUriType.Current, encounterResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateEncountersResourceUri(ResourceUriType.NextPage, encounterResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateEncountersResourceUri(ResourceUriType.PreviousPage, encounterResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private EncounterIdentifierDto CreateLinksForEncounter<T>(long patientId, T dto)
        {
            EncounterIdentifierDto identifier = (EncounterIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateEncounterForPatientResourceUri(patientId, identifier.Id), "self", "GET"));

            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateUpdateHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "update", "PATCH"));
            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateRemoveHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "marknotcurrent", "DELETE"));

            return identifier;
        }

        /// <summary>
        /// Prepare the model for adding a new encounter
        /// </summary>
        private EncounterDetail PrepareEncounterDetail(EncounterForCreationDto encounterForCreation)
        {
            var encounterDetail = new EncounterDetail();
            encounterDetail = _mapper.Map<EncounterDetail>(encounterForCreation);

            return encounterDetail;
        }

        /// <summary>
        /// Determine if this element should be displayed
        /// </summary>
        private bool ShouldElementBeDisplayed(Encounter encounter, DatasetCategoryElement datasetCategoryElement)
        {
            if (datasetCategoryElement.Chronic)
            {
                // Does patient have chronic condition
                if (!encounter.Patient.HasCondition(datasetCategoryElement.DatasetCategoryElementConditions.Select(c => c.Condition).ToList()))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine if the category should be displayed
        /// </summary>
        private bool ShouldCategoryBeDisplayed(Encounter encounter, DatasetCategory datasetCategory)
        {
            if (datasetCategory.Chronic)
            {
                if (!encounter.Patient.HasCondition(datasetCategory.DatasetCategoryConditions.Select(c => c.Condition).ToList()))
                {
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Determine if this element is chronic in nature
        /// </summary>
        private bool IsElementChronic(Encounter encounter, DatasetCategoryElement datasetCategoryElement)
        {
            // Encounter type is chronic then element must have chronic selected and patient must have condition
            if (datasetCategoryElement.Chronic)
            {
                return !encounter.Patient.HasCondition(datasetCategoryElement.DatasetCategoryElementConditions.Select(c => c.Condition).ToList());
            }
            else
            {
                return false;
            }
        }
    }
}