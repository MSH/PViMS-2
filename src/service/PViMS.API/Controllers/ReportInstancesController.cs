using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Application.Commands.ReportInstanceAggregate;
using PVIMS.API.Application.Queries.ReportInstanceAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;
using PVIMS.Core.CustomAttributes;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Entities.Keyless;
using SelectionDataItem = PVIMS.Core.Entities.SelectionDataItem;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class ReportInstancesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IRepositoryInt<ActivityExecutionStatusEvent> _activityExecutionStatusEventRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<Dataset> _datasetRepository;
        private readonly IRepositoryInt<DatasetInstance> _datasetInstanceRepository;
        private readonly IRepositoryInt<PatientClinicalEvent> _patientClinicalEventRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<ReportInstanceMedication> _reportInstanceMedicationRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IReportService _reportService;
        private readonly IWorkFlowService _workFlowService;
        private readonly IInfrastructureService _infrastructureService;
        private readonly IArtefactService _artefactService;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReportInstancesController> _logger;

        public ReportInstancesController(
            IMediator mediator,
            IPropertyMappingService propertyMappingService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<ActivityExecutionStatusEvent> activityExecutionStatusEventRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<Dataset> datasetRepository,
            IRepositoryInt<DatasetInstance> datasetInstanceRepository,
            IRepositoryInt<PatientClinicalEvent> patientClinicalEventRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<ReportInstanceMedication> reportInstanceMedicationRepository,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<WorkFlow> workFlowRepository,
            IReportService reportService,
            IInfrastructureService infrastructureService,
            IWorkFlowService workFlowService,
            IArtefactService artefactService,
            IUnitOfWorkInt unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ReportInstancesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _reportInstanceMedicationRepository = reportInstanceMedicationRepository ?? throw new ArgumentNullException(nameof(reportInstanceMedicationRepository));
            _activityExecutionStatusEventRepository = activityExecutionStatusEventRepository ?? throw new ArgumentNullException(nameof(activityExecutionStatusEventRepository));
            _patientClinicalEventRepository = patientClinicalEventRepository ?? throw new ArgumentNullException(nameof(patientClinicalEventRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _datasetRepository = datasetRepository ?? throw new ArgumentNullException(nameof(datasetRepository));
            _datasetInstanceRepository = datasetInstanceRepository ?? throw new ArgumentNullException(nameof(datasetInstanceRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _workFlowService = workFlowService ?? throw new ArgumentNullException(nameof(workFlowService));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _infrastructureService = infrastructureService ?? throw new ArgumentNullException(nameof(infrastructureService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single report instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult of type ReportInstanceIdentifierDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "GetReportInstanceByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<ReportInstanceIdentifierDto>> GetReportInstanceByIdentifier(Guid workFlowGuid, int id)
        {
            var mappedReportInstance = await GetReportInstanceAsync<ReportInstanceIdentifierDto>(workFlowGuid, id);
            if (mappedReportInstance == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForReportInstance<ReportInstanceIdentifierDto>(workFlowGuid, mappedReportInstance));
        }

        /// <summary>
        /// Get a single report instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult of type ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "GetReportInstanceByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<ReportInstanceDetailDto>> GetReportInstanceByDetail(Guid workFlowGuid, int id)
        {
            var query = new ReportInstanceDetailQuery(workFlowGuid, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceDetailQuery - {workFlowGuid}: {id}",
                workFlowGuid.ToString(),
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single report instance using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult of type ReportInstanceExpandedDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "GetReportInstanceByExpanded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.expanded.v1+json", "application/vnd.pvims.expanded.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<ReportInstanceExpandedDto>> GetReportInstanceByExpanded(Guid workFlowGuid, int id)
        {
            var query = new ReportInstanceExpandedQuery(workFlowGuid, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceExpandedQuery - {workFlowGuid}: {id}",
                workFlowGuid.ToString(),
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Download a summary for the patient
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique id of the report instance</param>
        /// <returns>An ActionResult</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{id}", Name = "DownloadPatientSummary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.patientsummary.v1+json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult> DownloadPatientSummary(Guid workFlowGuid, int id)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == id);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            var model = workFlowGuid == new Guid("892F3305-7819-4F18-8A87-11CBA3AEE219") ? 
                await _artefactService.CreatePatientSummaryForActiveReportAsync(reportInstanceFromRepo.ContextGuid) : 
                await _artefactService.CreatePatientSummaryForSpontaneousReportAsync(reportInstanceFromRepo.ContextGuid);

            return PhysicalFile(model.FullPath, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        /// <summary>
        /// Get all report instances using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetReportInstancesByDetail(Guid workFlowGuid, 
            [FromQuery] ReportInstanceResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesDetailQuery(workFlowGuid, 
                false, 
                false,
                reportInstanceResourceParameters.ActiveReportsOnly == Models.ValueTypes.YesNoValueType.Yes, 
                reportInstanceResourceParameters.SearchFrom,
                reportInstanceResourceParameters.SearchTo,
                reportInstanceResourceParameters.SearchTerm,
                reportInstanceResourceParameters.QualifiedName, 
                reportInstanceResourceParameters.PageNumber, 
                reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesDetailQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all new report instances using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetNewReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.new.v1+json", "application/vnd.pvims.new.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.new.v1+json", "application/vnd.pvims.new.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetNewReportInstancesByDetail(Guid workFlowGuid,
            [FromQuery] IdResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesAnalysisQuery(workFlowGuid, "", reportInstanceResourceParameters.PageNumber, reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesAnalysisQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all report instances for analysis using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetAnalysisReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.analysis.v1+json", "application/vnd.pvims.analysis.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.analysis.v1+json", "application/vnd.pvims.analysis.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetAnalysisReportInstancesByDetail(Guid workFlowGuid,
            [FromQuery] ReportInstanceActivityResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesAnalysisQuery(workFlowGuid, reportInstanceResourceParameters.QualifiedName, reportInstanceResourceParameters.PageNumber, reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesAnalysisQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Get all report instances for feedback using a valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ReportInstanceDetailDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetFeedbackReportInstancesByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.feedback.v1+json", "application/vnd.pvims.feedback.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.feedback.v1+json", "application/vnd.pvims.feedback.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        [Authorize(Roles = "Clinician")]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<ReportInstanceDetailDto>>> GetFeedbackReportInstancesByDetail(Guid workFlowGuid,
            [FromQuery] ReportInstanceActivityResourceParameters reportInstanceResourceParameters)
        {
            if (!_propertyMappingService.ValidMappingExistsFor<ReportInstanceDetailDto, ReportInstance>
               (reportInstanceResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new ReportInstancesFeedbackQuery(workFlowGuid, reportInstanceResourceParameters.QualifiedName, reportInstanceResourceParameters.PageNumber, reportInstanceResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: ReportInstancesFeedbackQuery - {workFlowGuid}",
                workFlowGuid.ToString());

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = reportInstanceResourceParameters.PageSize,
                currentPage = reportInstanceResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Download a file attachment
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportinstanceId">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="activityExecutionStatusEventId">The unique identifier of the activity</param>
        /// <param name="id">The unique identifier for the attachment</param>
        /// <returns>An ActionResult</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportinstanceId}/activity/{activityExecutionStatusEventId}/attachments/{id}", Name = "DownloadActivitySingleAttachment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.attachment.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult> DownloadActivitySingleAttachment(Guid workFlowGuid, int reportinstanceId, int activityExecutionStatusEventId, int  id)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == reportinstanceId);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            var activityExecutionStatusEvent = await _activityExecutionStatusEventRepository.GetAsync(f => f.Id == activityExecutionStatusEventId);
            if (activityExecutionStatusEvent == null)
            {
                return NotFound();
            }

            var attachmentFromRepo = activityExecutionStatusEvent.Attachments.SingleOrDefault(f => f.Id == id);
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
        /// Causality report
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="causalityReportResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type CausalityReportDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances", Name = "GetCausalityReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.causalityreport.v1+json", "application/vnd.pvims.causalityreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.causalityreport.v1+json", "application/vnd.pvims.causalityreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<CausalityReportDto>>> GetCausalityReport(Guid workFlowGuid, 
                        [FromQuery] CausalityReportResourceParameters causalityReportResourceParameters)
        {
            var workFlowFromRepo = await _workFlowRepository.GetAsync(f => f.WorkFlowGuid == workFlowGuid);
            if (workFlowFromRepo == null)
            {
                return NotFound();
            }

            if (causalityReportResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter parameters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetCausalityResults<CausalityReportDto>(causalityReportResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<CausalityReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForCausalityReport(wrapper, workFlowGuid, causalityReportResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Change report instance activity
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <param name="activityChange">The payload for setting the new status</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/activity", Name = "UpdateReportInstanceActivity")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceActivity(Guid workFlowGuid, int id,
            [FromBody] ActivityChangeDto activityChange)
        {
            if (activityChange == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeReportInstanceActivityCommand(workFlowGuid, id, activityChange.Comments, activityChange.CurrentExecutionStatus, activityChange.NewExecutionStatus, activityChange.ContextCode, activityChange.ContextDate);

            _logger.LogInformation(
                "----- Sending command: ChangeReportInstanceActivityCommand - {id}",
                id);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Change report classification
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that the report instance is associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <param name="reportInstanceClassificationForUpdate">The payload for setting the new classification</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/classification", Name = "UpdateReportInstanceClassification")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceClassification(Guid workFlowGuid, int id,
            [FromBody] ReportInstanceClassificationForUpdateDto reportInstanceClassificationForUpdate)
        {
            if (reportInstanceClassificationForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var reportClassification = ReportClassification.FromName(reportInstanceClassificationForUpdate.ReportClassification);
            var command = new ChangeReportClassificationCommand(workFlowGuid, id, reportClassification);

            _logger.LogInformation(
                "----- Sending command: ChangeReportClassificationCommand - {workFlowGuid}: {reportInstanceId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Set report instance terminology
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <param name="terminologyForUpdate">The payload for setting the new status</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/terminology", Name = "UpdateReportInstanceTerminology")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceTerminology(Guid workFlowGuid, int id,
            [FromBody] ReportInstanceTerminologyForUpdateDto terminologyForUpdate)
        {
            if (terminologyForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var command = new ChangeReportTerminologyCommand(workFlowGuid, id, terminologyForUpdate.TerminologyMedDraId);

            _logger.LogInformation(
                "----- Sending command: ChangeReportTerminologyCommand - {workFlowGuid}: {reportInstanceId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Create an E2B instance for the report
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="id">The unique identifier of the reporting instance</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{id}/createe2b", Name = "CreateE2BInstance")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateE2BInstance(Guid workFlowGuid, int id)
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == id);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, "E2BINITIATED") == false)
            {
                ModelState.AddModelError("Message", "Invalid status for activity");
            }

            if (ModelState.IsValid)
            {
                if (workFlowGuid == new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986"))
                {
                    await CreateE2BForSpontaneousAsync(reportInstanceFromRepo);
                }
                else
                {
                    await CreateE2BForActiveAsync(reportInstanceFromRepo);
                }    

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Set naranjo or who causality for a report instance medication
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="id">The unique identifier of the reporting instance medication</param>
        /// <param name="causalityForUpdate">The payload for setting the new causality</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/medications/{id}/causality", Name = "UpdateReportInstanceMedicationCausality")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceMedicationCausality(Guid workFlowGuid, int reportInstanceId, int id,
            [FromBody] ReportInstanceMedicationCausalityForUpdateDto causalityForUpdate)
        {
            if (causalityForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == reportInstanceId);
            if (reportInstanceFromRepo == null)
            {
                return NotFound();
            }

            var reportInstanceMedicationFromRepo = reportInstanceFromRepo.Medications.SingleOrDefault(m => m.Id == id);
            if (reportInstanceMedicationFromRepo == null)
            {
                return NotFound();
            }

            if (await _workFlowService.ValidateExecutionStatusForCurrentActivityAsync(reportInstanceFromRepo.ContextGuid, "CAUSALITYSET") == false)
            {
                ModelState.AddModelError("Message", "Invalid status for activity");
            }

            if (ModelState.IsValid)
            {
                if(causalityForUpdate.CausalityConfigType == CausalityConfigType.NaranjoScale)
                {
                    reportInstanceMedicationFromRepo.SetNaranjoCausality(causalityForUpdate.Causality);
                }
                if (causalityForUpdate.CausalityConfigType == CausalityConfigType.WHOScale)
                {
                    reportInstanceMedicationFromRepo.SetWhoCausality(causalityForUpdate.Causality);
                }

                _reportInstanceMedicationRepository.Update(reportInstanceMedicationFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get a single report instance task using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique id of the report instance</param>
        /// <param name="id">The unique id of the report instance task</param>
        /// <returns>An ActionResult of type ReportInstanceTaskIdentifierDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}", Name = "GetReportInstanceTaskByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ReportInstanceTaskIdentifierDto>> GetReportInstanceTaskByIdentifier(Guid workFlowGuid, int reportInstanceId, int id)
        {
            var query = new ReportInstanceTaskIdentifierQuery(workFlowGuid, reportInstanceId, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceTaskIdentifierQuery - {workFlowGuid}: {reportInstanceId}: {id}",
                workFlowGuid.ToString(),
                reportInstanceId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single report instance task using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique id of the report instance</param>
        /// <param name="id">The unique id of the report instance task</param>
        /// <returns>An ActionResult of type TaskDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}", Name = "GetReportInstanceTaskByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public async Task<ActionResult<TaskDto>> GetReportInstanceTaskByDetail(Guid workFlowGuid, int reportInstanceId, int id)
        {
            var query = new ReportInstanceTaskDetailQuery(workFlowGuid, reportInstanceId, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceTaskDetailQuery - {workFlowGuid}: {reportInstanceId}: {id}",
                workFlowGuid.ToString(),
                reportInstanceId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Add a new task to a report instance
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="reportInstanceTaskForCreation">The payload containing details of the task</param>
        /// <returns></returns>
        [HttpPost("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks", Name = "CreateReportInstanceTask")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateReportInstanceTask(Guid workFlowGuid, int reportInstanceId,
            [FromBody] ReportInstanceTaskForCreationDto reportInstanceTaskForCreation)
        {
            if (reportInstanceTaskForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for command");
                return BadRequest(ModelState);
            }

            var taskType = TaskType.FromName(reportInstanceTaskForCreation.TaskType.ToString());
            var command = new AddTaskToReportInstanceCommand(workFlowGuid, reportInstanceId, reportInstanceTaskForCreation.Source, reportInstanceTaskForCreation.Description, taskType);

            _logger.LogInformation(
                "----- Sending command: AddTaskToReportInstanceCommand - {workFlowGuid}: {reportInstanceId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetReportInstanceTaskByIdentifier",
                new
                {
                    workFlowGuid,
                    reportInstanceId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Change task details for report instance task
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="id">The unique identifier of the reporting instance task</param>
        /// <param name="reportInstanceTaskDetailsForUpdate">The payload for updating task details</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}/details", Name = "UpdateReportInstanceTaskDetails")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceTaskDetails(Guid workFlowGuid, int reportInstanceId, int id,
            [FromBody] ReportInstanceTaskDetailsForUpdateDto reportInstanceTaskDetailsForUpdate)
        {
            var command = new ChangeTaskDetailsCommand(workFlowGuid, reportInstanceId, id, reportInstanceTaskDetailsForUpdate.Source, reportInstanceTaskDetailsForUpdate.Description);

            _logger.LogInformation(
                "----- Sending command: ChangeTaskDetailsCommand - {workFlowGuid}: {reportInstanceId}: {id}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceTaskId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Change current status for report instance task
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="id">The unique identifier of the reporting instance task</param>
        /// <param name="reportInstanceTaskStatusForUpdate">The payload for updating task status</param>
        /// <returns></returns>
        [HttpPut("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{id}/status", Name = "UpdateReportInstanceTaskStatus")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateReportInstanceTaskStatus(Guid workFlowGuid, int reportInstanceId, int id,
            [FromBody] ReportInstanceTaskStatusForUpdateDto reportInstanceTaskStatusForUpdate)
        {
            var taskStatus = Core.Aggregates.ReportInstanceAggregate.TaskStatus.FromName(reportInstanceTaskStatusForUpdate.TaskStatus.ToString());
            var command = new ChangeTaskStatusCommand(workFlowGuid, reportInstanceId, id, taskStatus);

            _logger.LogInformation(
                "----- Sending command: ChangeTaskStatusCommand - {workFlowGuid}: {reportInstanceId}: {id}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceTaskId);

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get a single report instance task comment using it's unique id and valid media type 
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique id of the report instance</param>
        /// <param name="reportInstanceTaskId">The unique id of the report instance task</param>
        /// <param name="id">The unique id of the report instance task</param>
        /// <returns>An ActionResult of type ReportInstanceTaskCommentIdentifierDto</returns>
        [HttpGet("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{reportInstanceTaskId}/comments/{id}", Name = "GetReportInstanceTaskCommentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ReportInstanceTaskCommentIdentifierDto>> GetReportInstanceTaskCommentByIdentifier(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, int id)
        {
            var query = new ReportInstanceTaskCommentIdentifierQuery(workFlowGuid, reportInstanceId, reportInstanceTaskId, id);

            _logger.LogInformation(
                "----- Sending query: GetReportInstanceTaskCommentIdentifierQuery - {workFlowGuid}: {reportInstanceId}: {reportInstanceTaskId}: {id}",
                workFlowGuid.ToString(),
                reportInstanceId,
                reportInstanceTaskId,
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Add a new comment to a report instance task
        /// </summary>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="reportInstanceId">The unique identifier of the reporting instance</param>
        /// <param name="reportInstanceTaskId">The unique identifier of the reporting instance task</param>
        /// <param name="reportInstanceTaskCommentForCreation">The payload containing details of the task comment</param>
        /// <returns></returns>
        [HttpPost("workflow/{workFlowGuid}/reportinstances/{reportInstanceId}/tasks/{reportInstanceTaskId}/comments", Name = "CreateReportInstanceTaskComment")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateReportInstanceTaskComment(Guid workFlowGuid, int reportInstanceId, int reportInstanceTaskId, 
            [FromBody] ReportInstanceTaskCommentForCreationDto reportInstanceTaskCommentForCreation)
        {
            if (reportInstanceTaskCommentForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for command");
                return BadRequest(ModelState);
            }

            var command = new AddCommentToReportInstanceTaskCommand(workFlowGuid, reportInstanceId, reportInstanceTaskId, reportInstanceTaskCommentForCreation.Comment);

            _logger.LogInformation(
                "----- Sending command: AddCommentToReportInstanceTaskCommand - {workFlowGuid}: {reportInstanceId}: {reportInstanceTaskId}",
                command.WorkFlowGuid.ToString(),
                command.ReportInstanceId,
                command.ReportInstanceTaskId);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetReportInstanceTaskCommentByIdentifier",
                new
                {
                    workFlowGuid,
                    reportInstanceId,
                    reportInstanceTaskId,
                    id = commandResult.Id
                }, commandResult);
        }

        private async Task<T> GetReportInstanceAsync<T>(Guid workFlowGuid, long id) where T : class
        {
            var reportInstanceFromRepo = await _reportInstanceRepository.GetAsync(f => f.WorkFlow.WorkFlowGuid == workFlowGuid && f.Id == id, new string[] { "Activities", "Tasks" });

            if (reportInstanceFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedReportInstance = _mapper.Map<T>(reportInstanceFromRepo);

                return mappedReportInstance;
            }

            return null;
        }

        private ReportInstanceIdentifierDto CreateLinksForReportInstance<T>(Guid workFlowGuid, T dto)
        {
            ReportInstanceIdentifierDto identifier = (ReportInstanceIdentifierDto)(object)dto;

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateReportInstanceResourceUri(workFlowGuid, identifier.Id), "self", "GET"));

            var reportInstance = _reportInstanceRepository.Get(r => r.Id == identifier.Id, new string[] { "WorkFlow", "Activities.ExecutionEvents", "Activities.CurrentStatus" });
            if (reportInstance == null)
            {
                return identifier;
            }

            var currentActivityInstance = reportInstance.Activities.Single(a => a.Current == true);

            switch (reportInstance.CurrentActivity.QualifiedName)
            {
                case "Confirm Report Data":
                    if (currentActivityInstance.CurrentStatus.Description == "UNCONFIRMED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "confirm", "PUT"));
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "delete", "PUT"));
                    }

                    break;

                case "Set MedDRA and Causality":
                    identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceTerminology", workFlowGuid, identifier.Id), "setmeddra", "PUT"));

                    if (currentActivityInstance.CurrentStatus.Description != "NOTSET")
                    {
                        // ConfigType.AssessmentScale
                        var configValue = _configRepository.Get(c => c.ConfigType == ConfigType.AssessmentScale).ConfigValue;

                        if (configValue == "Both Scales" || configValue == "WHO Scale")
                        {
                            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "whocausalityset", "PUT"));
                        }

                        if (configValue == "Both Scales" || configValue == "Naranjo Scale")
                        {
                            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "naranjocausalityset", "PUT"));
                        }

                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "causalityset", "PUT"));
                    }

                    break;

                case "Extract E2B":
                    if (currentActivityInstance.CurrentStatus.Description == "NOTGENERATED" || currentActivityInstance.CurrentStatus.Description == "E2BSUBMITTED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("CreateE2BInstance", workFlowGuid, identifier.Id), "createe2b", "PUT"));
                    }
                    if (currentActivityInstance.CurrentStatus.Description == "E2BINITIATED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "preparereporte2b", "PUT"));

                        var evt = currentActivityInstance.ExecutionEvents
                            .OrderByDescending(ee => ee.EventDateTime)
                            .First(ee => ee.ExecutionStatus.Id == currentActivityInstance.CurrentStatus.Id);
                        var tag = (reportInstance.WorkFlow.Description == "New Active Surveilliance Report") ? "Active" : "Spontaneous";

                        var datasetInstance = _unitOfWork.Repository<DatasetInstance>()
                            .Queryable()
                            .Where(di => di.Tag == tag
                                && di.ContextId == evt.Id)
                            .SingleOrDefault();

                        if(datasetInstance != null)
                        {
                            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDatasetInstanceResourceUri(datasetInstance.Dataset.Id, datasetInstance.Id), "updatee2b", "PUT"));
                        }
                    }

                    if (currentActivityInstance.CurrentStatus.Description == "E2BGENERATED")
                    {
                        identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUriForReportInstance("UpdateReportInstanceStatus", workFlowGuid, identifier.Id), "confirmsubmissione2b", "PUT"));

                        var executionEvent = currentActivityInstance.ExecutionEvents
                            .OrderByDescending(ee => ee.EventDateTime)
                            .First(ee => ee.ExecutionStatus.Description == "E2BGENERATED");
                        if (executionEvent != null)
                        {
                            var e2bAttachment = executionEvent.Attachments.SingleOrDefault(att => att.Description == "E2b");
                            if (e2bAttachment != null)
                            {
                                identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateDownloadActivitySingleAttachmentResourceUri(workFlowGuid, reportInstance.Id, executionEvent.Id, e2bAttachment.Id), "downloadxml", "GET"));
                            }
                        }
                    }

                    break;

                default:
                    break;
            }

            var validRoles = new string[] { "RegClerk", "DataCap", "Clinician" };
            //if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report" && _userRoleRepository.Exists(ur => ur.User.Id == user.Id && validRoles.Contains(ur.Role.Key)))
            if (reportInstance.WorkFlow.Description == "New Active Surveilliance Report")
            {
                identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("Patient", identifier.Id), "viewpatient", "GET"));
            }

            if (reportInstance.WorkFlow.Description == "New Spontaneous Surveilliance Report")
            {
                var datasetInstance = _datasetInstanceRepository.Get(di => di.DatasetInstanceGuid == reportInstance.ContextGuid);
                if (datasetInstance != null)
                {
                    identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateUpdateDatasetInstanceResourceUri(datasetInstance.Dataset.Id, datasetInstance.Id), "updatespont", "PUT"));
                }
            }

            return identifier;
        }

        /// <summary>
        /// Get results from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="causalityReportResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetCausalityResults<T>(CausalityReportResourceParameters causalityReportResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = causalityReportResourceParameters.PageNumber,
                PageSize = causalityReportResourceParameters.PageSize
            };

            // Determine causality configuration
            var config = _infrastructureService.GetOrCreateConfig(ConfigType.AssessmentScale);
            var configValue = (CausalityConfigType)Enum.Parse(typeof(CausalityConfigType), config.ConfigValue.Replace(" ", ""));

            var resultsFromService = PagedCollection<CausalityNotSetList>.Create(_reportService.GetCausalityNotSetItems(
                causalityReportResourceParameters.SearchFrom,
                causalityReportResourceParameters.SearchTo,
                configValue,
                causalityReportResourceParameters.FacilityId,
                causalityReportResourceParameters.CausalityCriteria), pagingInfo.PageNumber, pagingInfo.PageSize);

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
        /// Create E2B dataset instance for an active report
        /// </summary>
        /// <returns></returns>
        private async Task CreateE2BForActiveAsync(ReportInstance reportInstanceFromRepo)
        {
            DatasetInstance datasetInstance = null;

            // Determine which E2B dataset to use
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.E2BVersion);
            if (config == null)
            {
                throw new KeyNotFoundException(nameof(config));
            }
            var datasetName = config.ConfigValue;

            var dataset = await _datasetRepository.GetAsync(d => d.DatasetName == datasetName, new string[] { "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType", "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType" });
            var patientClinicalEvent = await _patientClinicalEventRepository.GetAsync(p => p.PatientClinicalEventGuid == reportInstanceFromRepo.ContextGuid, new string[] { "Patient" });
            
            // Add activity and link E2B to new element
            var newActivityExecutionStatusEvent = await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "E2BINITIATED", "AUTOMATION: E2B dataset created", null, "");

            if (dataset != null && patientClinicalEvent != null)
            {
                datasetInstance = dataset.CreateInstance(newActivityExecutionStatusEvent.Id, null);
                datasetInstance.Tag = "Active";
                
                _unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);

                // Default values
                if (datasetName.Contains("(R2)"))
                {
                    datasetInstance.InitialiseValues(datasetInstance.Tag, null, patientClinicalEvent);

                    SetInstanceValuesForActiveRelease2(datasetInstance, patientClinicalEvent);
                }
                if (datasetName.Contains("(R3)"))
                {
                    var term = _workFlowService.GetTerminologyMedDraForReportInstance(patientClinicalEvent.PatientClinicalEventGuid);

                    datasetInstance.InitialiseValues(datasetInstance.Tag, null, patientClinicalEvent);

                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.2 Batch Number"), "MSH.PViMS-B01000" + patientClinicalEvent.Id.ToString());
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.5 Date of Batch Transmission"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.1 Message Identifier"), "MSH.PViMS-B01000" + patientClinicalEvent.Id.ToString() + "-" + DateTime.Now.ToString("mmsss"));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.4 Date of Message Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.1 Sender’s (case) Safety Report Unique Identifier"), String.Format("ZA-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), patientClinicalEvent.Id.ToString()));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.2 Date of Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.8.1 Worldwide Unique Case Identification Number"), String.Format("ZA-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), patientClinicalEvent.Id.ToString()));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.2.1b MedDRA Code for Reaction / Event"), term.DisplayName);
                }

                await _unitOfWork.CompleteAsync();
            }
        } // end of sub

        /// <summary>
        /// Create E2B dataset instance for a spontaneous report
        /// </summary>
        /// <returns></returns>
        private async Task CreateE2BForSpontaneousAsync(ReportInstance reportInstanceFromRepo)
        {
            DatasetInstance datasetInstance = null;

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _userRepository.Get(u => u.UserName == userName);

            // Determine which E2B dataset to use
            var config = await _configRepository.GetAsync(c => c.ConfigType == ConfigType.E2BVersion);
            if (config == null)
            {
                throw new KeyNotFoundException(nameof(config));
            }
            var datasetName = config.ConfigValue;

            var dataset = await _datasetRepository.GetAsync(d => d.DatasetName == datasetName, new string[] { "DatasetCategories.DatasetCategoryElements.DatasetElement.Field.FieldType", "DatasetCategories.DatasetCategoryElements.DatasetElement.DatasetElementSubs.Field.FieldType" });
            var sourceInstance = await _datasetInstanceRepository.GetAsync(ds => ds.DatasetInstanceGuid == reportInstanceFromRepo.ContextGuid, new string[] { "Dataset" });

            if (dataset != null && sourceInstance != null)
            {
                // Add activity and link E2B to new element
                var evt = await _workFlowService.ExecuteActivityAsync(reportInstanceFromRepo.ContextGuid, "E2BINITIATED", "AUTOMATION: E2B dataset created", null, "");

                datasetInstance = dataset.CreateInstance(evt.Id, null);
                datasetInstance.Tag = "Spontaneous";
                _unitOfWork.Repository<DatasetInstance>().Save(datasetInstance);

                // Default values
                if (datasetName.Contains("(R2)"))
                {
                    datasetInstance.InitialiseValues(datasetInstance.Tag, sourceInstance, null);
                    SetInstanceValuesForSpontaneousRelease2(datasetInstance, sourceInstance, user);
                }
                if (datasetName.Contains("(R3)"))
                {
                    datasetInstance.InitialiseValues(datasetInstance.Tag, sourceInstance, null);
                    SetInstanceValuesForSpontaneousRelease3(datasetInstance, sourceInstance, user, sourceInstance.Id);
                }

                await _unitOfWork.CompleteAsync();
            }
        }

        /// <summary>
        /// E2B data mapping for active datasets
        /// </summary>
        /// <returns></returns>
        private void SetInstanceValuesForActiveRelease2(DatasetInstance datasetInstance, PatientClinicalEvent patientClinicalEvent)
        {
            var reportInstance = _unitOfWork.Repository<ReportInstance>().Queryable().Single(ri => ri.ContextGuid == patientClinicalEvent.PatientClinicalEventGuid);

            // ************************************* ichicsrmessageheader
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "7FF710CB-C08C-4C35-925E-484B983F2135"), datasetInstance.Id.ToString("D8")); // Message Number
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "693614B6-D5D5-457E-A03B-EAAFA66E6FBD"), DateTime.Today.ToString("yyyyMMddhhmmss")); // Message Date

            // ************************************* safetyreport
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "6799CAD0-2A65-48A5-8734-0090D7C2D85E"), string.Format("PH.FDA.{0}", reportInstance.Id.ToString("D6"))); //Safety Report ID
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "9C92D382-03AF-4A52-9A2F-04A46ADA0F7E"), DateTime.Today.ToString("yyyyMMdd")); //Transmission Date 
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "AE53FEB2-FF27-4CD5-AD54-C3FFED1490B5"), "2"); //Report Type

            IExtendable pcExtended = patientClinicalEvent;
            var objectValue = pcExtended.GetAttributeValue("Is the adverse event serious?");
            var serious = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(serious))
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Is the adverse event serious?" && sdi.SelectionKey == serious).Value;
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), selectionValue == "Yes" ? "1=Yes" : "2=No"); //Serious
            }

            objectValue = pcExtended.GetAttributeValue("Seriousness");
            var seriousReason = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(seriousReason) && serious == "1")
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(si => si.AttributeKey == "Seriousness" && si.SelectionKey == seriousReason).Value;

                var sd = "2=No";
                var slt = "2=No";
                var sh = "2=No";
                var sdi = "2=No";
                var sca = "2=No";
                var so = "2=No";

                switch (selectionValue)
                {
                    case "Death":
                        sd = "1=Yes";
                        break;

                    case "Life threatening":
                        slt = "1=Yes";
                        break;

                    case "A congenital anomaly or birth defect":
                        sca = "1=Yes";
                        break;

                    case "Initial or prolonged hospitalization":
                        sh = "1=Yes";
                        break;

                    case "Persistent or significant disability or incapacity":
                        sdi = "1=Yes";
                        break;

                    case "A medically important event":
                        so = "1=Yes";
                        break;

                    default:
                        break;
                }

                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA"), sd); //Seriousness Death
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "26C6F08E-B80B-411E-BFDC-0506FE102253"), slt); //Seriousness Life Threatening
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "837154A9-D088-41C6-A9E2-8A0231128496"), sh); //Seriousness Hospitalization
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DDEBDEC0-2A90-49C7-970E-B7855CFDF19D"), sdi); //Seriousness Disabling
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DF89C98B-1D2A-4C8E-A753-02E265841F4F"), sca); //Seriousness Congenital Anomaly
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "33A75547-EF1B-42FB-8768-CD6EC52B24F8"), so); //Seriousness Other
            }

            objectValue = pcExtended.GetAttributeValue("Date of Report");
            var reportDate = objectValue != null ? objectValue.ToString() : "";
            DateTime tempdt;
            if (DateTime.TryParse(reportDate, out tempdt))
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "65ADEF15-961A-4558-B864-7832D276E0E3"), Convert.ToDateTime(reportDate).ToString("yyyyMMdd")); //Date report was first received
            }
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A10C704D-BC1D-445E-B084-9426A91DB63B"), DateTime.Today.ToString("yyyyMMdd")); //Date of most recent info

            // ************************************* primarysource
            objectValue = pcExtended.GetAttributeValue("Full Name of Reporter");
            var fullName = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(fullName))
            {
                if (fullName.Contains(" "))
                {
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C35D5F5A-D539-4EEE-B080-FF384D5FBE08"), fullName.Substring(0, fullName.IndexOf(" "))); //Reporter Given Name
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F214C619-EE0E-433E-8F52-83469778E418"), fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1))); //Reporter Family Name
                }
                else
                {
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C35D5F5A-D539-4EEE-B080-FF384D5FBE08"), fullName); //Reporter Given Name
                }
            }

            objectValue = pcExtended.GetAttributeValue("Type of Reporter");
            var profession = objectValue != null ? objectValue.ToString() : "";
            if (!String.IsNullOrWhiteSpace(profession))
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Type of Reporter" && sdi.SelectionKey == profession).Value;

                switch (selectionValue)
                {
                    case "Other health professional":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "3=Other Health Professional"); //Qualification
                        break;

                    case "Physician":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "1=Physician");
                        break;

                    case "Consumer or other non health professional":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "5=Consumer or other non health professional");
                        break;

                    case "Pharmacist":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "2=Pharmacist");
                        break;

                    case "Lawyer":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1D59E85E-66AF-4E70-B779-6AB873DE1E84"), "4=Lawyer");
                        break;

                    default:
                        break;
                }
            }

            // ************************************* sender
            var regAuth = _unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.RegulatoryAuthority);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Type"), "2=Regulatory Authority");
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Organization"), regAuth.OrganisationName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Given Name"), regAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Family Name"), regAuth.ContactSurname);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Street Address"), regAuth.StreetAddress);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender City"), regAuth.City);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender State"), regAuth.State);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Postcode"), regAuth.PostCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Number"), regAuth.ContactNumber);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Country Code"), regAuth.CountryCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Email Address"), regAuth.ContactEmail);

            // ************************************* receiver
            var repAuth = _unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.ReportingAuthority);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Type"), "5=WHO Collaborating Center for International Drug Monitoring");
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Organization"), repAuth.OrganisationName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Given Name"), repAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Family Name"), repAuth.ContactSurname);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Street Address"), repAuth.StreetAddress);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver City"), repAuth.City);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver State"), repAuth.State);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Postcode"), repAuth.PostCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel"), repAuth.ContactNumber);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel Country Code"), repAuth.CountryCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Email Address"), repAuth.ContactEmail);

            // ************************************* patient
            var init = String.Format("{0}{1}", patientClinicalEvent.Patient.FirstName.Substring(0, 1), patientClinicalEvent.Patient.Surname.Substring(0, 1));
            if (!String.IsNullOrWhiteSpace(init)) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "A0BEAB3A-0B0A-457E-B190-1B66FE60CA73"), init); }; //Patient Initial

            var dob = patientClinicalEvent.Patient.DateOfBirth;
            var onset = patientClinicalEvent.OnsetDate;
            var recovery = patientClinicalEvent.ResolutionDate;
            if (dob != null)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "4F71B7F4-4317-4680-B3A3-9C1C1F72AD6A"), Convert.ToDateTime(dob).ToString("yyyyMMdd")); //Patient Birthdate

                if (onset != null)
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "E10C259B-DD2C-4F19-9D41-16FDDF9C5807"), age.ToString()); //Patient Onset Age
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "CA9B94C2-E1EF-407B-87C3-181224AF637A"), "804=Day"); //Patient Onset Age Unit
                }
            }

            var encounter = _unitOfWork.Repository<Encounter>().Queryable().FirstOrDefault(e => e.Patient.Id == patientClinicalEvent.Patient.Id && e.Archived == false & e.EncounterDate <= patientClinicalEvent.OnsetDate);
            if (encounter != null)
            {
                var encounterInstance = _unitOfWork.Repository<DatasetInstance>().Queryable().SingleOrDefault(ds => ds.Dataset.DatasetName == "Chronic Treatment" && ds.ContextId == encounter.Id);
                if (encounterInstance != null)
                {
                    var weight = encounterInstance.GetInstanceValue("Weight (kg)");
                    if (!String.IsNullOrWhiteSpace(weight)) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "89A6E687-A220-4319-AAC1-AFBB55C81873"), weight); }; //Patient Weight

                    var height = encounterInstance.GetInstanceValue("Height (cm)");
                    if (!String.IsNullOrWhiteSpace(height)) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "40DAD435-8282-4B3E-B65E-3478FF55D028"), height); }; //Patient Height

                    var lmp = encounterInstance.GetInstanceValue("Date of last menstrual period");
                    if (!String.IsNullOrWhiteSpace(lmp)) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "93253F91-60D1-4161-AF1A-F3ABDD140CB9"), Convert.ToDateTime(lmp).ToString("yyyyMMdd")); }; //Patient Last Menstrual Date

                    var gest = encounterInstance.GetInstanceValue("Estimated gestation (weeks)");
                    if (!String.IsNullOrWhiteSpace(gest))
                    {
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B6BE9689-B6B2-4FCF-8918-664AFC91A4E0"), gest); //Gestation Period
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1F174413-2A1E-45BD-B5C4-0C8F5DFFBFF4"), "803=Week");  //Gestation Period Unit
                    };
                }
            }

            // ************************************* reaction
            var terminologyMedDra = _workFlowService.GetTerminologyMedDraForReportInstance(patientClinicalEvent.PatientClinicalEventGuid);
            var term = terminologyMedDra != null ? terminologyMedDra.DisplayName : "";
            if (!String.IsNullOrWhiteSpace(term)) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "C8DD9A5E-BD9A-488D-8ABF-171271F5D370"), term); }; //Reaction MedDRA LLT
            if (onset != null) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "1EAD9E11-60E6-4B27-9A4D-4B296B169E90"), Convert.ToDateTime(onset).ToString("yyyyMMdd")); }; //Reaction Start Date
            if (recovery != null) { datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "3A0F240E-8B36-48F6-9527-77E55F6E7CF1"), Convert.ToDateTime(recovery).ToString("yyyyMMdd")); }; // Reaction End Date
            if (onset != null && recovery != null)
            {
                var rduration = (Convert.ToDateTime(recovery) - Convert.ToDateTime(onset)).Days;
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "0712C664-2ADD-44C0-B8D5-B6E83FB01F42"), rduration.ToString()); //Reaction Duration
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "F96E702D-DCC5-455A-AB45-CAEFF25BF82A"), "804=Day"); //Reaction Duration Unit
            }

            // ************************************* test
            var destinationTestElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "693A2E8C-B041-46E7-8687-0A42E6B3C82E"); // Test History
            foreach (PatientLabTest labTest in patientClinicalEvent.Patient.PatientLabTests.Where(lt => lt.TestDate >= patientClinicalEvent.OnsetDate).OrderByDescending(lt => lt.TestDate))
            {
                var newContext = Guid.NewGuid();

                datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Date"), labTest.TestDate.ToString("yyyyMMdd"), (Guid)newContext);
                datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Name"), labTest.LabTest.Description, (Guid)newContext);

                var testResult = !String.IsNullOrWhiteSpace(labTest.LabValue) ? labTest.LabValue : !String.IsNullOrWhiteSpace(labTest.TestResult) ? labTest.TestResult : "";
                datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Result"), testResult, (Guid)newContext);

                var testUnit = labTest.TestUnit != null ? labTest.TestUnit.Description : "";
                if (!String.IsNullOrWhiteSpace(testUnit)) { datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Test Unit"), testUnit, (Guid)newContext); };

                var lowRange = labTest.ReferenceLower;
                if (!String.IsNullOrWhiteSpace(lowRange)) { datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "Low Test Range"), lowRange, (Guid)newContext); };

                var highRange = labTest.ReferenceUpper;
                if (!String.IsNullOrWhiteSpace(highRange)) { datasetInstance.SetInstanceSubValue(destinationTestElement.DatasetElementSubs.Single(des => des.ElementName == "High Test Range"), highRange, (Guid)newContext); };
            }

            // ************************************* drug
            string[] validNaranjoCriteria = { "Possible", "Probable", "Definite" };
            string[] validWHOCriteria = { "Possible", "Probable", "Certain" };

            var destinationProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.DatasetElementGuid.ToString() == "E033BDE8-EDC8-43FF-A6B0-DEA6D6FA581C"); // Medicinal Products
            foreach (ReportInstanceMedication med in reportInstance.Medications)
            {
                var newContext = Guid.NewGuid();

                var patientMedication = _unitOfWork.Repository<PatientMedication>().Queryable().Single(pm => pm.PatientMedicationGuid == med.ReportInstanceMedicationGuid);
                IExtendable mcExtended = patientMedication;

                var character = "";
                character = (validNaranjoCriteria.Contains(med.NaranjoCausality) || validWHOCriteria.Contains(med.WhoCausality)) ? "1=Suspect" : "2=Concomitant";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Characterization"), character, (Guid)newContext);

                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Medicinal Product"), patientMedication.Concept.ConceptName, (Guid)newContext);

                objectValue = mcExtended.GetAttributeValue("Batch Number");
                var batchNumber = objectValue != null ? objectValue.ToString() : "";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Batch Number"), batchNumber, (Guid)newContext);

                objectValue = mcExtended.GetAttributeValue("Comments");
                var comments = objectValue != null ? objectValue.ToString() : "";
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Additional Information"), comments, (Guid)newContext);

                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Text"), patientMedication.Dose, (Guid)newContext);

                var form = patientMedication?.Concept?.MedicationForm.Description;
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Dosage Form"), form, (Guid)newContext);

                var startdate = patientMedication.StartDate;
                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Start Date"), startdate.ToString("yyyyMMdd"), (Guid)newContext);

                var enddate = patientMedication.EndDate;
                if (enddate.HasValue)
                {
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug End Date"), Convert.ToDateTime(enddate).ToString("yyyyMMdd"), (Guid)newContext);

                    var rduration = (Convert.ToDateTime(enddate) - Convert.ToDateTime(startdate)).Days;
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration"), rduration.ToString(), (Guid)newContext);
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration Unit"), "804=Day", (Guid)newContext);
                }

                var doseUnit = MapDoseUnitForActive(patientMedication.DoseUnit);
                if (!string.IsNullOrWhiteSpace(doseUnit)) { datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Structured Dosage Unit"), doseUnit, (Guid)newContext); };

                objectValue = mcExtended.GetAttributeValue("Clinician action taken with regard to medicine if related to AE");
                var drugAction = objectValue != null ? objectValue.ToString() : "";
                if (!string.IsNullOrWhiteSpace(drugAction)) { drugAction = MapDrugActionForActive(drugAction); };
                if (!string.IsNullOrWhiteSpace(drugAction)) { datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Action"), doseUnit, (Guid)newContext); };

                // Causality
                if (med.WhoCausality != null)
                {
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), patientClinicalEvent.SourceDescription, (Guid)newContext);
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "WHO Causality Scale", (Guid)newContext);
                    datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.WhoCausality.ToLowerInvariant() == "ignored" ? "" : med.WhoCausality, (Guid)newContext);
                }
                else
                {
                    if (med.NaranjoCausality != null)
                    {
                        datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), patientClinicalEvent.SourceDescription, (Guid)newContext);
                        datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "Naranjo Causality Scale", (Guid)newContext);
                        datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), med.NaranjoCausality.ToLowerInvariant() == "ignored" ? "" : med.NaranjoCausality, (Guid)newContext);
                    }
                }
            } // foreach (ReportInstanceMedication med in reportInstance.Medications)

        } // end of sub

        /// <summary>
        /// E2B data mapping for spontaneous datasets using R2
        /// </summary>
        /// <returns></returns>
        private void SetInstanceValuesForSpontaneousRelease2(DatasetInstance datasetInstance, DatasetInstance sourceInstance, User currentUser)
        {
            // ************************************* ichicsrmessageheader
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Message Header").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Message Number").DatasetElement, datasetInstance.Id.ToString("D8"));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Message Header").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Message Date").DatasetElement, DateTime.Today.ToString("yyyyMMddhhmmss"));

            // ************************************* safetyreport
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Safety Report ID").DatasetElement, string.Format("PH-FDA-{0}", sourceInstance.Id.ToString("D5")));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Transmission Date").DatasetElement, DateTime.Today.ToString("yyyyMMdd"));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Report Type").DatasetElement, "1");

            var seriousReason = sourceInstance.GetInstanceValue("Reaction serious details");
            if (!String.IsNullOrWhiteSpace(seriousReason))
            {
                var sd = "2=No";
                var slt = "2=No";
                var sh = "2=No";
                var sdi = "2=No";
                var sca = "2=No";
                var so = "2=No";

                switch (seriousReason)
                {
                    case "Resulted in death":
                        sd = "1=Yes";
                        break;

                    case "Is life-threatening":
                        slt = "1=Yes";
                        break;

                    case "Is a congenital anomaly/birth defect":
                        sca = "1=Yes";
                        break;

                    case "Requires hospitalization or longer stay in hospital":
                        sh = "1=Yes";
                        break;

                    case "Results in persistent or significant disability/incapacity (as per reporter's opinion)":
                        sdi = "1=Yes";
                        break;

                    case "Other medically important condition":
                        so = "1=Yes";
                        break;

                    default:
                        break;
                }

                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "B4EA6CBF-2D9C-482D-918A-36ABB0C96EFA"), sd); //Seriousness Death
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "26C6F08E-B80B-411E-BFDC-0506FE102253"), slt); //Seriousness Life Threatening
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "837154A9-D088-41C6-A9E2-8A0231128496"), sh); //Seriousness Hospitalization
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DDEBDEC0-2A90-49C7-970E-B7855CFDF19D"), sdi); //Seriousness Disabling
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "DF89C98B-1D2A-4C8E-A753-02E265841F4F"), sca); //Seriousness Congenital Anomaly
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "33A75547-EF1B-42FB-8768-CD6EC52B24F8"), so); //Seriousness Other
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), "1"); //Serious
            }
            else
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.DatasetElementGuid.ToString() == "510EB752-2D75-4DC3-8502-A4FCDC8A621A"), "2"); //Serious
            }

            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date report was first received").DatasetElement, sourceInstance.Created.ToString("yyyyMMdd"));
            datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Safety Report").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Date of most recent info").DatasetElement, sourceInstance.Created.ToString("yyyyMMdd"));

            // ************************************* primarysource
            var fullName = sourceInstance.GetInstanceValue("Reporter Name");
            if (!String.IsNullOrWhiteSpace(fullName))
            {
                if (fullName.Contains(" "))
                {
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Given Name").DatasetElement, fullName.Substring(0, fullName.IndexOf(" ")));
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Family Name").DatasetElement, fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1)));
                }
                else
                {
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Primary Source").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reporter Given Name").DatasetElement, fullName);
                }
            }

            // ************************************* sender
            var regAuth = _unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.RegulatoryAuthority);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Type"), "2=Regulatory Authority");
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Organization"), regAuth.OrganisationName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Given Name"), regAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Family Name"), regAuth.ContactSurname);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Street Address"), regAuth.StreetAddress);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender City"), regAuth.City);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender State"), regAuth.State);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Postcode"), regAuth.PostCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Number"), regAuth.ContactNumber);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Tel Country Code"), regAuth.CountryCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Sender Email Address"), regAuth.ContactEmail);

            // ************************************* receiver
            var repAuth = _unitOfWork.Repository<SiteContactDetail>().Queryable().Single(cd => cd.ContactType == ContactType.ReportingAuthority);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Type"), "5=WHO Collaborating Center for International Drug Monitoring");
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Organization"), repAuth.OrganisationName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Given Name"), repAuth.ContactFirstName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Family Name"), repAuth.ContactSurname);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Street Address"), repAuth.StreetAddress);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver City"), repAuth.City);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver State"), repAuth.State);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Postcode"), repAuth.PostCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel"), repAuth.ContactNumber);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Tel Country Code"), repAuth.CountryCode);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Receiver Email Address"), repAuth.ContactEmail);

            // ************************************* patient
            var dob = sourceInstance.GetInstanceValue("Date of Birth");
            var onset = sourceInstance.GetInstanceValue("Reaction known start date");
            var recovery = sourceInstance.GetInstanceValue("Reaction date of recovery");
            if (!String.IsNullOrWhiteSpace(dob))
            {
                datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Birthdate").DatasetElement, Convert.ToDateTime(dob).ToString("yyyyMMdd"));

                if (!String.IsNullOrWhiteSpace(onset))
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Onset Age").DatasetElement, age.ToString());
                    datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Patient").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Patient Onset Age Unit").DatasetElement, "804=Day");
                }
            }

            // ************************************* reaction
            var term = sourceInstance.GetInstanceValue("TerminologyMedDra");
            var termOut = "NOT SET";
            if (!String.IsNullOrWhiteSpace(term))
            {
                var termid = Convert.ToInt32(term);
                termOut = _unitOfWork.Repository<TerminologyMedDra>().Queryable().Single(u => u.Id == termid).DisplayName;
            };
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "Reaction MedDRA LLT"), termOut);
            if (!String.IsNullOrWhiteSpace(onset)) { datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction Start Date").DatasetElement, Convert.ToDateTime(onset).ToString("yyyyMMdd")); };
            if (!String.IsNullOrWhiteSpace(onset) && !String.IsNullOrWhiteSpace(recovery))
            {
                var rduration = (Convert.ToDateTime(recovery) - Convert.ToDateTime(onset)).Days;
                datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction Duration").DatasetElement, rduration.ToString());
                datasetInstance.SetInstanceValue(datasetInstance.Dataset.DatasetCategories.Single(dc => dc.DatasetCategoryName == "Reaction").DatasetCategoryElements.Single(dce => dce.DatasetElement.ElementName == "Reaction Duration Unit").DatasetElement, "804=Day");
            }

            // ************************************* drug
            var destinationProductElement = _unitOfWork.Repository<DatasetElement>().Queryable().SingleOrDefault(u => u.ElementName == "Medicinal Products");
            var sourceContexts = sourceInstance.GetInstanceSubValuesContext("Product Information");
            foreach (Guid sourceContext in sourceContexts)
            {
                var drugItemValues = sourceInstance.GetInstanceSubValues("Product Information", sourceContext);
                var drugName = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Product").InstanceValue;

                if (drugName != string.Empty)
                {
                    Guid? newContext = datasetInstance.GetContextForInstanceSubValue(destinationProductElement, destinationProductElement.DatasetElementSubs.SingleOrDefault(des => des.ElementName == "Medicinal Product"), drugName);
                    if (newContext != null)
                    {
                        var reportInstanceMedication = _unitOfWork.Repository<ReportInstanceMedication>().Queryable().Single(x => x.ReportInstanceMedicationGuid == sourceContext);

                        // Causality
                        if (reportInstanceMedication.WhoCausality != null)
                        {
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), sourceInstance.GetInstanceValue("Description of reaction"), (Guid)newContext);
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "WHO Causality Scale", (Guid)newContext);
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), reportInstanceMedication.WhoCausality.ToLowerInvariant() == "ignored" ? "" : reportInstanceMedication.WhoCausality, (Guid)newContext);
                        }
                        else
                        {
                            if (reportInstanceMedication.NaranjoCausality != null)
                            {
                                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Reaction Assessment"), sourceInstance.GetInstanceValue("Description of reaction"), (Guid)newContext);
                                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Method of Assessment"), "Naranjo Causality Scale", (Guid)newContext);
                                datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Assessment Result"), reportInstanceMedication.NaranjoCausality.ToLowerInvariant() == "ignored" ? "" : reportInstanceMedication.NaranjoCausality, (Guid)newContext);
                            }
                        }

                        // Treatment Duration
                        var startValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Start Date");
                        var endValue = drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug End Date");
                        if (startValue != null && endValue != null)
                        {
                            var rduration = (Convert.ToDateTime(endValue.InstanceValue) - Convert.ToDateTime(startValue.InstanceValue)).Days;
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration"), rduration.ToString(), (Guid)newContext);
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Drug Treatment Duration Unit"), "804=Day", (Guid)newContext);
                        }

                        // Dosage
                        if (drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Strength") != null && drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose Number") != null)
                        {
                            decimal strength = ConvertValueToDecimal(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Drug Strength").InstanceValue);
                            decimal dosage = ConvertValueToDecimal(drugItemValues.SingleOrDefault(div => div.DatasetElementSub.ElementName == "Dose Number").InstanceValue);

                            decimal dosageCalc = strength * dosage;
                            datasetInstance.SetInstanceSubValue(destinationProductElement.DatasetElementSubs.Single(des => des.ElementName == "Structured Dosage"), dosageCalc.ToString(), (Guid)newContext);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// E2B data mapping for spontaneous datasets using R3
        /// </summary>
        /// <returns></returns>
        private void SetInstanceValuesForSpontaneousRelease3(DatasetInstance datasetInstance, DatasetInstance sourceInstance, User currentUser, int datasetInstanceId)
        {
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.2 Batch Number"), "MSH.PViMS-B01000" + datasetInstanceId.ToString());
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.1.5 Date of Batch Transmission"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.1 Message Identifier"), "MSH.PViMS-B01000" + datasetInstanceId.ToString() + "-" + DateTime.Now.ToString("mmsss"));
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "N.2.r.4 Date of Message Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.1 Sender’s (case) Safety Report Unique Identifier"), String.Format("US-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), datasetInstanceId.ToString()));
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.2 Date of Creation"), DateTime.Now.ToString("yyyyMMddHHmmsss"));
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.8.1 Worldwide Unique Case Identification Number"), String.Format("US-MSH.PViMS-{0}-{1}", DateTime.Today.ToString("yyyy"), datasetInstanceId.ToString()));

            // Default remaining fields
            // C1 Identification
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.3 Type of Report"), "1=Spontaneous report");
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.1.4 Date Report Was First Received from Source"), sourceInstance.Created.ToString("yyyy-MM-dd"));

            // C2 Primary Source
            var fullName = sourceInstance.GetInstanceValue("Reporter Name");
            if (fullName != string.Empty)
            {
                if (fullName.Contains(" "))
                {
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.2 Reporter’s Given Name"), fullName.Substring(0, fullName.IndexOf(" ")));
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.4 Reporter’s Family Name"), fullName.Substring(fullName.IndexOf(" ") + 1, fullName.Length - (fullName.IndexOf(" ") + 1)));
                }
                else
                {
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.1.2 Reporter’s Given Name"), fullName);
                }
            }
            var place = sourceInstance.GetInstanceValue("Reporter Place of Practise");
            if (place != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.1 Reporter’s Organisation"), place);
            }
            var address = sourceInstance.GetInstanceValue("Reporter Address");
            if (address != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.3 Reporter’s Street"), address.Substring(0, 99));
            }
            var telNo = sourceInstance.GetInstanceValue("Reporter Telephone Number");
            if (telNo != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.2.r.2.7 Reporter’s Telephone"), telNo);
            }

            // C3 Sender
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.3.3 Sender’s Given Name"), currentUser.FirstName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.3.5 Sender’s Family Name"), currentUser.LastName);
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "C.3.4.8 Sender’s E-mail Address"), currentUser.Email);

            // D Patient
            var dob = sourceInstance.GetInstanceValue("Date of Birth");
            var onset = sourceInstance.GetInstanceValue("Date of Onset");
            if (dob != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.1 Date of Birth"), dob);

                if (onset != string.Empty)
                {
                    var age = (Convert.ToDateTime(onset) - Convert.ToDateTime(dob)).Days;
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.2a Age at Time of Onset of Reaction / Event"), age.ToString());
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.2.2bAge at Time of Onset of Reaction / Event (unit)"), "Day");
                }
            }
            var weight = sourceInstance.GetInstanceValue("Weight (kg)");
            if (weight != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.3 Body Weight (kg)"), weight);
            }
            var sex = sourceInstance.GetInstanceValue("Sex");
            if (sex != string.Empty)
            {
                if (sex == "Male")
                {
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.5 Sex"), "1=Male");
                }
                if (sex == "Female")
                {
                    datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.5 Sex"), "2=Female");
                }
            }
            var death = sourceInstance.GetInstanceValue("Reaction date of death");
            if (death != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "D.9.1 Date of Death"), death);
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.3.2a Results in Death"), "Yes");
            }

            // E Reaction
            var evnt = sourceInstance.GetInstanceValue("Description of reaction");
            if (evnt != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.1.1a Reaction / Event as Reported by the Primary Source in Native Language"), evnt);
            }

            var term = sourceInstance.GetInstanceValue("TerminologyMedDra");
            var termOut = "NOT SET";
            if (term != string.Empty)
            {
                var termid = Convert.ToInt32(term);
                termOut = _unitOfWork.Repository<TerminologyMedDra>().Queryable().Single(u => u.Id == termid).DisplayName;
            };
            datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.2.1b MedDRA Code for Reaction / Event"), termOut);

            if (onset != string.Empty)
            {
                datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.4 Date of Start of Reaction / Event"), onset);
            }

            var outcome = sourceInstance.GetInstanceValue("Outcome of reaction");
            if (outcome != string.Empty)
            {
                switch (outcome)
                {
                    case "Died - Drug may be contributory":
                    case "Died - Due to adverse reaction":
                    case "Died - Unrelated to drug":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "5=fatal");
                        break;

                    case "Not yet recovered":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "3=not recovered/not resolved/ongoing");
                        break;

                    case "Recovered":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "1=recovered/resolved");
                        break;

                    case "Uncertain outcome":
                        datasetInstance.SetInstanceValue(_unitOfWork.Repository<DatasetElement>().Queryable().Single(dse => dse.ElementName == "E.i.7 Outcome of Reaction / Event at the Time of Last Observation"), "0=unknown");
                        break;

                    default:
                        break;
                }
            }

            for (int i = 1; i <= 6; i++)
            {
                var drugId = 0;
                var elementName = "";
                var drugName = "";
                var tempi = 0;

                if (i < 4)
                {
                    drugId = i;
                    elementName = string.Format("Suspected Drug {0}", drugId);
                    drugName = sourceInstance.GetInstanceValue(elementName);

                    if (drugName != string.Empty)
                    {
                        // Create a new context
                        var context = Guid.NewGuid();

                        datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.1 Characterisation of Drug Role"), "1=Suspect", context);
                        datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.2.2 Medicinal Product Name as Reported by the Primary Source"), drugName, context);

                        elementName = string.Format("Suspected Drug {0} Dosage", drugId);
                        var dosage = sourceInstance.GetInstanceValue(elementName);
                        if (dosage != string.Empty)
                        {
                            if (Int32.TryParse(dosage, out tempi))
                            {
                                datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1a Dose (number)"), dosage, context);
                            }
                        }
                        elementName = string.Format("Suspected Drug {0} Dosage Unit", drugId);
                        var dosageUnit = sourceInstance.GetInstanceValue(elementName);
                        if (dosageUnit != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1b Dose (unit)"), dosageUnit, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Date Started", drugId);
                        var dateStarted = sourceInstance.GetInstanceValue(elementName);
                        if (dateStarted != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.4 Date of Start of Drug"), dateStarted, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Date Stopped", drugId);
                        var dateStopped = sourceInstance.GetInstanceValue(elementName);
                        if (dateStopped != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.5 Date of Last Administration"), dateStopped, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Batch Number", drugId);
                        var batch = sourceInstance.GetInstanceValue(elementName);
                        if (batch != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.7 Batch / Lot Number"), batch, context);
                        }
                        elementName = string.Format("Suspected Drug {0} Route", drugId);
                        var route = sourceInstance.GetInstanceValue(elementName);
                        if (route != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.10.1 Route of Administration"), route, context);
                        }
                    }
                }
                else
                {
                    drugId = i - 3;
                    elementName = string.Format("Concomitant Drug {0}", drugId);
                    drugName = sourceInstance.GetInstanceValue(elementName);

                    if (drugName != string.Empty)
                    {
                        // Create a new context
                        var context = Guid.NewGuid();

                        datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.1 Characterisation of Drug Role"), "1=Suspect", context);
                        datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.2.2 Medicinal Product Name as Reported by the Primary Source"), drugName, context);

                        elementName = string.Format("Concomitant Drug {0} Dosage", drugId);
                        var dosage = sourceInstance.GetInstanceValue(elementName);
                        if (dosage != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1a Dose (number)"), dosage, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Dosage Unit", drugId);
                        var dosageUnit = sourceInstance.GetInstanceValue(elementName);
                        if (dosageUnit != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.1b Dose (unit)"), dosageUnit, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Date Started", drugId);
                        var dateStarted = sourceInstance.GetInstanceValue(elementName);
                        if (dateStarted != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.4 Date of Start of Drug"), dateStarted, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Date Stopped", drugId);
                        var dateStopped = sourceInstance.GetInstanceValue(elementName);
                        if (dateStopped != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.5 Date of Last Administration"), dateStopped, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Batch Number", drugId);
                        var batch = sourceInstance.GetInstanceValue(elementName);
                        if (batch != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.7 Batch / Lot Number"), batch, context);
                        }
                        elementName = string.Format("Concomitant Drug {0} Route", drugId);
                        var route = sourceInstance.GetInstanceValue(elementName);
                        if (route != string.Empty)
                        {
                            datasetInstance.SetInstanceSubValue(_unitOfWork.Repository<DatasetElementSub>().Queryable().Single(dse => dse.ElementName == "G.k.4.r.10.1 Route of Administration"), route, context);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generic function to convert a string based value to a decimal
        /// </summary>
        /// <returns></returns>
        private decimal ConvertValueToDecimal(string value)
        {
            decimal tempdec = 0;
            if (Decimal.TryParse(value, out tempdec))
            {
                return tempdec;
            }
            else
            {
                return decimal.MinValue;
            };
        }

        /// <summary>
        /// E2B data mapping for active datasets
        /// </summary>
        /// <returns></returns>
        private string MapDoseUnitForActive(string doseUnit)
        {
            switch (doseUnit)
            {
                case "Bq":
                    return "014=Bq becquerel(s)";

                case "Ci":
                    return "018=Ci curie(s)";

                case "{DF}":
                    return "032=DF dosage form";

                case "[drp]":
                    return "031=Gtt drop(s)";

                case "GBq":
                    return "015=GBq gigabecquerel(s)";

                case "g":
                    return "002=G gram(s)";

                case "[iU]":
                    return "025=Iu international unit(s)";

                case "[iU]/kg":
                    return "028=iu/kg iu/kilogram";

                case "kBq":
                    return "017=Kbq kilobecquerel(s)";

                case "kg":
                    return "001=kg kilogram(s)";

                case "k[iU]":
                    return "026=Kiu iu(1000s)";

                case "L":
                    return "011=l litre(s)";

                case "MBq":
                    return "016=MBq megabecquerel(s)";

                case "M[iU]":
                    return "027=Miu iu(1,000,000s)";

                case "uCi":
                    return "020=uCi microcurie(s)";

                case "ug":
                    return "004=ug microgram(s)";

                case "ug/kg":
                    return "007=mg/kg milligram(s)/kilogram";

                case "uL":
                    return "013=ul microlitre(s)";

                case "mCi":
                    return "019=MCi millicurie(s)";

                case "meq":
                    return "029=Meq milliequivalent(s)";

                case "mg":
                    return "003=Mg milligram(s)";

                case "mg/kg":
                    return "007=mg/kg milligram(s)/kilogram";

                case "mg/m2":
                    return "009=mg/m 2 milligram(s)/sq. meter";

                case "ug/m2":
                    return "010=ug/ m 2 microgram(s)/ sq. Meter";

                case "mL":
                    return "012=ml millilitre(s)";

                case "mmol":
                    return "023=Mmol millimole(s)";

                case "mol":
                    return "022=Mol mole(s)";

                case "nCi":
                    return "021=NCi nanocurie(s)";

                case "ng":
                    return "005=ng nanogram(s)";

                case "%":
                    return "030=% percent";

                case "pg":
                    return "006=pg picogram(s)";

                default:
                    break;
            }

            return "";
        }

        /// <summary>
        /// E2B data mapping for active datasets
        /// </summary>
        /// <returns></returns>
        private string MapDrugActionForActive(string drugAction)
        {
            if (!String.IsNullOrWhiteSpace(drugAction))
            {
                var selectionValue = _unitOfWork.Repository<SelectionDataItem>().Queryable().Single(sdi => sdi.AttributeKey == "Clinician action taken with regard to medicine if related to AE" && sdi.SelectionKey == drugAction).Value;

                switch (selectionValue)
                {
                    case "Dose not changed":
                        return "4=Dose not changed";

                    case "Dose reduced":
                        return "2=Dose reduced";

                    case "Drug interrupted":
                        return "5=Unknown";

                    case "Drug withdrawn":
                        return "1=Drug withdrawn";

                    case "Not applicable":
                        return "6=Not applicable";

                    default:
                        break;
                }
            } // if (!String.IsNullOrWhiteSpace(drugAction))

            return "";
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="workFlowGuid">The unique identifier of the work flow that report instances are associated to</param>
        /// <param name="causalityReportResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForCausalityReport(
            LinkedResourceBaseDto wrapper,
            Guid workFlowGuid,
            CausalityReportResourceParameters causalityReportResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateCausalityReportResourceUri(workFlowGuid, ResourceUriType.Current, causalityReportResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateCausalityReportResourceUri(workFlowGuid, ResourceUriType.NextPage, causalityReportResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateCausalityReportResourceUri(workFlowGuid, ResourceUriType.PreviousPage, causalityReportResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }
    }
}
