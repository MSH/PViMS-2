using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Queries.FacilityAggregate;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Models;
using System;
using System.Threading.Tasks;
using PVIMS.API.Application.Queries.DashboardAggregate;
using System.Collections.Generic;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class DashboardsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DashboardsController> _logger;

        public DashboardsController(IMediator mediator, 
            ILogger<DashboardsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get a single facility using it's unique id and valid media type 
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
