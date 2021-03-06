﻿using AutoMapper;
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
using System.Threading.Tasks;
using VPS.Common.Collections;
using VPS.Common.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class OutcomesController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<Outcome> _outcomeRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public OutcomesController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<Outcome> outcomeRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _outcomeRepository = outcomeRepository ?? throw new ArgumentNullException(nameof(outcomeRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all outcomes using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of OutcomeIdentifierDto</returns>
        [HttpGet("outcomes", Name = "GetOutcomesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<OutcomeIdentifierDto>> GetOutcomesByIdentifier(
            [FromQuery] IdResourceParameters baseResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<OutcomeIdentifierDto>
                (baseResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedOutcomesWithLinks = GetOutcomes<OutcomeIdentifierDto>(baseResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<OutcomeIdentifierDto>(mappedOutcomesWithLinks.TotalCount, mappedOutcomesWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, outcomeResourceParameters,
            //    mappedOutcomesWithLinks.HasNext, mappedOutcomesWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single outcome using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the outcome</param>
        /// <returns>An ActionResult of type OutcomeIdentifierDto</returns>
        [HttpGet("outcomes/{id}", Name = "GetOutcomeByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType(HeaderNames.Accept,
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<OutcomeIdentifierDto>> GetOutcomeByIdentifier(long id)
        {
            var mappedOutcome = await GetOutcomeAsync<OutcomeIdentifierDto>(id);
            if (mappedOutcome == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForOutcome<OutcomeIdentifierDto>(mappedOutcome));
        }

        /// <summary>
        /// Get outcomes from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="baseResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetOutcomes<T>(IdResourceParameters baseResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = baseResourceParameters.PageNumber,
                PageSize = baseResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<Outcome>(baseResourceParameters.OrderBy, "asc");

            var pagedOutcomesFromRepo = _outcomeRepository.List(pagingInfo, null, orderby, "");
            if (pagedOutcomesFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedOutcomes = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedOutcomesFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedOutcomesFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedOutcomes.TotalCount,
                    pageSize = mappedOutcomes.PageSize,
                    currentPage = mappedOutcomes.CurrentPage,
                    totalPages = mappedOutcomes.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedOutcomes.ForEach(dto => CreateLinksForOutcome(dto));

                return mappedOutcomes;
            }

            return null;
        }

        /// <summary>
        /// Get single outcome from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetOutcomeAsync<T>(long id) where T : class
        {
            var outcomeFromRepo = await _outcomeRepository.GetAsync(f => f.Id == id);

            if (outcomeFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedOutcome = _mapper.Map<T>(outcomeFromRepo);

                return mappedOutcome;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private OutcomeIdentifierDto CreateLinksForOutcome<T>(T dto)
        {
            OutcomeIdentifierDto identifier = (OutcomeIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Outcome", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
