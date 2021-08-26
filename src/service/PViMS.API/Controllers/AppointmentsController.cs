using AutoMapper;
using LinqKit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Application.Commands.AppointmentAggregate;
using PVIMS.API.Application.Queries.AppointmentAggregate;
using PVIMS.Core.Aggregates.UserAggregate;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Keyless;
using PVIMS.Core.Paging;
using PVIMS.Core.Repositories;
using PVIMS.Core.Services;
using PVIMS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class AppointmentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Patient> _patientRepository;
        private readonly IRepositoryInt<Appointment> _appointmentRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IRepositoryInt<CustomAttributeConfiguration> _customAttributeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly IReportService _reportService;
        private readonly PVIMSDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IMediator mediator,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<Patient> patientRepository,
            IRepositoryInt<Appointment> appointmentRepository,
            IRepositoryInt<Facility> facilityRepository,
            IRepositoryInt<CustomAttributeConfiguration> customAttributeRepository,
            IUnitOfWorkInt unitOfWork,
            IReportService reportService,
            PVIMSDbContext dbContext,
            ILogger<AppointmentsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _customAttributeRepository = customAttributeRepository ?? throw new ArgumentNullException(nameof(customAttributeRepository));
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all appointments using a valid media type 
        /// </summary>
        /// <param name="appointmentResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of AppointmentSearchDto</returns>
        [HttpGet("appointments", Name = "GetAppointmentsForSearch")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.search.v1+json", "application/vnd.pvims.search.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.search.v1+json", "application/vnd.pvims.search.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<AppointmentSearchDto>> GetAppointmentsForSearch(
            [FromQuery] AppointmentResourceParameters appointmentResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<AppointmentSearchDto>
                (appointmentResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            if (!String.IsNullOrWhiteSpace(appointmentResourceParameters.FirstName))
            {
                if (Regex.Matches(appointmentResourceParameters.FirstName, @"[-a-zA-Z ']").Count < appointmentResourceParameters.FirstName.Length)
                {
                    ModelState.AddModelError("Message", "First name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
                }
            }

            if (!String.IsNullOrWhiteSpace(appointmentResourceParameters.LastName))
            {
                if (Regex.Matches(appointmentResourceParameters.LastName, @"[-a-zA-Z ']").Count < appointmentResourceParameters.LastName.Length)
                {
                    ModelState.AddModelError("Message", "Last name contains invalid characters (Enter A-Z, a-z, space, apostrophe)");
                }
            }

            if (!String.IsNullOrWhiteSpace(appointmentResourceParameters.CustomAttributeValue))
            {
                if (Regex.Matches(appointmentResourceParameters.CustomAttributeValue, @"[-a-zA-Z ']").Count < appointmentResourceParameters.CustomAttributeValue.Length)
                {
                    ModelState.AddModelError("Message", "Custom attribute value contains invalid characters (Enter A-Z, a-z, 0-9, space, apostrophe)");
                }
            }

            var mappedAppointmentsWithLinks = GetAppointments<AppointmentSearchDto>(appointmentResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<AppointmentSearchDto>(mappedAppointmentsWithLinks.TotalCount, mappedAppointmentsWithLinks);
            var wrapperWithLinks = CreateLinksForAppointments(wrapper, appointmentResourceParameters,
                mappedAppointmentsWithLinks.HasNext, mappedAppointmentsWithLinks.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Get a single appointment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Appointment</param>
        /// <returns>An ActionResult of type AppointmentIdentifierDto</returns>
        [HttpGet("patients/{patientId}/appointments/{id}", Name = "GetAppointmentByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<AppointmentIdentifierDto>> GetAppointmentByIdentifier(int patientId, int id)
        {
            var query = new AppointmentIdentifierQuery(patientId, id);

            _logger.LogInformation(
                $"----- Sending query: AppointmentIdentifierQuery - {patientId} - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a single appointment using it's unique id and valid media type 
        /// </summary>
        /// <param name="patientId">The unique identifier for the Patient</param>
        /// <param name="id">The unique identifier for the Appointment</param>
        /// <returns>An ActionResult of type AppointmentDetailDto</returns>
        [HttpGet("patients/{patientId}/appointments/{id}", Name = "GetAppointmentByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<AppointmentDetailDto>> GetAppointmentByDetail(int patientId, int id)
        {
            var query = new AppointmentDetailQuery(patientId, id);

            _logger.LogInformation(
                $"----- Sending query: AppointmentDetailQuery - {patientId} - {id}");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
        }

        /// <summary>
        /// Get a list of outstanding visits
        /// </summary>
        /// <param name="outstandingVisitResourceParameters">
        /// Specify paging and filtering information (including requested page number and page size)
        /// </param>
        /// <returns>An ActionResult of type OutstandingVisitReportDto</returns>
        [HttpGet("appointments", Name = "GetOutstandingVisitReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.outstandingvisitreport.v1+json", "application/vnd.pvims.outstandingvisitreport.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.outstandingvisitreport.v1+json", "application/vnd.pvims.outstandingvisitreport.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>> GetOutstandingVisitReport(
                        [FromQuery] OutstandingVisitResourceParameters outstandingVisitResourceParameters)
        {
            if (outstandingVisitResourceParameters == null)
            {
                ModelState.AddModelError("Message", "Unable to locate filter paramters payload");
                return BadRequest(ModelState);
            }

            var mappedResults = GetOutstandingVisitResults<OutstandingVisitReportDto>(outstandingVisitResourceParameters);

            // Add custom mappings to appointments
            mappedResults.ForEach(dto => CustomAppointmentMap(dto));

            var wrapper = new LinkedCollectionResourceWrapperDto<OutstandingVisitReportDto>(mappedResults.TotalCount, mappedResults);
            var wrapperWithLinks = CreateLinksForOutstandingVisitReport(wrapper, outstandingVisitResourceParameters,
                mappedResults.HasNext, mappedResults.HasPrevious);

            return Ok(wrapperWithLinks);
        }

        /// <summary>
        /// Create a new appointment for a patient
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="appointmentForCreation">The appointment payload</param>
        /// <returns></returns>
        [HttpPost("patients/{patientId}/appointments", Name = "CreateAppointment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateAppointment(int patientId, 
            [FromBody] AppointmentForCreationDto appointmentForCreation)
        {
            if (appointmentForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new appointment");
                return BadRequest(ModelState);
            }

            var command = new AddAppointmentCommand(patientId, appointmentForCreation.AppointmentDate, appointmentForCreation.Reason);

            _logger.LogInformation(
                $"----- Sending command: AddAppointmentCommand - {command.PatientId} - {command.AppointmentDate}");

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetAppointmentByIdentifier",
                new
                {
                    patientId,
                    id = commandResult.Id
                }, commandResult);
        }

        /// <summary>
        /// Update an existing appointment
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the appointment</param>
        /// <param name="appointmentForUpdate">The appointment payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/appointments/{id}", Name = "UpdateAppointment")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateAppointment(int patientId, int id,
            [FromBody] AppointmentForUpdateDto appointmentForUpdate)
        {
            if (appointmentForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ChangeAppointmentDetailsCommand(patientId, id, appointmentForUpdate.AppointmentDate, appointmentForUpdate.Reason, appointmentForUpdate.Cancelled == Models.ValueTypes.YesNoValueType.Yes, appointmentForUpdate.CancellationReason);

            _logger.LogInformation(
                $"----- Sending command: ChangeAppointmentDetailsCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Update an existing appointment as did not arrive
        /// </summary>
        /// <param name="patientId">The unique id of the patient</param>
        /// <param name="id">The unique id of the appointment</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/appointments/{id}/dna", Name = "UpdateAppointmentAsDNA")]
        [Consumes("application/json")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateAppointmentAsDNA(long patientId, long id)
        {
            var appointmentFromRepo = await _appointmentRepository.GetAsync(f => f.PatientId == patientId && f.Id == id);
            if (appointmentFromRepo == null)
            {
                return NotFound();
            }

            var patientFromRepo = await _patientRepository.GetAsync(f => f.Id == patientId);
            if (patientFromRepo == null)
            {
                ModelState.AddModelError("Message", "Unable to locate patient");
            }

            if (appointmentFromRepo.AppointmentDate > DateTime.Today)
            {
                ModelState.AddModelError("Message", "Appointment date has not passed");
            }

            if (appointmentFromRepo.Dna)
            {
                ModelState.AddModelError("Message", "Appointment already marked as DNA");
            }

            if (ModelState.IsValid)
            {
                appointmentFromRepo.MarkAsDNA();

                _appointmentRepository.Update(appointmentFromRepo);
                await _unitOfWork.CompleteAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Archive an existing appointment
        /// </summary>
        /// <param name="patientId">The unique identifier of the patient</param>
        /// <param name="id">The unique id of the appointment</param>
        /// <param name="appointmentForDelete">The deletion payload</param>
        /// <returns></returns>
        [HttpPut("patients/{patientId}/appointments/{id}/archive", Name = "ArchiveAppointment")]
        public async Task<IActionResult> ArchiveAppointment(int patientId, int id,
            [FromBody] ArchiveDto appointmentForDelete)
        {
            if (appointmentForDelete == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var command = new ArchiveAppointmentCommand(patientId, id, appointmentForDelete.Reason);

            _logger.LogInformation(
                $"----- Sending command: ArchiveAppointmentCommand - {id}");

            var commandResult = await _mediator.Send(command);

            if (!commandResult)
            {
                return BadRequest("Command not created");
            }

            return Ok();
        }

        /// <summary>
        /// Get appointments from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="appointmentResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetAppointments<T>(AppointmentResourceParameters appointmentResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = appointmentResourceParameters.PageNumber,
                PageSize = appointmentResourceParameters.PageSize
            };

            var facility = !String.IsNullOrWhiteSpace(appointmentResourceParameters.FacilityName) ? _facilityRepository.Get(f => f.FacilityName == appointmentResourceParameters.FacilityName) : null;
            var customAttribute = _customAttributeRepository.Get(ca => ca.Id == appointmentResourceParameters.CustomAttributeId);
            var path = customAttribute?.CustomAttributeType == PVIMS.Core.CustomAttributes.CustomAttributeType.Selection ? "CustomSelectionAttribute" : "CustomStringAttribute";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@FacilityId", facility != null ? facility.Id : 0));
            parameters.Add(new SqlParameter("@PatientId", appointmentResourceParameters.PatientId.ToString()));
            parameters.Add(new SqlParameter("@CriteriaId", appointmentResourceParameters.CriteriaId));
            parameters.Add(new SqlParameter("@FirstName", !String.IsNullOrWhiteSpace(appointmentResourceParameters.FirstName) ? (Object)appointmentResourceParameters.FirstName : DBNull.Value));
            parameters.Add(new SqlParameter("@LastName", !String.IsNullOrWhiteSpace(appointmentResourceParameters.LastName) ? (Object)appointmentResourceParameters.LastName : DBNull.Value));
            parameters.Add(new SqlParameter("@SearchFrom", appointmentResourceParameters.SearchFrom > DateTime.MinValue ? (Object)appointmentResourceParameters.SearchFrom : DBNull.Value));
            parameters.Add(new SqlParameter("@SearchTo", appointmentResourceParameters.SearchTo < DateTime.MaxValue ? (Object)appointmentResourceParameters.SearchTo : DBNull.Value));
            parameters.Add(new SqlParameter("@CustomAttributeKey", !String.IsNullOrWhiteSpace(appointmentResourceParameters.CustomAttributeValue) ? (Object)customAttribute?.AttributeKey : DBNull.Value));
            parameters.Add(new SqlParameter("@CustomPath", !String.IsNullOrWhiteSpace(appointmentResourceParameters.CustomAttributeValue) ? (Object)path : DBNull.Value));
            parameters.Add(new SqlParameter("@CustomValue", !String.IsNullOrWhiteSpace(appointmentResourceParameters.CustomAttributeValue) ? (Object)appointmentResourceParameters.CustomAttributeValue : DBNull.Value));

            var resultsFromService = PagedCollection<AppointmentList>.Create(_context.AppointmentLists
                .FromSqlRaw("Exec spSearchAppointments @FacilityId, @PatientId, @CriteriaId, @FirstName, @LastName, @SearchFrom, @SearchTo, @CustomAttributeKey, @CustomPath, @CustomValue",
                        parameters.ToArray()), pagingInfo.PageNumber, pagingInfo.PageSize);

            if (resultsFromService != null)
            {
                // Map EF entity to Dto
                var mappedAppointments = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(resultsFromService),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    resultsFromService.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedAppointments.TotalCount,
                    pageSize = mappedAppointments.PageSize,
                    currentPage = mappedAppointments.CurrentPage,
                    totalPages = mappedAppointments.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedAppointments.ForEach(dto => CreateLinksForAppointment(appointmentResourceParameters.PatientId, dto));

                return mappedAppointments;
            }

            return null;
        }

        /// <summary>
        /// Get patients from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier, detail or expanded Dto</typeparam>
        /// <param name="outstandingVisitResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetOutstandingVisitResults<T>(OutstandingVisitResourceParameters outstandingVisitResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = outstandingVisitResourceParameters.PageNumber,
                PageSize = outstandingVisitResourceParameters.PageSize
            };

            var resultsFromService = PagedCollection<OutstandingVisitList>.Create(_reportService.GetOutstandingVisitItems(
                outstandingVisitResourceParameters.SearchFrom, 
                outstandingVisitResourceParameters.SearchTo, 
                outstandingVisitResourceParameters.FacilityId), pagingInfo.PageNumber, pagingInfo.PageSize);

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
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="appointmentResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForAppointments(
            LinkedResourceBaseDto wrapper,
            AppointmentResourceParameters appointmentResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.Current, appointmentResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.NextPage, appointmentResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateAppointmentsResourceUri(ResourceUriType.PreviousPage, appointmentResourceParameters),
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
        private AppointmentIdentifierDto CreateLinksForAppointment<T>(long patientId, T dto)
        {
            AppointmentIdentifierDto identifier = (AppointmentIdentifierDto)(object)dto;

            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateUpdateHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "update", "PATCH"));
            //identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateRemoveHouseholdMemberForHouseholdResourceUri(_urlHelper, organisationunitId, householdId.ToGuid(), identifier.HouseholdMemberGuid), "marknotcurrent", "DELETE"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private OutstandingVisitReportDto CustomAppointmentMap(OutstandingVisitReportDto dto)
        {
            var patient = _patientRepository.Get(p => p.Id == dto.PatientId);
            if (patient == null)
            {
                return dto;
            }

            dto.Facility = patient.CurrentFacility?.Facility.DisplayName;

            return dto;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="outstandingVisitResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForOutstandingVisitReport(
            LinkedResourceBaseDto wrapper,
            OutstandingVisitResourceParameters outstandingVisitResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateOutstandingVisitReportResourceUri(ResourceUriType.Current, outstandingVisitResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateOutstandingVisitReportResourceUri(ResourceUriType.NextPage, outstandingVisitResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateOutstandingVisitReportResourceUri(ResourceUriType.PreviousPage, outstandingVisitResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }
    }
}
