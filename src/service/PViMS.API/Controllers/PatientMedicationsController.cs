using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Models;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PVIMS.API.Infrastructure.Services;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientMedicationsController : ControllerBase
    {
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<Concept> _conceptRepository;
        private readonly IRepositoryInt<Product> _productRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientMedicationsController(IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<Concept> conceptRepository,
            IRepositoryInt<Product> productRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IUnitOfWorkInt unitOfWork,
            ICustomAttributeService customAttributeService,
            IWorkFlowService workFlowService,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _conceptRepository = conceptRepository ?? throw new ArgumentNullException(nameof(conceptRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Get a single patient medication using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient medication</param>
        /// <returns>An ActionResult of type PatientMedicationIdentifierDto</returns>
        [HttpGet("{patientId}/medications/{id}", Name = "GetPatientMedicationByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientMedicationIdentifierDto>> GetPatientMedicationByIdentifier(long patientId, long id)
        {
            var mappedPatientMedication = await GetPatientMedicationAsync<PatientMedicationIdentifierDto>(patientId, id);
            if (mappedPatientMedication == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientMedication<PatientMedicationIdentifierDto>(mappedPatientMedication));
        }

        /// <summary>
        /// Get a single patient medication using it's unique code and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the patient medication</param>
        /// <returns>An ActionResult of type PatientMedicationDetailDto</returns>
        [HttpGet("{patientId}/medications/{id}", Name = "GetPatientMedicationByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientMedicationDetailDto>> GetPatientMedicationByDetail(long patientId, long id)
        {
            var mappedPatientMedication = await GetPatientMedicationAsync<PatientMedicationDetailDto>(patientId, id);
            if (mappedPatientMedication == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatientMedication<PatientMedicationDetailDto>(CustomPatientMedicationMap(mappedPatientMedication)));
        }

        /// <summary>
        /// Create a new patient medication record
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="medicationForUpdate">The medication payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/medications", Name = "CreatePatientMedication")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatientMedication(int patientId, 
            [FromBody] PatientMedicationForUpdateDto medicationForUpdate)
        {
            if (medicationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new medication");
                return BadRequest(ModelState);
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            Concept conceptFromRepo = null;
            if (medicationForUpdate.ConceptId > 0)
            {
                conceptFromRepo = _conceptRepository.Get(medicationForUpdate.ConceptId);
                if (conceptFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate concept");
                }
            }

            Product productFromRepo = null;
            if(medicationForUpdate.ProductId.HasValue && medicationForUpdate.ProductId > 0)
            {
                productFromRepo = _productRepository.Get(p => p.Id == medicationForUpdate.ProductId);
                if (productFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate product");
                }
                conceptFromRepo = productFromRepo.Concept;
            }

            // Ensure we always have a concept - even if the user selected a product
            if (conceptFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate concept");
            }

            ValidateMedicationForUpdateModel(patientFromRepo, medicationForUpdate, 0);

            if (ModelState.IsValid)
            {
                var medicationDetail = PrepareMedicationDetail(medicationForUpdate);
                if (!medicationDetail.IsValid())
                {
                    medicationDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    var patientMedication = new PatientMedication
                    {
                        MedicationSource = medicationForUpdate.SourceDescription,
                        Concept = conceptFromRepo,
                        Product = productFromRepo,
                        StartDate = medicationForUpdate.StartDate,
                        EndDate = medicationForUpdate.EndDate,
                        Dose = medicationForUpdate.Dose,
                        DoseFrequency = medicationForUpdate.DoseFrequency,
                        DoseUnit = medicationForUpdate.DoseUnit,
                        Patient = patientFromRepo
                    };

                    //throw new Exception(JsonConvert.SerializeObject(patientMedication));
                    _modelExtensionBuilder.UpdateExtendable(patientMedication, medicationDetail.CustomAttributes, "Admin");

                    _patientMedicationRepository.Save(patientMedication);
                    await AddOrUpdateMedicationsToReportInstanceAsync(patientMedication);
                    
                    await _unitOfWork.CompleteAsync();

                    var mappedPatientMedication = _mapper.Map<PatientMedicationIdentifierDto>(patientMedication);
                    if (mappedPatientMedication == null)
                    {
                        return StatusCode(500, "Unable to locate newly added medication");
                    }

                    return CreatedAtRoute("GetPatientMedicationByIdentifier",
                        new
                        {
                            id = mappedPatientMedication.Id
                        }, CreateLinksForPatientMedication<PatientMedicationIdentifierDto>(mappedPatientMedication));
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing patient medication
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="medicationForUpdate">The medication payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/medications/{id}", Name = "UpdatePatientMedication")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatientMedication(long patientId, long id,
            [FromBody] PatientMedicationForUpdateDto medicationForUpdate)
        {
            if (medicationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var medicationFromRepo = await _patientMedicationRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (medicationFromRepo == null)
            {
                return NotFound();
            }

            Concept conceptFromRepo = null;
            if (medicationForUpdate.ConceptId > 0)
            {
                conceptFromRepo = _conceptRepository.Get(medicationForUpdate.ConceptId);
                if (conceptFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate concept");
                }
            }

            Product productFromRepo = null;
            if (medicationForUpdate.ProductId.HasValue && medicationForUpdate.ProductId > 0)
            {
                productFromRepo = _productRepository.Get(p => p.Id == medicationForUpdate.ProductId);
                if (productFromRepo == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate product");
                }
                conceptFromRepo = productFromRepo.Concept;
            }

            // Ensure we always have a concept - even if the user selected a product
            if (conceptFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate concept");
            }

            ValidateMedicationForUpdateModel(patientFromRepo, medicationForUpdate, id);

            if (ModelState.IsValid)
            {
                var medicationDetail = PrepareMedicationDetail(medicationForUpdate);
                if (!medicationDetail.IsValid())
                {
                    medicationDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    medicationFromRepo.MedicationSource = medicationForUpdate.SourceDescription;
                    medicationFromRepo.Concept = conceptFromRepo;
                    medicationFromRepo.Product = productFromRepo;
                    medicationFromRepo.StartDate = medicationForUpdate.StartDate;
                    medicationFromRepo.EndDate = medicationForUpdate.EndDate;
                    medicationFromRepo.Dose = medicationForUpdate.Dose;
                    medicationFromRepo.DoseFrequency = medicationForUpdate.DoseFrequency;
                    medicationFromRepo.DoseUnit = medicationForUpdate.DoseUnit;

                    //throw new Exception(JsonConvert.SerializeObject(patientMedication));
                    _modelExtensionBuilder.UpdateExtendable(medicationFromRepo, medicationDetail.CustomAttributes, "Admin");

                    _patientMedicationRepository.Update(medicationFromRepo);
                    await AddOrUpdateMedicationsToReportInstanceAsync(medicationFromRepo);

                    await _unitOfWork.CompleteAsync();

                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Delete an existing patient medication
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the medication</param>
        /// <param name="medicationForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/medications/{id}/archive", Name = "ArchivePatientMedication")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ArchivePatientMedication(long patientId, long id,
            [FromBody] ArchiveDto medicationForDelete)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var medicationFromRepo = await _patientMedicationRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (medicationFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(medicationForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < medicationForDelete.Reason.Length)
            {
                ModelState.AddModelError("Message", "Reason contains invalid characters (Enter A-Z, a-z, space, period, apostrophe)");
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError("Message", "Unable to locate user");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                medicationFromRepo.Archived = true;
                medicationFromRepo.ArchivedDate = DateTime.Now;
                medicationFromRepo.ArchivedReason = medicationForDelete.Reason;
                medicationFromRepo.AuditUser = user;
                _patientMedicationRepository.Update(medicationFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Get single patient medication from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Parent resource id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientMedicationAsync<T>(long patientId, long id) where T : class
        {
            var patientMedicationFromRepo = await _patientMedicationRepository.GetAsync(pc => pc.Patient.Id == patientId && pc.Id == id);

            if (patientMedicationFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatientMedication = _mapper.Map<T>(patientMedicationFromRepo);

                return mappedPatientMedication;
            }

            return null;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientMedicationDetailDto CustomPatientMedicationMap(PatientMedicationDetailDto dto)
        {
            var patientMedication = _patientMedicationRepository.Get(p => p.Id == dto.Id);
            if (patientMedication == null)
            {
                return dto;
            }
            IExtendable patientMedicationExtended = patientMedication;

            // Map all custom attributes
            dto.MedicationAttributes = _modelExtensionBuilder.BuildModelExtension(patientMedicationExtended)
                .Select(h => new AttributeValueDto()
                {
                    Key = h.AttributeKey,
                    Value = h.TransformValueToString(),
                    Category = h.Category,
                    SelectionValue = (h.Type == CustomAttributeType.Selection) ? GetSelectionValue(h.AttributeKey, h.Value.ToString()) : string.Empty
                }).Where(s => (s.Value != "0" && !String.IsNullOrWhiteSpace(s.Value)) || !String.IsNullOrWhiteSpace(s.SelectionValue)).ToList();

            dto.IndicationType = _customAttributeService.GetCustomAttributeValue("PatientMedication", "Type of Indication", patientMedicationExtended);

            return dto;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientMedicationIdentifierDto CreateLinksForPatientMedication<T>(T dto)
        {
            PatientMedicationIdentifierDto identifier = (PatientMedicationIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("PatientMedication", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Validate the input model for updating a medication
        /// </summary>
        private void ValidateMedicationForUpdateModel(Patient patientFromRepo, PatientMedicationForUpdateDto medicationForUpdateDto, long patientMedicationId)
        {
            if (Regex.Matches(medicationForUpdateDto.SourceDescription, @"[-a-zA-Z0-9 .,()']").Count < medicationForUpdateDto.SourceDescription.Length)
            {
                ModelState.AddModelError("Message", "Source description contains invalid characters (Enter A-Z, a-z, 0-9, hyphen, space, period, comma, parentheses, apostrophe)");
            }

            if (medicationForUpdateDto.StartDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Start Date should be before current date");
            }
            if (medicationForUpdateDto.StartDate < patientFromRepo.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Start Date should be after Date Of Birth");
            }

            if(medicationForUpdateDto.EndDate.HasValue)
            {
                if (medicationForUpdateDto.EndDate > DateTime.Today)
                {
                    ModelState.AddModelError("Message", "End Date should be before current date");
                }
                if (medicationForUpdateDto.EndDate < patientFromRepo.DateOfBirth)
                {
                    ModelState.AddModelError("Message", "End Date should be after Date Of Birth");
                }
                if (medicationForUpdateDto.EndDate < medicationForUpdateDto.StartDate)
                {
                    ModelState.AddModelError("Message", "End Date should be after Start Date");
                }
            }

            // validate source term, check medication overlapping - START DATE
            if (patientFromRepo.CheckMedicationStartDateAgainstStartDateWithNoEndDate(medicationForUpdateDto.ConceptId, medicationForUpdateDto.StartDate, patientMedicationId))
            {
                ModelState.AddModelError("Message", "Duplication of medication. Please check start and end dates...");
            }
            else
            {
                if (patientFromRepo.CheckMedicationStartDateWithinRange(medicationForUpdateDto.ConceptId, medicationForUpdateDto.StartDate, patientMedicationId))
                {
                    ModelState.AddModelError("Message", "Duplication of medication. Please check start and end dates...");
                }
                else
                {
                    if (medicationForUpdateDto.EndDate.HasValue)
                    {
                        if (patientFromRepo.CheckMedicationStartDateWithNoEndDateBeforeStart(medicationForUpdateDto.ConceptId, medicationForUpdateDto.StartDate, patientMedicationId))
                        {
                            ModelState.AddModelError("Message", "Duplication of medication. Please check start and end dates...");
                        }
                    }
                }
            }

            // Check medication overlapping - END DATE
            if (medicationForUpdateDto.EndDate.HasValue)
            {
                if (patientFromRepo.CheckMedicationEndDateAgainstStartDateWithNoEndDate(medicationForUpdateDto.ConceptId, Convert.ToDateTime(medicationForUpdateDto.EndDate), patientMedicationId))
                {
                    ModelState.AddModelError("Message", "Duplication of medication. Please check start and end dates...");
                }
                else
                {
                    if (patientFromRepo.CheckMedicationEndDateWithinRange(medicationForUpdateDto.ConceptId, Convert.ToDateTime(medicationForUpdateDto.EndDate), patientMedicationId))
                    {
                        ModelState.AddModelError("Message", "Duplication of medication. Please check start and end dates...");
                    }
                }
            }

            if (Regex.Matches(medicationForUpdateDto.Dose, @"[a-zA-Z0-9.]").Count < medicationForUpdateDto.Dose.Length)
            {
                ModelState.AddModelError("Message", "Dose contains invalid characters (Enter A-Z, a-z, 0-9, period)");
            }

            if (Regex.Matches(medicationForUpdateDto.DoseFrequency, @"[a-zA-Z0-9.]").Count < medicationForUpdateDto.DoseFrequency.Length)
            {
                ModelState.AddModelError("Message", "Dose frequency contains invalid characters (Enter A-Z, a-z, 0-9, period)");
            }
        }

        /// <summary>
        /// Prepare the model for the medication
        /// </summary>
        private MedicationDetail PrepareMedicationDetail(PatientMedicationForUpdateDto medicationForUpdate)
        {
            var medicationDetail = new MedicationDetail();
            medicationDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientMedication>();

            //medicationDetail = _mapper.Map<MedicationDetail>(medicationForUpdate);
            foreach (var newAttribute in medicationForUpdate.Attributes)
            {
                var customAttribute = _customAttributeRepository.Get(ca => ca.Id == newAttribute.Key);
                if (customAttribute != null)
                {
                // Validate attribute exists for household entity and is a PMT attribute
                var attributeDetail = medicationDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                    
                if (attributeDetail == null)
                    {
                        ModelState.AddModelError("Message", $"Unable to locate custom attribute on patient medication {newAttribute.Key}");
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
            return medicationDetail;
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
        /// Modify source report instance with medification changes
        /// </summary>
        /// <param name="patientMedication">The medication to be updated</param>
        /// <returns></returns>
        private async Task AddOrUpdateMedicationsToReportInstanceAsync(PatientMedication patientMedication)
        {
            var weeks = 0;
            var config = _configRepository.Get(c => c.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks);
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    weeks = Convert.ToInt32(config.ConfigValue);
                }
            }

            // Manage modifications to report instance - if one exists
            IEnumerable<PatientClinicalEvent> events;
            if (!patientMedication.EndDate.HasValue)
            {
                events = patientMedication.Patient.PatientClinicalEvents.Where(pce => pce.OnsetDate >= patientMedication.StartDate.AddDays(weeks * -7) && pce.Archived == false);
            }
            else
            {
                events = patientMedication.Patient.PatientClinicalEvents.Where(pce => pce.OnsetDate >= patientMedication.StartDate.AddDays(weeks * -7) && pce.OnsetDate <= Convert.ToDateTime(patientMedication.EndDate).AddDays(weeks * 7) && pce.Archived == false);
            }

            // Prepare medications
            List<ReportInstanceMedicationListItem> instanceMedications = new List<ReportInstanceMedicationListItem>();
            var item = new ReportInstanceMedicationListItem()
            {
                MedicationIdentifier = patientMedication.DisplayName,
                ReportInstanceMedicationGuid = patientMedication.PatientMedicationGuid
            };
            instanceMedications.Add(item);

            foreach (var evt in events)
            {
                await _workFlowService.AddOrUpdateMedicationsForWorkFlowInstanceAsync(evt.PatientClinicalEventGuid, instanceMedications);
            }
        }
    }
}
