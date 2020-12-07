using AutoMapper;
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
    [Route("api")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<SiteContactDetail> _contactRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;

        public ContactsController(IPropertyMappingService propertyMappingService,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<SiteContactDetail> contactRepository,
            IUnitOfWorkInt unitOfWork)
        {
            _propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Get all contact details using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of ContactDetailDto</returns>
        [HttpGet("contactdetails", Name = "GetContactsByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<ContactDetailDto>> GetContactsByDetail(
            [FromQuery] ContactResourceParameters contactResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<ContactDetailDto>
                (contactResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedContactDetailsWithLinks = GetContactDetails<ContactDetailDto>(contactResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<ContactDetailDto>(mappedContactDetailsWithLinks.TotalCount, mappedContactDetailsWithLinks);
            //var wrapperWithLinks = CreateLinksForFacilities(wrapper, labTestResourceParameters,
            //    mappedLabTestsWithLinks.HasNext, mappedLabTestsWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single contact detail using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type ContactIdentifierDto</returns>
        [HttpGet("contactdetails/{id}", Name = "GetContactByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<ContactIdentifierDto>> GetContactByIdentifier(long id)
        {
            var mappedContactDetail = await GetContactDetailAsync<ContactIdentifierDto>(id);
            if (mappedContactDetail == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForContactDetail<ContactIdentifierDto>(mappedContactDetail));
        }

        /// <summary>
        /// Get a single contact detail using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the lab test</param>
        /// <returns>An ActionResult of type ContactDetailDto</returns>
        [HttpGet("contactdetails/{id}", Name = "GetContactByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ContactDetailDto>> GetContactByDetail(long id)
        {
            var mappedContactDetail = await GetContactDetailAsync<ContactDetailDto>(id);
            if (mappedContactDetail == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForContactDetail<ContactDetailDto>(mappedContactDetail));
        }

        /// <summary>
        /// Update an existing contact detail
        /// </summary>
        /// <param name="id">The unique id of the contact detail</param>
        /// <param name="contactForUpdateDto">The contact payload</param>
        /// <returns></returns>
        [HttpPut("contactdetails/{id}", Name = "UpdateContactDetail")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateContactDetail(long id,
            [FromBody] ContactForUpdateDto contactForUpdateDto)
        {
            var contactDetailFromRepo = await _contactRepository.GetAsync(f => f.Id == id);
            if (contactDetailFromRepo == null)
            {
                return NotFound();
            }

            if (contactForUpdateDto == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
            }

            if (Regex.Matches(contactForUpdateDto.OrganisationName, @"[a-zA-Z0-9 ]").Count < contactForUpdateDto.OrganisationName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            if (Regex.Matches(contactForUpdateDto.ContactFirstName, @"[a-zA-Z ]").Count < contactForUpdateDto.ContactFirstName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (Regex.Matches(contactForUpdateDto.ContactLastName, @"[a-zA-Z ]").Count < contactForUpdateDto.ContactLastName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (Regex.Matches(contactForUpdateDto.StreetAddress, @"[a-zA-Z0-9 ']").Count < contactForUpdateDto.StreetAddress.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space, comma)");
            }

            if (!String.IsNullOrEmpty(contactForUpdateDto.City))
            {
                if (Regex.Matches(contactForUpdateDto.City, @"[a-zA-Z ]").Count < contactForUpdateDto.City.Length)
                {
                    ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
                }
            }

            if (!String.IsNullOrEmpty(contactForUpdateDto.State))
            {
                if (Regex.Matches(contactForUpdateDto.State, @"[a-zA-Z ]").Count < contactForUpdateDto.State.Length)
                {
                    ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
                }
            }

            if (!String.IsNullOrEmpty(contactForUpdateDto.PostCode))
            {
                if (Regex.Matches(contactForUpdateDto.PostCode, @"[a-zA-Z0-9]").Count < contactForUpdateDto.PostCode.Length)
                {
                    ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9)");
                }
            }

            if (!String.IsNullOrEmpty(contactForUpdateDto.ContactNumber))
            {
                if (Regex.Matches(contactForUpdateDto.ContactNumber, @"[-0-9]").Count < contactForUpdateDto.ContactNumber.Length)
                {
                    ModelState.AddModelError("Message", "Value contains invalid characters (Enter 0-9, hyphen)");
                }
            }

            if (!String.IsNullOrEmpty(contactForUpdateDto.ContactEmail))
            {
                if (Regex.Matches(contactForUpdateDto.ContactEmail, @"[-a-zA-Z@._]").Count < contactForUpdateDto.ContactEmail.Length)
                {
                    ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, hyphen, @, period, underscore)");
                }
            }

            if (!String.IsNullOrEmpty(contactForUpdateDto.CountryCode))
            {
                if (Regex.Matches(contactForUpdateDto.CountryCode, @"[0-9]").Count < contactForUpdateDto.CountryCode.Length)
                {
                    ModelState.AddModelError("Message", "Value contains invalid characters (Enter 0-9)");
                }
            }

            if (ModelState.IsValid)
            {
                contactDetailFromRepo.OrganisationName = contactForUpdateDto.OrganisationName;
                contactDetailFromRepo.ContactFirstName = contactForUpdateDto.ContactFirstName;
                contactDetailFromRepo.ContactSurname = contactForUpdateDto.ContactLastName;
                contactDetailFromRepo.StreetAddress = contactForUpdateDto.StreetAddress;
                contactDetailFromRepo.City = contactForUpdateDto.City;
                contactDetailFromRepo.State = contactForUpdateDto.State;
                contactDetailFromRepo.CountryCode = contactForUpdateDto.CountryCode;
                contactDetailFromRepo.PostCode = contactForUpdateDto.PostCode;
                contactDetailFromRepo.ContactNumber = contactForUpdateDto.ContactNumber;
                contactDetailFromRepo.ContactEmail = contactForUpdateDto.ContactEmail;

                _contactRepository.Update(contactDetailFromRepo);
                _unitOfWork.Complete();

                return Ok();
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Get contact details from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="contactResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
        private PagedCollection<T> GetContactDetails<T>(ContactResourceParameters contactResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = contactResourceParameters.PageNumber,
                PageSize = contactResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<SiteContactDetail>(contactResourceParameters.OrderBy, "asc");

            var pagedContactsFromRepo = _contactRepository.List(pagingInfo, null, orderby, "");
            if (pagedContactsFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedContacts = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedContactsFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedContactsFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedContacts.TotalCount,
                    pageSize = mappedContacts.PageSize,
                    currentPage = mappedContacts.CurrentPage,
                    totalPages = mappedContacts.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                mappedContacts.ForEach(dto => CreateLinksForContactDetail(dto));

                return mappedContacts;
            }

            return null;
        }

        /// <summary>
        /// Get single contact detail from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetContactDetailAsync<T>(long id) where T : class
        {
            var contactDetailFromRepo = await _contactRepository.GetAsync(f => f.Id == id);

            if (contactDetailFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedContactDetail = _mapper.Map<T>(contactDetailFromRepo);

                return mappedContactDetail;
            }

            return null;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private ContactIdentifierDto CreateLinksForContactDetail<T>(T dto)
        {
            ContactIdentifierDto identifier = (ContactIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "Contact", identifier.Id), "self", "GET"));

            return identifier;
        }
    }
}
