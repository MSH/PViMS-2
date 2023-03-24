using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Application.Queries.DashboardAggregate;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class DashboardsController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardsController> _logger;

        public DashboardsController(IMediator mediator,
            ITypeHelperService typeHelperService,
            ILogger<DashboardsController> logger)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all dashboards using a valid media type 
        /// </summary>
        /// <param name="dashboardResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of DashboardDetailDto</returns>
        [HttpGet("dashboards", Name = "GetDashboardsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<DashboardDetailDto>>> GetDashboardsByDetail(
            [FromQuery] IdResourceParameters dashboardResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<DashboardDetailDto>
                (dashboardResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new DashboardsDetailQuery(
                dashboardResourceParameters.OrderBy,
                dashboardResourceParameters.PageNumber,
                dashboardResourceParameters.PageSize);

            _logger.LogInformation(
                "----- Sending query: DashboardsDetailQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = dashboardResourceParameters.PageSize,
                currentPage = dashboardResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
        }

        /// <summary>
        /// Generate a set of dashboard widgets using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the Facility</param>
        /// <returns>An ActionResult of type ChartDto</returns>
        [HttpGet("dashboards/{id}", Name = "GenerateDashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<List<ChartDto>>> GenerateDashboard(int id)
        {
            var query = new GenerateDashboardQuery(id);

            _logger.LogInformation(
                "----- Sending query: GenerateDashboardQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }
    }
}
