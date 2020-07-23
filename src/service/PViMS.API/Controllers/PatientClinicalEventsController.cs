using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PVIMS.API.Attributes;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Services;
using PVIMS.Core.Entities;
using PVIMS.Core.Models;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using VPS.Common.Repositories;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize]
    public class PatientClinicalEventsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Core.Entities.CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<Core.Entities.SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IWorkFlowService _workFlowService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientClinicalEventsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<Core.Entities.CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<Core.Entities.SelectionDataItem> selectionDataItemRepository,
            IWorkFlowService workFlowService,
            IUnitOfWorkInt unitOfWork,
            ICustomAttributeService customAttributeService,
            IHttpContextAccessor httpContextAccessor)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventIdentifierDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientClinicalEventIdentifierDto>> GetPatientClinicalEventByIdentifier(long patientId, long id)
        {
            var mappedPatientClinicalEvent = await GetPatientClinicalEventAsync<PatientClinicalEventIdentifierDto>(patientId, id);
            if (mappedPatientClinicalEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientClinicalEvent<PatientClinicalEventIdentifierDto>(mappedPatientClinicalEvent));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventDetailDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientClinicalEventDetailDto>> GetPatientClinicalEventByDetail(long patientId, long id)
        {
            var mappedPatientClinicalEvent = await GetPatientClinicalEventAsync<PatientClinicalEventDetailDto>(patientId, id);
            if (mappedPatientClinicalEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientClinicalEvent<PatientClinicalEventDetailDto>(CustomPatientClinicalEventMap(mappedPatientClinicalEvent)));
        }

        /// <summary>
        /// Get a single patient clinical event using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient clinical event</param>
        /// <returns>An ActionResult of type PatientClinicalEventExpandedDto</returns>
        [HttpGet("{patientId}/clinicalevents/{id}", Name = "GetPatientClinicalEventByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientClinicalEventExpandedDto>> GetPatientClinicalEventByExpanded(long patientId, long id)
        {
            var mappedPatientClinicalEvent = await GetPatientClinicalEventAsync<PatientClinicalEventExpandedDto>(patientId, id);
            if (mappedPatientClinicalEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientClinicalEvent<PatientClinicalEventExpandedDto>(CustomPatientClinicalEventMap(mappedPatientClinicalEvent)));
        }

        /// <summary>
        /// Create a new patient clinical event record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="clinicalEventForUpdate">The clinical event payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/clinicalevents", Name = "CreatePatientClinicalEvent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientClinicalEvent(int patientId, 
            [FromBody] PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            if (clinicalEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new clinical event");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var sourceTermFromRepo = _terminologyMeddraRepository.Get(clinicalEventForUpdate.SourceTerminologyMedDraId);
            if (sourceTermFromRepo == null)
            {
                return BadRequest();
            }

            ValidateClinicalEventForUpdateModel(patientFromRepo, clinicalEventForUpdate, 0);

            if (ModelState.IsValid)
            {
                var clinicalEventDetail = PrepareClinicalEventDetail(clinicalEventForUpdate);
                if (!clinicalEventDetail.IsValid())
                {
                    clinicalEventDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    var patientClinicalEvent = new PatientClinicalEvent
                    {
                        SourceDescription = clinicalEventForUpdate.SourceDescription,
                        SourceTerminologyMedDra = sourceTermFromRepo,
                        OnsetDate = clinicalEventForUpdate.OnsetDate,
                        ResolutionDate = clinicalEventForUpdate.ResolutionDate,
                        Patient = patientFromRepo
                    };

                    _modelExtensionBuilder.UpdateExtendable(patientClinicalEvent, clinicalEventDetail.CustomAttributes, "Admin");

                    _patientClinicalEventRepository.Save(patientClinicalEvent);

                    _workFlowService.CreateWorkFlowInstance("New Active Surveilliance Report", 
                        patientClinicalEvent.PatientClinicalEventGuid, 
                        patientFromRepo.FullName, 
                        patientClinicalEvent.SourceTerminologyMedDra.DisplayName);

                    var weeks = 0;
                    var config = _configRepository.Get(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks);
                    if (config != null)
                    {
                        if (!String.IsNullOrEmpty(config.ConfigValue))
                        {
                            weeks = Convert.ToInt32(config.ConfigValue);
                        }
                    }

                    // Prepare medications
                    List<ReportInstanceMedicationListItem> medications = new List<ReportInstanceMedicationListItem>();
                    foreach (var med in patientFromRepo.PatientMedications.Where(m => m.Archived == false 
                            && (m.DateEnd == null && m.DateStart.AddDays(weeks * -7) <= patientClinicalEvent.OnsetDate) 
                            || (m.DateEnd != null && m.DateStart.AddDays(weeks * -7) <= patientClinicalEvent.OnsetDate && Convert.ToDateTime(m.DateEnd).AddDays(weeks * 7) >= patientClinicalEvent.OnsetDate))
                        .OrderBy(m => m.Concept.ConceptName))
                    {
                        var item = new ReportInstanceMedicationListItem()
                        {
                            MedicationIdentifier = med.DisplayName,
                            ReportInstanceMedicationGuid = med.PatientMedicationGuid
                        };
                        medications.Add(item);
                    }
                    _workFlowService.AddOrUpdateMedicationsForWorkFlowInstance(patientClinicalEvent.PatientClinicalEventGuid, medications);

                    _unitOfWork.Complete();

                    var mappedPatientClinicalEvent = _mapper.Map<PatientClinicalEventIdentifierDto>(patientClinicalEvent);
                    if (mappedPatientClinicalEvent == null)
                    {
                        return StatusCode(500, "Unable to locate newly added clinical event");
                    }

                    return CreatedAtRoute("GetPatientClinicalEventByIdentifier",
                        new
                        {
                            id = mappedPatientClinicalEvent.Id
                        }, CreateLinksForPatientClinicalEvent<PatientClinicalEventIdentifierDto>(mappedPatientClinicalEvent));
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing patient clinical event
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the clinical event</param>
        /// <param name="clinicalEventForUpdate">The condition payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/clinicalevents/{id}", Name = "UpdatePatientClinicalEvent")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientClinicalEvent(long patientId, long id,
            [FromBody] PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            if (clinicalEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var clinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (clinicalEventFromRepo == null)
            {
                return NotFound();
            }

            var sourceTermFromRepo = _terminologyMeddraRepository.Get(clinicalEventForUpdate.SourceTerminologyMedDraId);
            if (sourceTermFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate source term");
            }

            ValidateClinicalEventForUpdateModel(patientFromRepo, clinicalEventForUpdate, id);

            if (ModelState.IsValid)
            {
                var clinicalEventDetail = PrepareClinicalEventDetail(clinicalEventForUpdate);
                if (!clinicalEventDetail.IsValid())
                {
                    clinicalEventDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    clinicalEventFromRepo.SourceDescription = clinicalEventForUpdate.SourceDescription;
                    clinicalEventFromRepo.OnsetDate = clinicalEventForUpdate.OnsetDate;
                    clinicalEventFromRepo.ResolutionDate = clinicalEventForUpdate.ResolutionDate;

                    _modelExtensionBuilder.UpdateExtendable(clinicalEventFromRepo, clinicalEventDetail.CustomAttributes, "Admin");

                    _patientClinicalEventRepository.Update(clinicalEventFromRepo);

                    _workFlowService.UpdateIdentifiersForWorkFlowInstance(clinicalEventFromRepo.PatientClinicalEventGuid,
                        clinicalEventFromRepo.Patient.FullName,
                        clinicalEventFromRepo.SourceTerminologyMedDra.DisplayName);

                    _unitOfWork.Complete();

                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing patient clinical event
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the clinical event</param>
        /// <param name="conditionForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/clinicalevents/{id}/archive", Name = "ArchivePatientClinicalEvent")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientClinicalEvent(long patientId, long id,
            [FromBody] ArchiveDto conditionForDelete)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var clinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (clinicalEventFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(conditionForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < conditionForDelete.Reason.Length)
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
                clinicalEventFromRepo.Archived = true;
                clinicalEventFromRepo.ArchivedDate = DateTime.Now;
                clinicalEventFromRepo.ArchivedReason = conditionForDelete.Reason;
                clinicalEventFromRepo.AuditUser = user;
                _patientClinicalEventRepository.Update(clinicalEventFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get single patient clinical event from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientClinicalEventAsync<T>(long patientId, long id) where T : class
        {
            var patientClinicalEventFromRepo = await _patientClinicalEventRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientClinicalEventFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientClinicalEvent = _mapper.Map<T>(patientClinicalEventFromRepo);

                return mappedPatientClinicalEvent;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientClinicalEventIdentifierDto CreateLinksForPatientClinicalEvent<T>(T dto)
        {
            PatientClinicalEventIdentifierDto identifier = (PatientClinicalEventIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "PatientClinicalEvent", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientClinicalEventDetailDto CustomPatientClinicalEventMap(PatientClinicalEventDetailDto dto)
        {
            var patientClinicalEvent = _patientClinicalEventRepository.Get(p => p.Id == dto.Id);
            if (patientClinicalEvent == null)
            {
                return dto;
            }
            VPS.CustomAttributes.IExtendable patientClinicalEventExtended = patientClinicalEvent;

            // Map all custom attributes
            dto.ClinicalEventAttributes = _modelExtensionBuilder.BuildModelExtension(patientClinicalEventExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == VPS.CustomAttributes.CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.ReportDate = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Date of Report", patientClinicalEventExtended);
            dto.IsSerious = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Is the adverse event serious?", patientClinicalEventExtended);

            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientClinicalEventExpandedDto CustomPatientClinicalEventMap(PatientClinicalEventExpandedDto dto)
        {
            var patientClinicalEventFromRepo = _patientClinicalEventRepository.Get(p => p.Id == dto.Id);
            if (patientClinicalEventFromRepo == null)
            {
                return dto;
            }
            VPS.CustomAttributes.IExtendable patientClinicalEventExtended = patientClinicalEventFromRepo;

            // Map all custom attributes
            dto.ClinicalEventAttributes = _modelExtensionBuilder.BuildModelExtension(patientClinicalEventExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == VPS.CustomAttributes.CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.ReportDate = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Date of Report", patientClinicalEventExtended);
            dto.IsSerious = _customAttributeService.GetCustomAttributeValue("PatientClinicalEvent", "Is the adverse event serious?", patientClinicalEventExtended);

            // Activity
            var activityItems = _workFlowService.GetExecutionStatusEventsForEventView(patientClinicalEventFromRepo);
            foreach (var activity in activityItems)
            {
                if (activity.ActivityItems.Count > 0)
                {
                    foreach(var activityItem in activity.ActivityItems)
                    {
                        dto.Activity.Add(new ActivityExecutionStatusEventDto()
                        {
                            ExecutedDate = activityItem.ExecutedDate,
                            Activity = activityItem.Activity,
                            ExecutedBy = activityItem.ExecutedBy,
                            ExecutionEvent = activityItem.ExecutionStatus,
                            Comments = activityItem.Comments,
                        });
                    }
                }
            }

            var reportInstanceFromRepo = _reportInstanceRepository.Get(ri => ri.ContextGuid == patientClinicalEventFromRepo.PatientClinicalEventGuid);
            if(reportInstanceFromRepo == null)
            {
                return dto;
            }

            // Meddra term
            dto.SetMedDraTerm = reportInstanceFromRepo.TerminologyMedDra?.DisplayName;

            // Meddra medications
            dto.Medications = _mapper.Map<ICollection<ReportInstanceMedicationDetailDto>>(reportInstanceFromRepo.Medications.Where(m => !String.IsNullOrWhiteSpace(m.WhoCausality) || (!String.IsNullOrWhiteSpace(m.NaranjoCausality))));

            return dto;
        }

        /// <summary>
        /// Get the corresponding selection value
        /// </summary>
        /// <param name="attributeKey">The custom attribute key look up value</param>
        /// <param name="selectionKey">The selection key look up value</param>
        /// <returns></returns>
        private string GetSelectionValue(string attributeKey, string selectionKey)
        {
            var selectionitem = _selectionDataItemRepository.Get(s => s.AttributeKey == attributeKey && s.SelectionKey == selectionKey);

            return (selectionitem == null) ? string.Empty : selectionitem.Value;
        }

        /// <summary>
        /// Validate the input model for updating a clinical event
        /// </summary>
        private void ValidateClinicalEventForUpdateModel(Patient patientFromRepo, PatientClinicalEventForUpdateDto clinicalEventForUpdateDto, long patientClinicalEventId)
        {
            if (Regex.Matches(clinicalEventForUpdateDto.SourceDescription, @"[-a-zA-Z0-9 .,()']").Count < clinicalEventForUpdateDto.SourceDescription.Length)
            {
                ModelState.AddModelError("Message", "Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");
            }

            if (clinicalEventForUpdateDto.OnsetDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Onset Date should be before current date");
            }
            if (clinicalEventForUpdateDto.OnsetDate < patientFromRepo.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Onset Date should be after Date Of Birth");
            }

            if(clinicalEventForUpdateDto.ResolutionDate.HasValue)
            {
                if (clinicalEventForUpdateDto.ResolutionDate > DateTime.Today)
                {
                    ModelState.AddModelError("Message", "Resolution Date should be before current date");
                }
                if (clinicalEventForUpdateDto.ResolutionDate < patientFromRepo.DateOfBirth)
                {
                    ModelState.AddModelError("Message", "Resolution Date should be after Date Of Birth");
                }
                if (clinicalEventForUpdateDto.ResolutionDate < clinicalEventForUpdateDto.OnsetDate)
                {
                    ModelState.AddModelError("Message", "Resolution Date should be after Onset Date");
                }
            }

            // Check clinical event overlapping - ONSET DATE
            if (patientFromRepo.CheckEventOnsetDateAgainstOnsetDateWithNoResolutionDate(clinicalEventForUpdateDto.SourceTerminologyMedDraId, clinicalEventForUpdateDto.OnsetDate, patientClinicalEventId))
            {
                ModelState.AddModelError("Message", "Duplication of adverse event. Please check onset and resolution dates...");
            }
            else
            {
                if (patientFromRepo.CheckEventOnsetDateWithinRange(clinicalEventForUpdateDto.SourceTerminologyMedDraId, clinicalEventForUpdateDto.OnsetDate, patientClinicalEventId))
                {
                    ModelState.AddModelError("Message", "Duplication of adverse event. Please check onset and resolution dates...");
                }
                else
                {
                    if (clinicalEventForUpdateDto.ResolutionDate.HasValue)
                    {
                        if (patientFromRepo.CheckEventOnsetDateWithNoResolutionDateBeforeOnset(clinicalEventForUpdateDto.SourceTerminologyMedDraId, clinicalEventForUpdateDto.OnsetDate, patientClinicalEventId))
                        {
                            ModelState.AddModelError("Message", "Duplication of adverse event. Please check onset and resolution dates...");
                        }
                    }
                }
            }

            // Check clinical event overlapping - RESOLUTION DATE
            if (clinicalEventForUpdateDto.ResolutionDate.HasValue)
            {
                if (patientFromRepo.CheckEventResolutionDateAgainstOnsetDateWithNoResolutionDate(clinicalEventForUpdateDto.SourceTerminologyMedDraId, Convert.ToDateTime(clinicalEventForUpdateDto.ResolutionDate), patientClinicalEventId))
                {
                    ModelState.AddModelError("Message", "Duplication of adverse event. Please check onset and resolution dates...");
                }
                else
                {
                    if (patientFromRepo.CheckEventResolutionDateWithinRange(clinicalEventForUpdateDto.SourceTerminologyMedDraId, Convert.ToDateTime(clinicalEventForUpdateDto.ResolutionDate), patientClinicalEventId))
                    {
                        ModelState.AddModelError("Message", "Duplication of adverse event. Please check onset and resolution dates...");
                    }
                }
            }
        }

        /// <summary>
        /// Prepare the model for the clinical event
        /// </summary>
        private ClinicalEventDetail PrepareClinicalEventDetail(PatientClinicalEventForUpdateDto clinicalEventForUpdate)
        {
            var clinicalEventDetail = new ClinicalEventDetail();
            clinicalEventDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientClinicalEvent>();

            //clinicalEventDetail = _mapper.Map<ClinicalEventDetail>(clinicalEventForUpdate);
            foreach (var newAttribute in clinicalEventForUpdate.Attributes)
            {
                var customAttribute = _customAttributeRepository.Get(ca => ca.Id == newAttribute.Key);
                if (customAttribute != null)
                {
                // Validate attribute exists for household entity and is a PMT attribute
                var attributeDetail = clinicalEventDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                    
                if (attributeDetail == null)
                    {
                        ModelState.AddModelError("Message", $"Unable to locate custom attribute on patient clinical event {newAttribute.Key}");
                    }
                    else
                    {
                        attributeDetail.Value = newAttribute.Value;
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", $"Unable to locate custom attribute {newAttribute.Key}");
                }
            }
            // Update patient custom attributes from source
            return clinicalEventDetail;
        }
    }
}
