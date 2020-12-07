using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using PVIMS.API.Attributes;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VPS.Common.Repositories;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class WorkFlowsController : ControllerBase
    {
        private readonly IRepositoryInt<WorkFlow> _workFlowRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IArtefactService _artefactService;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WorkFlowsController(IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<WorkFlow> workFlowRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<User> userRepository,
            IArtefactService artefactService,
            IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _workFlowRepository = workFlowRepository ?? throw new ArgumentNullException(nameof(workFlowRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _artefactService = artefactService ?? throw new ArgumentNullException(nameof(artefactService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Get a single workFlow using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the workFlow</param>
        /// <returns>An ActionResult of type WorkFlowIdentifierDto</returns>
        [HttpGet("workflow/{id}", Name = "GetWorkFlowByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<WorkFlowIdentifierDto>> GetWorkFlowByIdentifier(Guid id)
        {
            var mappedWorkFlow = await GetWorkFlowAsync<WorkFlowIdentifierDto>(id);
            if (mappedWorkFlow == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForWorkFlow<WorkFlowIdentifierDto>(mappedWorkFlow));
        }

        /// <summary>
        /// Get a single workFlow using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the workFlow</param>
        /// <returns>An ActionResult of type WorkFlowDetailDto</returns>
        [HttpGet("workflow/{id}", Name = "GetWorkFlowByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<WorkFlowDetailDto>> GetWorkFlowByDetail(Guid id)
        {
            var mappedWorkFlow = await GetWorkFlowAsync<WorkFlowDetailDto>(id);
            if (mappedWorkFlow == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForWorkFlow<WorkFlowDetailDto>(CustomWorkFlowMap(mappedWorkFlow)));
        }

        /// <summary>
        /// Download a dataset for corresponding work flow
        /// </summary>
        /// <param name="id">The unique id of the work flow you would like to download the dataset for</param>
        /// <param name="analyserDatasetResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult</returns>
        [HttpGet("workflow/{id}", Name = "DownloadDataset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.dataset.v1+json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "Analyst")]
        public async Task<ActionResult> DownloadDataset(Guid id,
            [FromQuery] AnalyserDatasetResourceParameters analyserDatasetResourceParameters)
        {
            var workflowFromRepo = await _workFlowRepository.GetAsync(f => f.WorkFlowGuid == id);
            if (workflowFromRepo == null)
            {
                return NotFound();  
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = _userRepository.Get(u => u.UserName == userName);

            if (!userFromRepo.AllowDatasetDownload)
            {
                ModelState.AddModelError("Message", "You do not have permissions to download a dataset");
                return BadRequest(ModelState);
            }

            var model = id == new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986") 
                ? _artefactService.CreateSpontaneousDatasetForDownload() 
                : _artefactService.CreateActiveDatasetForDownload(new long[] { }, analyserDatasetResourceParameters?.CohortGroupId ?? 0);

            return PhysicalFile(model.FullPath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        /// <summary>
        /// Get single workFlow from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetWorkFlowAsync<T>(Guid id) where T : class
        {
            var workFlowFromRepo = await _workFlowRepository.GetAsync(f => f.WorkFlowGuid == id);

            if (workFlowFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedWorkFlow = _mapper.Map<T>(workFlowFromRepo);

                return mappedWorkFlow;
            }

            return null;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private WorkFlowDetailDto CustomWorkFlowMap(WorkFlowDetailDto dto)
        {
            var workFlowFromRepo = _workFlowRepository.Get(f => f.Id == dto.Id);
            if (workFlowFromRepo == null)
            {
                return dto;
            }

            dto.NewReportInstanceCount = GetNewReportInstancesCount(dto.WorkFlowGuid);
            dto.NewFeedbackInstanceCount = GetNewFeedbackCount(dto.WorkFlowGuid);
            dto.ActivityItems = workFlowFromRepo.Activities.Select(activity =>
                new ActivitySummaryDto()
                {
                    QualifiedName = activity.QualifiedName,
                    ReportInstanceCount = GetReportInstancesCount(dto.WorkFlowGuid, activity.QualifiedName)
                }).ToArray();

            return dto;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private WorkFlowIdentifierDto CreateLinksForWorkFlow<T>(T dto)
        {
            WorkFlowIdentifierDto identifier = (WorkFlowIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "WorkFlow", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        /// Get a total number of report instances as per the designated activity
        /// </summary>
        /// <param name="workFlowGuid">The uniwue identifier of the work flow that report instances are associated to</param>
        /// <param name="qualifiedName">The activity to get the report instance count for</param>
        /// <returns></returns>
        private int GetReportInstancesCount(Guid workFlowGuid, string qualifiedName)
        {
            // FIlter list
            var predicate = PredicateBuilder.New<ReportInstance>(true);
            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);

            switch (qualifiedName)
            {
                case "Confirm Report Data":
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true && a.CurrentStatus.Description != "DELETED"));
                    break;

                case "Extract E2B":
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true && a.CurrentStatus.Description != "E2BSUBMITTED"));
                    break;

                default:
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == qualifiedName && a.Current == true));
                    break;
            }

            return _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
        }

        /// <summary>
        /// Get a total number of new report instances
        /// </summary>
        /// <param name="workFlowGuid">The uniwue identifier of the work flow that report instances are associated to</param>
        /// <returns></returns>
        private int GetNewReportInstancesCount(Guid workFlowGuid)
        {
            var config = _configRepository.Get(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    var alertCount = Convert.ToInt32(config.ConfigValue);

                    // How many instances within the last alertcount
                    var compDate = DateTime.Now.AddDays(alertCount * -1);

                    // FIlter list
                    var predicate = PredicateBuilder.New<ReportInstance>(true);
                    predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
                    predicate = predicate.And(f => f.Created >= compDate);

                    return _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
                }
            }

            return 0;
        }

        /// <summary>
        /// Get a total number of new report instances with feedback for clinician
        /// </summary>
        /// <param name="workFlowGuid">The uniwue identifier of the work flow that report instances are associated to</param>
        /// <returns></returns>
        private int GetNewFeedbackCount(Guid workFlowGuid)
        {
            var config = _configRepository.Get(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            if (config != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    var alertCount = Convert.ToInt32(config.ConfigValue);

                    // How many instances within the last alertcount
                    var compDate = DateTime.Now.AddDays(alertCount * -1);

                    // FIlter list
                    var predicate = PredicateBuilder.New<ReportInstance>(true);
                    predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
                    predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == "Set MedDRA and Causality" && a.CurrentStatus.Description == "CAUSALITYSET" && a.Created >= compDate));

                    return _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
                }
            }

            return 0;
        }
    }
}
