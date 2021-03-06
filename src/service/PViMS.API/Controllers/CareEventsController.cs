﻿using AutoMapper;
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
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VPS.Common.Collections;
using VPS.Common.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/careevents")]
    [Authorize]
    public class CareEventsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<CareEvent> _careEventRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public CareEventsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<CareEvent> careEventRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _careEventRepository = careEventRepository ?? throw new ArgumentNullException(nameof(careEventRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all care events using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of CareEventIdentifierDto</returns>
        [HttpGet(Name = "GetCareEventsByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<CareEventIdentifierDto>> GetCareEventsByIdentifier(
            [FromQuery] IdResourceParameters caseEventResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<CareEventIdentifierDto>
                (caseEventResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedCareEventsWithLinks = GetCareEvents<CareEventIdentifierDto>(caseEventResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<CareEventIdentifierDto>(mappedCareEventsWithLinks.TotalCount, mappedCareEventsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, careEventResourceParameters,
            //    mappedCareEventsWithLinks.HasNext, mappedCareEventsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single care event using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the care event</param>
        /// <returns>An ActionResult of type CareEventIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetCareEventByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<CareEventIdentifierDto>> GetCareEventByIdentifier(long id)
        {
            var mappedCareEvent = await GetCareEventAsync<CareEventIdentifierDto>(id);
            if (mappedCareEvent == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForCareEvent<CareEventIdentifierDto>(mappedCareEvent));
        }

        /// <summary>
        /// Create a new care event
        /// </summary>
        /// <param name="careEventForUpdate">The care event payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateCareEvent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateCareEvent(
            [FromBody] CareEventForUpdateDto careEventForUpdate)
        {
            if (careEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(careEventForUpdate.CareEventName, @"[a-zA-Z ']").Count < careEventForUpdate.CareEventName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<CareEvent>().Queryable().
                Where(l => l.Description == careEventForUpdate.CareEventName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            long id = 0;

            if (ModelState.IsValid)
            {
                var newCareEvent = new CareEvent()
                {
                    Description = careEventForUpdate.CareEventName
                };

                _careEventRepository.Save(newCareEvent);
                id = newCareEvent.Id;
            }

            var mappedCareEvent = await GetCareEventAsync<CareEventIdentifierDto>(id);
            if (mappedCareEvent == null)
            {
                return StatusCode(500, "Unable to locate newly added item");
            }

            return CreatedAtRoute("GetCareEventByIdentifier",
                new
                {
                    id = mappedCareEvent.Id
                }, CreateLinksForCareEvent<CareEventIdentifierDto>(mappedCareEvent));
        }

        /// <summary>
        /// Update an existing care event
        /// </summary>
        /// <param name="id">The unique id of the care event</param>
        /// <param name="careEventForUpdate">The care event payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateCareEvent")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCareEvent(long id,
            [FromBody] CareEventForUpdateDto careEventForUpdate)
        {
            if (careEventForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(careEventForUpdate.CareEventName, @"[a-zA-Z ']").Count < careEventForUpdate.CareEventName.Length)
            {
                ModelState.AddModelError("Message", "Description contains invalid characters (Enter A-Z, a-z)");
                return BadRequest(ModelState);
            }

            if (_unitOfWork.Repository<CareEvent>().Queryable().
                Where(l => l.Description == careEventForUpdate.CareEventName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            var careEventFromRepo = await _careEventRepository.GetAsync(f => f.Id == id);
            if (careEventFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                careEventFromRepo.Description = careEventForUpdate.CareEventName;

                _careEventRepository.Update(careEventFromRepo);
                _unitOfWork.Complete();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing care event
        /// </summary>
        /// <param name="id">The unique id of the care event</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteCareEvent")]
        public async Task<IActionResult> DeleteCareEvent(long id)
        {
            var careEventFromRepo = await _careEventRepository.GetAsync(f => f.Id == id);
            if (careEventFromRepo == null)
            {
                return NotFound();
            }

            if (careEventFromRepo.WorkPlanCareEvents.Count > 0)
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                _careEventRepository.Delete(careEventFromRepo);
                _unitOfWork.Complete();
            }

            return NoContent();
        }

        /// <summary>
        /// Get care events from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="careEventResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetCareEvents<T>(IdResourceParameters careEventResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = careEventResourceParameters.PageNumber,
                PageSize = careEventResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<CareEvent>(careEventResourceParameters.OrderBy, "asc");

            var pagedCareEventsFromRepo = _careEventRepository.List(pagingInfo, null, orderby, "");
            if (pagedCareEventsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCareEvents = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedCareEventsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedCareEventsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedCareEvents.TotalCount,
                    pageSize = mappedCareEvents.PageSize,
                    currentPage = mappedCareEvents.CurrentPage,
                    totalPages = mappedCareEvents.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedCareEvents.ForEach(dto => CreateLinksForCareEvent(dto));

                return mappedCareEvents;
            }

            return null;
        }

        /// <summary>
        /// Get single care event from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetCareEventAsync<T>(long id) where T : class
        {
            var careEventFromRepo = await _careEventRepository.GetAsync(f => f.Id == id);

            if (careEventFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedCareEvent = _mapper.Map<T>(careEventFromRepo);

                return mappedCareEvent;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private CareEventIdentifierDto CreateLinksForCareEvent<T>(T dto)
        {
            CareEventIdentifierDto identifier = (CareEventIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "CareEvent", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
