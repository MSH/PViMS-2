using AutoMapper;
using Ionic.Zip;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PVIMS.API.Application.Queries.ReportInstanceAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.Models;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Infrastructure;
using PVIMS.API.Application.Queries.PatientAggregate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class PatientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly ITypeExtensionHandler _modelExtensionBuilder;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<Encounter> _encounterRepository;
        private readonly IRepositoryInt<PatientCondition> _patientConditionRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<PatientMedication> _patientMedicationRepository;
        private readonly IRepositoryInt<PatientLabTest> _patientLabTestRepository;
        private readonly IRepositoryInt<PatientFacility> _patientFacilityRepository;
        private readonly IRepositoryInt<PatientStatusHistory> _patientStatusHistoryRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IRepositoryInt<Attachment> _attachmentRepository;
        private readonly IRepositoryInt<AttachmentType> _attachmentTypeRepository;
        private readonly IRepositoryInt<CohortGroup> _cohortGroupRepository;
        private readonly IRepositoryInt<CohortGroupEnrolment> _cohortGroupEnrolmentRepository;
        private readonly IRepositoryInt<TerminologyMedDra> _terminologyMeddraRepository;
        private readonly IRepositoryInt<ConditionMedDra> _conditionMeddraRepository;
        private readonly IRepositoryInt<EncounterType> _encounterTypeRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IRepositoryInt<SelectionDataItem> _selectionDataItemRepository;
        private readonly IReportInstanceQueries _reportInstanceQueries;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IReportService _reportService;
        private readonly IPatientService _patientService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IArtefactService _artefactService;
        private readonly ICustomAttributeService _customAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PVIMSDbContext _context;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IMediator mediator, 
            IPropertyMappingService propertyMappingService, 
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            ITypeExtensionHandler modelExtensionBuilder,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<Encounter> encounterRepository,
            IRepositoryInt<PatientCondition> patientConditionRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<PatientMedication> patientMedicationRepository,
            IRepositoryInt<PatientLabTest> patientLabTestRepository,
            IRepositoryInt<PatientFacility> patientFacilityRepository,
            IRepositoryInt<PatientStatusHistory> patientStatusHistoryRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<Appointment> appointmentRepository,
            IRepositoryInt<Attachment> attachmentRepository,
            IRepositoryInt<AttachmentType> attachmentTypeRepository,
            IRepositoryInt<CohortGroup> cohortGroupRepository,
            IRepositoryInt<CohortGroupEnrolment> cohortGroupEnrolmentRepository,
            IRepositoryInt<TerminologyMedDra> terminologyMeddraRepository,
            IRepositoryInt<ConditionMedDra> conditionMeddraRepository,
            IRepositoryInt<EncounterType> encounterTypeRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IRepositoryInt<SelectionDataItem> selectionDataItemRepository,
            IReportInstanceQueries reportInstanceQueries,
            IReportService reportService,
            IPatientService patientService,
            IWorkFlowService workFlowService,
            IArtefactService artefactService,
            IUnitOfWorkInt unitOfWork,
            ICustomAttributeService customAttributeService,
            IHttpContextAccessor httpContextAccessor,
            PVIMSDbContext dbContext,
            ILogger<PatientsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _modelExtensionBuilder = modelExtensionBuilder ?? throw new ArgumentNullException(nameof(modelExtensionBuilder));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _patientConditionRepository = patientConditionRepository ?? throw new ArgumentNullException(nameof(patientConditionRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _patientMedicationRepository = patientMedicationRepository ?? throw new ArgumentNullException(nameof(patientMedicationRepository));
            _patientLabTestRepository = patientLabTestRepository ?? throw new ArgumentNullException(nameof(patientLabTestRepository));
            _patientFacilityRepository = patientFacilityRepository ?? throw new ArgumentNullException(nameof(patientFacilityRepository));
            _patientStatusHistoryRepository = patientStatusHistoryRepository ?? throw new ArgumentNullException(nameof(patientStatusHistoryRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _attachmentRepository = attachmentRepository ?? throw new ArgumentNullException(nameof(attachmentRepository));
            _attachmentTypeRepository = attachmentTypeRepository ?? throw new ArgumentNullException(nameof(attachmentTypeRepository));
            _cohortGroupRepository = cohortGroupRepository ?? throw new ArgumentNullException(nameof(cohortGroupRepository));
            _cohortGroupEnrolmentRepository = cohortGroupEnrolmentRepository ?? throw new ArgumentNullException(nameof(cohortGroupEnrolmentRepository));
            _terminologyMeddraRepository = terminologyMeddraRepository ?? throw new ArgumentNullException(nameof(terminologyMeddraRepository));
            _conditionMeddraRepository = conditionMeddraRepository ?? throw new ArgumentNullException(nameof(conditionMeddraRepository));
            _encounterTypeRepository = encounterTypeRepository ?? throw new ArgumentNullException(nameof(encounterTypeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _selectionDataItemRepository = selectionDataItemRepository ?? throw new ArgumentNullException(nameof(selectionDataItemRepository));
            _reportInstanceQueries = reportInstanceQueries ?? throw new ArgumentNullException(nameof(reportInstanceQueries));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _customAttributeService = customAttributeService ?? throw new ArgumentNullException(nameof(customAttributeService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all Patients using a valid media type 
        /// </summary>
        /// <param name="patientResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of PatientIdentifierDto</returns>
        [HttpGet(Name = "GetPatientsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<PatientIdentifierDto>>> GetPatientsByIdentifier(
            [FromQuery] PatientResourceParameters patientResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<PatientIdentifierDto>
                (patientResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new PatientsIdentifierQuery(patientResourceParameters.OrderBy,
                patientResourceParameters.FacilityName,
                patientResourceParameters.CustomAttributeId,
                patientResourceParameters.CustomAttributeValue,
                patientResourceParameters.PatientId,
                patientResourceParameters.DateOfBirth,
                patientResourceParameters.FirstName,
                patientResourceParameters.LastName,
                patientResourceParameters.PageNumber,
                patientResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: GetPatientsIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = patientResourceParameters.PageSize,
                currentPage = patientResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all Patients using a valid media type 
        /// </summary>
        /// <param name="patientResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of PatientDetailDto</returns>
        [HttpGet(Name = "GetPatientsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<PatientDetailDto>>> GetPatientsByDetail(
            [FromQuery] PatientResourceParameters patientResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<PatientDetailDto, Patient>
               (patientResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new PatientsDetailQuery(patientResourceParameters.OrderBy,
                patientResourceParameters.FacilityName,
                patientResourceParameters.CustomAttributeId,
                patientResourceParameters.CustomAttributeValue,
                patientResourceParameters.PatientId,
                patientResourceParameters.DateOfBirth,
                patientResourceParameters.FirstName,
                patientResourceParameters.LastName,
                patientResourceParameters.PageNumber,
                patientResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: GetPatientsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = patientResourceParameters.PageSize,
                currentPage = patientResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single patient by searching for a matching concomitant condition
        /// </summary>
        /// <param name="patientByConditionResourceParameters">
        /// Specify condition search term
        /// </param>
        /// <returns>An ActionResult of type PatientExpandedDto</returns>
        [HttpGet(Name = "GetPatientByCondition")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientExpandedDto>> GetPatientByCondition(
            [FromQuery] PatientByConditionResourceParameters patientByConditionResourceParameters)
        {
            var query = new PatientExpandedByConditionTermQuery(
                patientByConditionResourceParameters.CustomAttributeKey,
                patientByConditionResourceParameters.CustomAttributeValue);

            _logger.LogInformation(
                "----- Sending query: PatientExpandedByConditionTermQuery");

            var queryResult = await _mediator.Send(query);

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Patient using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Patient</param>
        /// <returns>An ActionResult of type PatientIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetPatientByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<PatientIdentifierDto>> GetPatientByIdentifier(long id)
        {
            var mappedPatient = await GetPatientAsync<PatientIdentifierDto>(id);
            if (mappedPatient == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForPatient<PatientIdentifierDto>(mappedPatient));
        }

        /// <summary>
        /// Get a single Patient using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Patient</param>
        /// <returns>An ActionResult of type ProgramDetailDto</returns>
        [HttpGet("{id}", Name = "GetPatientByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientDetailDto>> GetPatientByDetail(int id)
        {
            var query = new PatientDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: GetPatientDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single Patient using it's unique code and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Patient</param>
        /// <returns>An ActionResult of type PatientExpandedDto</returns>
        [HttpGet("{id}", Name = "GetPatientByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<PatientExpandedDto>> GetPatientByExpanded(int id)
        {
            var query = new PatientExpandedQuery(id);

            _logger.LogInformation(
                "----- Sending query: GetPatientExpandedQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Adverse event report
        /// </summary>
        /// <param name="adverseEventReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AdverseEventReportDto</returns>
        [HttpGet(Name = "GetAdverseEventReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.adverseventreport.v1+json", "application/vnd.pvims.adverseventreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.adverseventreport.v1+json", "application/vnd.pvims.adverseventreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<AdverseEventReportDto>> GetAdverseEventReport(
                        [FromQuery] AdverseEventReportResourceParameters adverseEventReportResourceParameters)
        {
            if (adverseEventReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetAdverseEventResults<AdverseEventReportDto>(adverseEventReportResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForAdverseEventReport(wrapper, adverseEventReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Quarterly adverse event report
        /// </summary>
        /// <param name="baseReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AdverseEventFrequencyReportDto</returns>
        [HttpGet(Name = "GetAdverseEventQuarterlyReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.quarterlyadverseventreport.v1+json", "application/vnd.pvims.quarterlyadverseventreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.quarterlyadverseventreport.v1+json", "application/vnd.pvims.quarterlyadverseventreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>> GetAdverseEventQuarterlyReport(
                        [FromQuery] BaseReportResourceParameters baseReportResourceParameters)
        {
            if (baseReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetQuarterlyAdverseEventResults<AdverseEventFrequencyReportDto>(baseReportResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForQuarterlyAdverseEventReport(wrapper, baseReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Annual adverse event report
        /// </summary>
        /// <param name="baseReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type AdverseEventFrequencyReportDto</returns>
        [HttpGet(Name = "GetAdverseEventAnnualReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.annualadverseventreport.v1+json", "application/vnd.pvims.annualadverseventreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.annualadverseventreport.v1+json", "application/vnd.pvims.annualadverseventreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>> GetAdverseEventAnnualReport(
                        [FromQuery] BaseReportResourceParameters baseReportResourceParameters)
        {
            if (baseReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetAnnualAdverseEventResults<AdverseEventFrequencyReportDto>(baseReportResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AdverseEventFrequencyReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForAnnualAdverseEventReport(wrapper, baseReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Patient on treatment report
        /// </summary>
        /// <param name="patientTreatmentReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type PatientTreatmentReportDto</returns>
        [HttpGet(Name = "GetPatientTreatmentReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.patienttreatmentreport.v1+json", "application/vnd.pvims.patienttreatmentreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.patienttreatmentreport.v1+json", "application/vnd.pvims.patienttreatmentreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<PatientTreatmentReportDto>> GetPatientTreatmentReport(
                        [FromQuery] PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters)
        {
            if (patientTreatmentReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetPatientTreatmentResults<PatientTreatmentReportDto>(patientTreatmentReportResourceParameters);

            // Add custom mappings to patients
            mappedResults.ForEach(dto => CustomPatientTreatmentReportMap(dto, patientTreatmentReportResourceParameters));

            var wrapper = new LinkedCollectionResourceWrapperDto<PatientTreatmentReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForPatientTreatmentReport(wrapper, patientTreatmentReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Patient by drug report
        /// </summary>
        /// <param name="patientMedicationReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type PatientMedicationReportDto</returns>
        [HttpGet(Name = "GetPatientByMedicationReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.patientmedicationreport.v1+json", "application/vnd.pvims.patientmedicationreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.patientmedicationreport.v1+json", "application/vnd.pvims.patientmedicationreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<PatientMedicationReportDto>> GetPatientByMedicationReport(
                        [FromQuery] PatientMedicationReportResourceParameters patientMedicationReportResourceParameters)
        {
            if (patientMedicationReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetPatientMedicationResults<PatientMedicationReportDto>(patientMedicationReportResourceParameters);

            // Add custom mappings to patients
            mappedResults.ForEach(dto => CustomPatientMedicationReportMap(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<PatientMedicationReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForPatientMedicationReport(wrapper, patientMedicationReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single patient attachment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the attachment</param>
        /// <returns>An ActionResult of type PatientIdentifierDto</returns>
        [HttpGet("{patientId}/attachments/{id}", Name = "GetPatientAttachmentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<AttachmentIdentifierDto>> GetPatientAttachmentByIdentifier(int patientId, int id)
        {
            var mappedAttachment = await GetAttachmentAsync<AttachmentIdentifierDto>(patientId, id);
            if (mappedAttachment == null)
            {
                return NotFound();
            }


            return Ok(CreateLinksForAttachment<AttachmentIdentifierDto>(patientId, mappedAttachment));
        }

        /// <summary>
        /// Download a file attachment
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="id">The unique identifier for the attachment</param>
        /// <returns>An ActionResult</returns>
        [HttpGet("{patientId}/attachments/{id}", Name = "DownloadSingleAttachment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.attachment.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadSingleAttachment(int patientId, int id)
        {
            var attachmentFromRepo = await _attachmentRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (attachmentFromRepo == null)
            {
                return NotFound();
            }

            byte[] buffer = (byte[])attachmentFromRepo.Content;

            var destFile = $"{Path.GetTempPath()}{attachmentFromRepo.FileName}";
            FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write);
            // Writes a block of bytes to this stream using data from // a byte array.
            fs.Write(buffer, 0, attachmentFromRepo.Content.Length);
            // close file stream
            fs.Close();

            var mime = "";
            switch (attachmentFromRepo.AttachmentType.Key)
            {
                case "docx":
                    mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break; 

                case "doc":
                    mime = "application/msword";
                    break;

                case "xlsx":
                    mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;

                case "xls":
                    mime = "application/vnd.ms-excel";
                    break;

                case "pdf":
                    mime = "application/pdf";
                    break;

                case "png":
                    mime = "image/png";
                    break;

                case "jpg":
                case "jpeg":
                    mime = "image/jpeg";
                    break;
                    
                case "bmp":
                    mime = "image/bmp";
                    break;

                case "xml":
                    mime = "application/xml";
                    break;
            }

            return PhysicalFile(destFile, mime);
        }

        /// <summary>
        /// Download all file attachments
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <returns>An ActionResult of type AuditLogIdentifierDto</returns>
        [HttpGet("{patientId}/attachments", Name = "DownloadAllAttachment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.attachment.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.attachment.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadAllAttachment(int patientId)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            FileStream fs;
            var attachmentFileNames = new List<string>();
            var generatedDate = DateTime.Now;

            foreach (Attachment attachment in patientFromRepo.Attachments.Where(a => a.Archived == false))
            {
                byte[] buffer = (byte[])attachment.Content;
                var attachmentFile = $"{Path.GetTempPath()}{attachment.FileName}";
                fs = new FileStream(attachmentFile, FileMode.Create, FileAccess.Write);

                // Writes a block of bytes to this stream using data from // a byte array.
                fs.Write(buffer, 0, attachment.Content.Length);

                // close file stream
                fs.Close();
                attachmentFileNames.Add(attachment.FileName);
            }

            var destFile = $"{Path.GetTempPath()}Patient_{patientId.ToString()}_Attachments_{generatedDate.ToString("yyyyMMddhhmmss")}.zip";

            using (var zip = new ZipFile())
            {
                zip.AddFiles(attachmentFileNames.Select(f => $"{Path.GetTempPath()}{f}").ToList(), string.Empty);
                zip.Save(destFile);
            }

            return PhysicalFile(destFile, "application/zip");
        }

        /// <summary>
        /// Create a new patient record
        /// </summary>
        /// <param name="patientForCreation">The patient payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreatePatient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientForCreationDto patientForCreation)
        {
            if (patientForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new patient");
                return BadRequest(ModelState);
            }

            // Ensure patient record does not exist
            var identifier_record = patientForCreation.Attributes[ _customAttributeRepository.Get(ca => ca.AttributeKey == "Medical Record Number").Id];
            var identifier_id = patientForCreation.Attributes[_customAttributeRepository.Get(ca => ca.AttributeKey == "Patient Identity Number").Id];

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_record });
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Patient Identity Number", AttributeValue = identifier_id });

            //if (!_patientService.isUnique(parameters))
            //{
            //    ModelState.AddModelError("Message", "Potential duplicate patient. Check medical record number and patient identity number.");
            //}

            ValidatePatientForCreationModel(patientForCreation);

            long id = 0;

            if (ModelState.IsValid)
            {
                var patientDetail = PreparePatientDetail(patientForCreation);
                if (!patientDetail.IsValid())
                {
                    patientDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    patientDetail.FirstName = patientForCreation.FirstName;
                    patientDetail.Surname = patientForCreation.LastName;
                    patientDetail.MiddleName = patientForCreation.MiddleName;
                    patientDetail.DateOfBirth = patientForCreation.DateOfBirth;
                    patientDetail.CurrentFacilityName = patientForCreation.FacilityName;
                    patientDetail.CohortGroupId = patientForCreation.CohortGroupId;
                    patientDetail.EnroledDate = patientForCreation.EnroledDate;
                    patientDetail.EncounterTypeId = patientForCreation.EncounterTypeId;
                    patientDetail.PriorityId = patientForCreation.PriorityId;
                    patientDetail.EncounterDate = patientForCreation.EncounterDate;

                    id = await _patientService.AddPatientAsync(patientDetail);
                    await _unitOfWork.CompleteAsync();

                    var mappedPatient = await GetPatientAsync<PatientIdentifierDto>(id);
                    if (mappedPatient == null)
                    {
                        return StatusCode(500, "Unable to locate newly added patient");
                    }

                    return CreatedAtAction("GetPatientByIdentifier",
                        new
                        {
                            id = mappedPatient.Id
                        }, CreateLinksForPatient<PatientIdentifierDto>(mappedPatient));
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing patient
        /// </summary>
        /// <param name="id">The unique id of the patient</param>
        /// <param name="patientForUpdate">The patient payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdatePatient")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdatePatient(int id,
            [FromBody] PatientForUpdateDto patientForUpdate)
        {
            if (patientForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == id);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            var facilityFromRepo = _facilityRepository.Get(f => f.FacilityName == patientForUpdate.FacilityName);
            if (facilityFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate facility");
            }

            // Ensure patient record does not exist
            var identifier_record = patientForUpdate.Attributes[_customAttributeRepository.Get(ca => ca.AttributeKey == "Medical Record Number").Id];
            var identifier_id = patientForUpdate.Attributes[_customAttributeRepository.Get(ca => ca.AttributeKey == "Patient Identity Number").Id];

            List<CustomAttributeParameter> parameters = new List<CustomAttributeParameter>();
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Medical Record Number", AttributeValue = identifier_record });
            parameters.Add(new CustomAttributeParameter() { AttributeKey = "Patient Identity Number", AttributeValue = identifier_id });

            if (!_patientService.isUnique(parameters, id))
            {
                ModelState.AddModelError("Message", "Potential duplicate patient. Check medical record number and patient identity number.");
            }

            ValidatePatientForUpdateModel(patientForUpdate);

            if (ModelState.IsValid)
            {
                var patientDetail = PreparePatientDetail(patientForUpdate);
                if (!patientDetail.IsValid())
                {
                    patientDetail.InvalidAttributes.ForEach(element => ModelState.AddModelError("Message", element));
                }

                if (ModelState.IsValid)
                {
                    patientFromRepo.FirstName = patientForUpdate.FirstName;
                    patientFromRepo.Surname = patientForUpdate.LastName;
                    patientFromRepo.MiddleName = patientForUpdate.MiddleName;
                    patientFromRepo.DateOfBirth = patientForUpdate.DateOfBirth;
                    patientFromRepo.Notes = patientForUpdate.Notes;

                    patientFromRepo.SetPatientFacility(facilityFromRepo);

                    _modelExtensionBuilder.UpdateExtendable(patientFromRepo, patientDetail.CustomAttributes, "Admin");

                    _patientRepository.Update(patientFromRepo);
                    await _unitOfWork.CompleteAsync();

                    return Ok();
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Archive an existing patient
        /// </summary>
        /// <param name="id">The unique identifier of the patient</param>
        /// <param name="patientForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{Id}/archive", Name = "ArchivePatient")]
        [Authorize(Roles = "Clinician")]
        public async Task<IActionResult> ArchivePatient(long id,
            [FromBody] ArchiveDto patientForDelete)
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == id);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(patientForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < patientForDelete.Reason.Length)
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
                foreach (var appointment in patientFromRepo.Appointments.Where(x => !x.Archived))
                {
                    appointment.Archived = true;
                    appointment.ArchivedDate = DateTime.Now;
                    appointment.ArchivedReason = patientForDelete.Reason;
                    appointment.AuditUser = user;
                    _appointmentRepository.Update(appointment);
                }

                foreach (var attachment in patientFromRepo.Attachments.Where(x => !x.Archived))
                {
                    attachment.Archived = true;
                    attachment.ArchivedDate = DateTime.Now;
                    attachment.ArchivedReason = patientForDelete.Reason;
                    attachment.AuditUser = user;
                    _attachmentRepository.Update(attachment);
                }

                foreach (var enrolment in patientFromRepo.CohortEnrolments.Where(x => !x.Archived))
                {
                    enrolment.Archived = true;
                    enrolment.ArchivedDate = DateTime.Now;
                    enrolment.ArchivedReason = patientForDelete.Reason;
                    enrolment.AuditUser = user;
                    _cohortGroupEnrolmentRepository.Update(enrolment);
                }

                foreach (var encounter in patientFromRepo.Encounters.Where(x => !x.Archived))
                {
                    encounter.Archived = true;
                    encounter.ArchivedDate = DateTime.Now;
                    encounter.ArchivedReason = patientForDelete.Reason;
                    encounter.AuditUser = user;
                    _encounterRepository.Update(encounter);
                }

                foreach (var patientFacility in patientFromRepo.PatientFacilities.Where(x => !x.Archived))
                {
                    patientFacility.Archived = true;
                    patientFacility.ArchivedDate = DateTime.Now;
                    patientFacility.ArchivedReason = patientForDelete.Reason;
                    patientFacility.AuditUser = user;
                    _patientFacilityRepository.Update(patientFacility);
                }

                foreach (var patientClinicalEvent in patientFromRepo.PatientClinicalEvents.Where(x => !x.Archived))
                {
                    patientClinicalEvent.Archived = true;
                    patientClinicalEvent.ArchivedDate = DateTime.Now;
                    patientClinicalEvent.ArchivedReason = patientForDelete.Reason;
                    patientClinicalEvent.AuditUser = user;
                    _patientClinicalEventRepository.Update(patientClinicalEvent);
                }

                foreach (var patientCondition in patientFromRepo.PatientConditions.Where(x => !x.Archived))
                {
                    patientCondition.Archived = true;
                    patientCondition.ArchivedDate = DateTime.Now;
                    patientCondition.ArchivedReason = patientForDelete.Reason;
                    patientCondition.AuditUser = user;
                    _patientConditionRepository.Update(patientCondition);
                }

                foreach (var patientLabTest in patientFromRepo.PatientLabTests.Where(x => !x.Archived))
                {
                    patientLabTest.Archived = true;
                    patientLabTest.ArchivedDate = DateTime.Now;
                    patientLabTest.ArchivedReason = patientForDelete.Reason;
                    patientLabTest.AuditUser = user;
                    _patientLabTestRepository.Update(patientLabTest);
                }

                foreach (var patientMedication in patientFromRepo.PatientMedications.Where(x => !x.Archived))
                {
                    patientMedication.ArchiveMedication(user, patientForDelete.Reason);
                    _patientMedicationRepository.Update(patientMedication);
                }

                foreach (var patientStatusHistory in patientFromRepo.PatientStatusHistories.Where(x => !x.Archived))
                {
                    patientStatusHistory.Archived = true;
                    patientStatusHistory.ArchivedDate = DateTime.Now;
                    patientStatusHistory.ArchivedReason = patientForDelete.Reason;
                    patientStatusHistory.AuditUser = user;
                    _patientStatusHistoryRepository.Update(patientStatusHistory);
                }

                patientFromRepo.Archived = true;
                patientFromRepo.ArchivedDate = DateTime.Now;
                patientFromRepo.ArchivedReason = patientForDelete.Reason;
                patientFromRepo.AuditUser = user;
                _patientRepository.Update(patientFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Create a new patient attachment
        /// </summary>
        /// <param name="patientId">The unique identifier for the patient</param>
        /// <param name="patientAttachmentForCreation">The attachment payload</param>
        /// <returns></returns>
        [HttpPost("{patientId}/attachments", Name = "CreatePatientAttachment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePatientAttachment(int patientId, [FromForm] PatientAttachmentForCreationDto patientAttachmentForCreation)
        {
            if (patientAttachmentForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new attachment");
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(patientAttachmentForCreation.Description))
            {
                if (Regex.Matches(patientAttachmentForCreation.Description, @"[a-zA-Z0-9 ]").Count < patientAttachmentForCreation.Description.Length)
                {
                    ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z, 0-9, space)");
                    return BadRequest(ModelState);
                }
            }

            if (patientAttachmentForCreation.Attachment.Length > 0)
            {
                var fileExtension = Path.GetExtension(patientAttachmentForCreation.Attachment.FileName).Replace(".", "");
                var attachmentType = _attachmentTypeRepository.Get(at => at.Key == fileExtension);

                if (attachmentType == null)
                {
                    ModelState.AddModelError("Message", "Invalid file type, please select another file");
                    return BadRequest(ModelState);
                }

                var fileName = ContentDispositionHeaderValue.Parse(patientAttachmentForCreation.Attachment.ContentDisposition).FileName.Trim();

                if (fileName.Length > 50)
                {
                    ModelState.AddModelError("Message", "Maximumum file name length of 50 characters, please rename the file before uploading");
                    return BadRequest(ModelState);
                }

                // Create the attachment
                BinaryReader reader = new BinaryReader(patientAttachmentForCreation.Attachment.OpenReadStream());
                byte[] buffer = reader.ReadBytes((int)patientAttachmentForCreation.Attachment.Length);

                var attachment = new Attachment
                {
                    Patient = patientFromRepo,
                    Description = patientAttachmentForCreation.Description,
                    FileName = patientAttachmentForCreation.Attachment.FileName,
                    AttachmentType = attachmentType,
                    Size = patientAttachmentForCreation.Attachment.Length,
                    Content = buffer
                };

                _attachmentRepository.Save(attachment);
                await _unitOfWork.CompleteAsync();

                var mappedAttachment = await GetAttachmentAsync<AttachmentIdentifierDto>(patientId, attachment.Id);
                if (mappedAttachment == null)
                {
                    return StatusCode(500, "Unable to locate newly added attachment");
                }

                return CreatedAtAction("GetPatientAttachmentByIdentifier",
                    new
                    {
                        id = mappedAttachment.Id
                    }, CreateLinksForAttachment<AttachmentIdentifierDto>(patientId, mappedAttachment));
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Archive an existing attachment
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the attachment</param>
        /// <param name="attachmentForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("{patientId}/attachments/{id}/archive", Name = "ArchivePatientAttachment")]
        public async Task<IActionResult> ArchivePatientAttachment(long patientId, long id,
            [FromBody] ArchiveDto attachmentForDelete)
        {
            var attachmentFromRepo = await _attachmentRepository.GetAsync(f => f.Patient.Id == patientId && f.Id == id);
            if (attachmentFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(attachmentForDelete.Reason, @"[-a-zA-Z0-9 .']").Count < attachmentForDelete.Reason.Length)
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
                attachmentFromRepo.Archived = true;
                attachmentFromRepo.ArchivedDate = DateTime.Now;
                attachmentFromRepo.ArchivedReason = attachmentForDelete.Reason;
                attachmentFromRepo.AuditUser = user;
                _attachmentRepository.Update(attachmentFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get single Patient from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetPatientAsync<T>(long id) where T : class
        {
            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == id);

            if (patientFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedPatient = _mapper.Map<T>(patientFromRepo);

                return mappedPatient;
            }

            return null;
        }

        /// <summary>
        /// Get single attachment from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="patientId">Resource parent id to search by</param>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetAttachmentAsync<T>(int patientId, int id) where T : class
        {
            var attachmentFromRepo = await _attachmentRepository.GetAsync(a => a.Patient.Id == patientId && a.Id == id);

            if (attachmentFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedAttachment = _mapper.Map<T>(attachmentFromRepo);

                return mappedAttachment;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="patientMedicationReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForPatientMedicationReport(
            LinkedResourceBaseDto wrapper,
            PatientMedicationReportResourceParameters patientMedicationReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreatePatientMedicationReportResourceUri(ResourceUriType.Current, patientMedicationReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientMedicationReportResourceUri(ResourceUriType.NextPage, patientMedicationReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientMedicationReportResourceUri(ResourceUriType.PreviousPage, patientMedicationReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="patientTreatmentReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForPatientTreatmentReport(
            LinkedResourceBaseDto wrapper,
            PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreatePatientTreatmentReportResourceUri(ResourceUriType.Current, patientTreatmentReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientTreatmentReportResourceUri(ResourceUriType.NextPage, patientTreatmentReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreatePatientTreatmentReportResourceUri(ResourceUriType.PreviousPage, patientTreatmentReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="adverseEventReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForAdverseEventReport(
            LinkedResourceBaseDto wrapper,
            AdverseEventReportResourceParameters adverseEventReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAdverseEventReportResourceUri(ResourceUriType.Current, adverseEventReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAdverseEventReportResourceUri(ResourceUriType.NextPage, adverseEventReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAdverseEventReportResourceUri(ResourceUriType.PreviousPage, adverseEventReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="baseReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForQuarterlyAdverseEventReport(
            LinkedResourceBaseDto wrapper,
            BaseReportResourceParameters baseReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.Current, "GetAdverseEventQuarterlyReport", baseReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.NextPage, "GetAdverseEventQuarterlyReport", baseReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.PreviousPage, "GetAdverseEventQuarterlyReport", baseReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="baseReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForAnnualAdverseEventReport(
            LinkedResourceBaseDto wrapper,
            BaseReportResourceParameters baseReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.Current, "GetAdverseEventAnnualReport", baseReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.NextPage, "GetAdverseEventAnnualReport", baseReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateReportResourceUriForWrapper(ResourceUriType.PreviousPage, "GetAdverseEventAnnualReport", baseReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientIdentifierDto CreateLinksForPatient<T>(T dto)
        {
            PatientIdentifierDto identifier = (PatientIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", identifier.Id), "self", "GET"));
            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateNewAppointmentForPatientResourceUri(identifier.Id), "newAppointment", "POST"));
            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateNewEnrolmentForPatientResourceUri(identifier.Id), "newEnrolment", "POST"));

            return identifier;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="patientId">The unique identifier of the parent resource</param>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private AttachmentIdentifierDto CreateLinksForAttachment<T>(int patientId, T dto)
        {
            AttachmentIdentifierDto identifier = (AttachmentIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreatePatientAppointmentResourceUri(patientId, identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private PatientMedicationReportDto CustomPatientMedicationReportMap(PatientMedicationReportDto dto)
        {
            dto.Patients = _mapper.Map<List<PatientListDto>>(_reportService.GetPatientListByDrugItems(dto.ConceptId));

            return dto;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <param name="patientTreatmentReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PatientTreatmentReportDto CustomPatientTreatmentReportMap(PatientTreatmentReportDto dto, PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters)
        {
            dto.Patients = _mapper.Map<List<PatientListDto>>(_reportService.GetPatientListOnStudyItems(patientTreatmentReportResourceParameters.SearchFrom, patientTreatmentReportResourceParameters.SearchTo, patientTreatmentReportResourceParameters.PatientOnStudyCriteria, dto.FacilityId));

            return dto;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="adverseEventReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetAdverseEventResults<T>(AdverseEventReportResourceParameters adverseEventReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = adverseEventReportResourceParameters.PageNumber,
                PageSize = adverseEventReportResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<AdverseEventList>.Create(_reportService.GetAdverseEventItems(
                adverseEventReportResourceParameters.SearchFrom, 
                adverseEventReportResourceParameters.SearchTo, 
                adverseEventReportResourceParameters.AdverseEventCriteria,
                adverseEventReportResourceParameters.AdverseEventStratifyCriteria), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="baseReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetQuarterlyAdverseEventResults<T>(BaseReportResourceParameters baseReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = baseReportResourceParameters.PageNumber,
                PageSize = baseReportResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<AdverseEventQuarterlyList>.Create(_reportService.GetAdverseEventQuarterlyItems(
                baseReportResourceParameters.SearchFrom,
                baseReportResourceParameters.SearchTo), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="baseReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetAnnualAdverseEventResults<T>(BaseReportResourceParameters baseReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = baseReportResourceParameters.PageNumber,
                PageSize = baseReportResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<AdverseEventAnnualList>.Create(_reportService.GetAdverseEventAnnualItems(
                baseReportResourceParameters.SearchFrom,
                baseReportResourceParameters.SearchTo), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="patientTreatmentReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetPatientTreatmentResults<T>(PatientTreatmentReportResourceParameters patientTreatmentReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = patientTreatmentReportResourceParameters.PageNumber,
                PageSize = patientTreatmentReportResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<PatientOnStudyList>.Create(_reportService.GetPatientOnStudyItems(
                patientTreatmentReportResourceParameters.SearchFrom,
                patientTreatmentReportResourceParameters.SearchTo,
                patientTreatmentReportResourceParameters.PatientOnStudyCriteria), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="patientMedicationReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetPatientMedicationResults<T>(PatientMedicationReportResourceParameters patientMedicationReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = patientMedicationReportResourceParameters.PageNumber,
                PageSize = patientMedicationReportResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<DrugList>.Create(_reportService.GetPatientsByDrugItems(patientMedicationReportResourceParameters.SearchTerm), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedResults = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedResults.TotalCount,
                    pageSize = mappedResults.PageSize,
                    currentPage = mappedResults.CurrentPage,
                    totalPages = mappedResults.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                return mappedResults;
            }

            return null;
        }

        /// <summary>
        /// Validate the input model for adding a new patient
        /// </summary>
        private void ValidatePatientForCreationModel(PatientForCreationDto patientForCreation)
        {
            if (Regex.Matches(patientForCreation.FirstName, @"[-a-zA-Z ']").Count < patientForCreation.FirstName.Length)
            {
                ModelState.AddModelError("Message", "First name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
            }

            if (!String.IsNullOrEmpty(patientForCreation.MiddleName))
            {
                if (Regex.Matches(patientForCreation.MiddleName, @"[-a-zA-Z ']").Count < patientForCreation.MiddleName.Length)
                {
                    ModelState.AddModelError("Message", "Middle name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
                }
            }

            if (Regex.Matches(patientForCreation.LastName, @"[-a-zA-Z ']").Count < patientForCreation.LastName.Length)
            {
                ModelState.AddModelError("Message", "Last name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
            }

            if (patientForCreation.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Date of birth should be before current date");
            }

            if (patientForCreation.DateOfBirth < DateTime.Today.AddYears(-120))
            {
                ModelState.AddModelError("Message", "Date of birth cannot be so far in the past");
            }

            var termSource = _terminologyMeddraRepository.Get(tm => tm.Id == patientForCreation.MeddraTermId);
            if (termSource == null)
            {
                ModelState.AddModelError("Message", "Unable to locate meddra term");
            }

            if (patientForCreation.CohortGroupId.HasValue && patientForCreation.CohortGroupId > 0)
            {
                var cohortGroup = _cohortGroupRepository.Get(cg => cg.Id == patientForCreation.CohortGroupId);
                if (cohortGroup == null)
                {
                    ModelState.AddModelError("Message", "Unable to locate cohort group");
                }

                if (!patientForCreation.EnroledDate.HasValue)
                {
                    ModelState.AddModelError("Message", "Cohort enrollment date must be specified if cohort selected");
                }
                else
                {
                    if (patientForCreation.EnroledDate > DateTime.Today)
                    {
                        ModelState.AddModelError("Message", "Cohort enrollment date should be before current date");
                    }
                    if (patientForCreation.EnroledDate < patientForCreation.DateOfBirth)
                    {
                        ModelState.AddModelError("Message", "Cohort enrollment date should be after date of birth");
                    }
                }
            }

            if (patientForCreation.StartDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Condition start date should be before current date");
            }
            if (patientForCreation.StartDate < patientForCreation.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Condition start date should be after date of birth");
            }

            if (patientForCreation.OutcomeDate.HasValue)
            {
                if (patientForCreation.OutcomeDate > DateTime.Today)
                {
                    ModelState.AddModelError("Message", "Condition outcome date should be before current date");
                }
                if (patientForCreation.OutcomeDate < patientForCreation.StartDate)
                {
                    ModelState.AddModelError("Message", "Condition outcome date should be after start date");
                }
            }

            var encounterType = _encounterTypeRepository.Get(et => et.Id == patientForCreation.EncounterTypeId);
            if (encounterType == null)
            {
                ModelState.AddModelError("Message", "Unable to locate encounter type");
            }

            if (patientForCreation.EncounterDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Encounter date should be before current date");
            }
            if (patientForCreation.EncounterDate < patientForCreation.DateOfBirth)
            {
                ModelState.AddModelError("Message", "Encounter date should be after date of birth");
            }
        }

        /// <summary>
        /// Validate the input model for updating an existing patient
        /// </summary>
        private void ValidatePatientForUpdateModel(PatientForUpdateDto patientForUpdate)
        {
            if (Regex.Matches(patientForUpdate.FirstName, @"[-a-zA-Z ']").Count < patientForUpdate.FirstName.Length)
            {
                ModelState.AddModelError("Message", "First name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
            }

            if (!String.IsNullOrEmpty(patientForUpdate.MiddleName))
            {
                if (Regex.Matches(patientForUpdate.MiddleName, @"[-a-zA-Z ']").Count < patientForUpdate.MiddleName.Length)
                {
                    ModelState.AddModelError("Message", "Middle name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
                }
            }

            if (Regex.Matches(patientForUpdate.LastName, @"[-a-zA-Z ']").Count < patientForUpdate.LastName.Length)
            {
                ModelState.AddModelError("Message", "Last name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
            }

            if (patientForUpdate.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Date of birth should be before current date");
            }

            if (patientForUpdate.DateOfBirth < DateTime.Today.AddYears(-120))
            {
                ModelState.AddModelError("Message", "Date of birth cannot be so far in the past");
            }
        }

        /// <summary>
        /// Prepare the model for adding a new patient
        /// </summary>
        private PatientDetailForCreation PreparePatientDetail(PatientForCreationDto patientForCreation)
        {
            var patientDetail = new PatientDetailForCreation();
            patientDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<Patient>();

            // Update patient custom attributes from source
            foreach (var newAttribute in patientForCreation.Attributes)
            {
                var customAttribute = _customAttributeRepository.Get(ca => ca.Id == newAttribute.Key);
                if (customAttribute != null)
                {
                    // Validate attribute exists for household entity and is a PMT attribute
                    var attributeDetail = patientDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                    if (attributeDetail == null)
                    {
                        ModelState.AddModelError("Message", $"Unable to locate custom attribute on patient {newAttribute.Key}");
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

            // Prepare primary condition
            var conditionDetail = new ConditionDetail();
            conditionDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<PatientCondition>();
            var termSource = _terminologyMeddraRepository.Get(tm => tm.Id == patientForCreation.MeddraTermId);

            conditionDetail.MeddraTermId = termSource.Id;
            conditionDetail.ConditionSource = termSource.MedDraTerm;
            conditionDetail.OnsetDate = patientForCreation.StartDate;
            conditionDetail.OutcomeDate = patientForCreation.OutcomeDate;

            patientDetail.Conditions.Add(conditionDetail);

            return patientDetail;
        }

        /// <summary>
        /// Prepare the model for updating an existing patient
        /// </summary>
        private PatientDetailForCreation PreparePatientDetail(PatientForUpdateDto patientForUpdate)
        {
            var patientDetail = new PatientDetailForCreation();
            patientDetail.CustomAttributes = _modelExtensionBuilder.BuildModelExtension<Patient>();

            // Update patient custom attributes from source
            foreach (var newAttribute in patientForUpdate.Attributes)
            {
                var customAttribute = _customAttributeRepository.Get(ca => ca.Id == newAttribute.Key);
                if (customAttribute != null)
                {
                    // Validate attribute exists for household entity and is a PMT attribute
                    var attributeDetail = patientDetail.CustomAttributes.SingleOrDefault(ca => ca.AttributeKey == customAttribute.AttributeKey);
                    if (attributeDetail == null)
                    {
                        ModelState.AddModelError("Message", $"Unable to locate custom attribute on patient {newAttribute.Key}");
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

            return patientDetail;
        }
    }
}
