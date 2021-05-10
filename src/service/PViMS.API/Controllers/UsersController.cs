using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Parameters;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;
using PViMS.Infrastructure.Identity.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class UsersController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<UserFacility> _userFacilityRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<UserFacility> userFacilityRepository,
            IRepositoryInt<Facility> facilityRepository,
            IUnitOfWorkInt unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userFacilityRepository = userFacilityRepository ?? throw new ArgumentNullException(nameof(userFacilityRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Get all users using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of UserIdentifierDto</returns>
        [HttpGet(Name = "GetUsersByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public ActionResult<LinkedCollectionResourceWrapperDto<UserIdentifierDto>> GetUsersByIdentifier(
            [FromQuery] UserResourceParameters userResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<UserIdentifierDto>
                (userResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var mappedUsersWithLinks = GetUsers<UserIdentifierDto>(userResourceParameters);

            var wrapper = new LinkedCollectionResourceWrapperDto<UserIdentifierDto>(mappedUsersWithLinks.TotalCount, mappedUsersWithLinks);
            var wrapperWithLinks = CreateLinksForUsers(wrapper, userResourceParameters,
                mappedUsersWithLinks.HasNext, mappedUsersWithLinks.HasPrevious);

            return Ok(wrapper);
        }

        /// <summary>
        /// Get a single user using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the user</param>
        /// <returns>An ActionResult of type UserIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetUserByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<UserIdentifierDto>> GetUserByIdentifier(long id)
        {
            var mappedUser = await GetUserAsync<UserIdentifierDto>(id);
            if (mappedUser == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForUser<UserIdentifierDto>(mappedUser));
        }

        /// <summary>
        /// Get a single user using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the user</param>
        /// <param name="userRoleResourceParameter">The unique identifier for the user</param>
        /// <returns>An ActionResult of type UserIdentifierDto</returns>
        [HttpGet("{id}/roles", Name = "GetUserRolesByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<UserRoleDto[]>> GetUserRolesByIdentifier(int id,
            [FromQuery] IdResourceParameters userRoleResourceParameter)
        {
            if (!_typeHelperService.TypeHasProperties<UserRoleDto>
                (userRoleResourceParameter.OrderBy))
            {
                return BadRequest();
            }

            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            //return Ok(GetUserRoles(id));
            return Ok();
        }

        /// <summary>
        /// Get a single user using it's unique id and valid media type 
        /// </summary>
        /// <param name="id">The unique identifier for the user</param>
        /// <returns>An ActionResult of type UserDetailDto</returns>
        [HttpGet("{id}", Name = "GetUserByDetail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.detail.v1+json", "application/vnd.pvims.detail.v1+xml")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<UserDetailDto>> GetUserByDetail(long id)
        {
            var mappedUser = await GetUserAsync<UserDetailDto>(id);
            if (mappedUser == null)
            {
                return NotFound();
            }

            return Ok(CreateLinksForUser<UserDetailDto>(CustomUserMap(mappedUser)));
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userForCreation">The user payload</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateUser")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateUser(
            [FromBody] UserForCreationDto userForCreation)
        {
            if (userForCreation == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            if (Regex.Matches(userForCreation.UserName, @"[a-zA-Z0-9 ]").Count < userForCreation.UserName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            if (Regex.Matches(userForCreation.FirstName, @"[a-zA-Z ]").Count < userForCreation.FirstName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (Regex.Matches(userForCreation.LastName, @"[a-zA-Z ]").Count < userForCreation.LastName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (userForCreation.Facilities == null)
            {
                ModelState.AddModelError("Message", "At least one facility must be selected");
            }

            if (userForCreation.Roles == null)
            {
                ModelState.AddModelError("Message", "At least one role must be selected");
            }

            if (_unitOfWork.Repository<User>().Queryable().
                Where(l => l.UserName == userForCreation.UserName)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
            }

            if (ModelState.IsValid)
            {
                var newApplicationUser = new ApplicationUser() { 
                    FirstName = userForCreation.FirstName, 
                    LastName = userForCreation.LastName, 
                    UserName = userForCreation.UserName, 
                    Email = userForCreation.Email, 
                };
                IdentityResult result = await _userManager.CreateAsync(newApplicationUser, userForCreation.Password);
                if (result.Succeeded)
                {
                    IdentityResult roleResult = await _userManager.AddToRolesAsync(newApplicationUser, userForCreation.Roles);

                    if (roleResult.Succeeded)
                    {
                        var userFromRepo = await _userRepository.GetAsync(f => f.Id == 1);
                        if (userFromRepo == null)
                        {
                            return StatusCode(500, "Unable to locate newly added item");
                        }

                        // ensure facilities are linked to user
                        var facilityNames = userForCreation.Facilities.ToArray();

                        foreach (string facilityName in facilityNames)
                        {
                            var uf = new UserFacility() { 
                                Facility = _facilityRepository.Get(f => f.FacilityName == facilityName), 
                                User = userFromRepo
                            };
                            userFromRepo.Facilities.Add(uf);
                        }
                        _userRepository.Update(userFromRepo);
                        await _unitOfWork.CompleteAsync();

                        var mappedUser = _mapper.Map<UserIdentifierDto>(userFromRepo);

                        return CreatedAtRoute("GetUserByIdentifier",
                            new
                            {
                                id = mappedUser.Id
                            }, CreateLinksForUser<UserIdentifierDto>(mappedUser));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("Message", $"{error.Description} ({error.Code})");
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Message", $"{error.Description} ({error.Code})");
                    }
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="userForUpdate">The user payload</param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "UpdateUser")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateUser(long id,
            [FromBody] UserForUpdateDto userForUpdate)
        {
            if (userForUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            if (Regex.Matches(userForUpdate.UserName, @"[a-zA-Z0-9 ]").Count < userForUpdate.UserName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, 0-9, space)");
            }

            if (Regex.Matches(userForUpdate.FirstName, @"[a-zA-Z ]").Count < userForUpdate.FirstName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (Regex.Matches(userForUpdate.LastName, @"[a-zA-Z ]").Count < userForUpdate.LastName.Length)
            {
                ModelState.AddModelError("Message", "Value contains invalid characters (Enter A-Z, a-z, space)");
            }

            if (userForUpdate.Facilities == null)
            {
                ModelState.AddModelError("Message", "At least one facility must be selected");
            }

            if (userForUpdate.Roles == null)
            {
                ModelState.AddModelError("Message", "At least one role must be selected");
            }

            if (_unitOfWork.Repository<User>().Queryable().
                Where(l => l.UserName == userForUpdate.UserName && l.Id != id)
                .Count() > 0)
            {
                ModelState.AddModelError("Message", "Item with same name already exists");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                userFromRepo.FirstName = userForUpdate.FirstName;
                userFromRepo.LastName = userForUpdate.LastName;
                userFromRepo.UserName = userForUpdate.UserName;
                //userFromRepo.Active = (userForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);
                userFromRepo.AllowDatasetDownload = (userForUpdate.AllowDatasetDownload == Models.ValueTypes.YesNoValueType.Yes);

                _userRepository.Update(userFromRepo);

                UpdateUserRoles(userForUpdate, userFromRepo);
                await UpdateUserFacilitiesAsync(userForUpdate, userFromRepo);

                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Reset the password for a user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <param name="userForPasswordUpdate">The password payload</param>
        /// <returns></returns>
        [HttpPut("{id}/password", Name = "ResetPassword")]
        [Consumes("application/json")]
        public async Task<IActionResult> ResetPassword(long id,
            [FromBody] UserForPasswordUpdateDto userForPasswordUpdate)
        {
            if (userForPasswordUpdate == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for new request");
                return BadRequest(ModelState);
            }

            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //userFromRepo.SecurityStamp = Guid.NewGuid().ToString("D");

                //String hashedNewPassword = _userManager.PasswordHasher.HashPassword(userForPasswordUpdate.Password);
                //userFromRepo.PasswordHash = hashedNewPassword;
                _userRepository.Update(userFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Confirm that the user has accepted the EULA
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <returns></returns>
        [HttpPut("{id}/accepteula", Name = "AcceptEula")]
        [Consumes("application/json")]
        public async Task<IActionResult> AcceptEula(long id)
        {
            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            if (userFromRepo.EulaAcceptanceDate != null)
            {
                ModelState.AddModelError("Message", "Eula accepted already");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                userFromRepo.EulaAcceptanceDate = DateTime.Now;

                _userRepository.Update(userFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }

        /// <summary>
        /// Delete an existing user
        /// </summary>
        /// <param name="id">The unique id of the user</param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteUser")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            if (_unitOfWork.Repository<AuditLog>().Queryable()
                .Any(a => a.User.Id == id))
            {
                ModelState.AddModelError("Message", "Unable to delete as item is in use.");
                return BadRequest(ModelState);
            }

            if (ModelState.IsValid)
            {
                var userFacilityValues = _unitOfWork.Repository<UserFacility>().Queryable().Where(c => c.User.Id == id).ToList();
                userFacilityValues.ForEach(userFacility => _userFacilityRepository.Delete(userFacility));

                _userRepository.Delete(userFromRepo);
                await _unitOfWork.CompleteAsync();
            }

            return NoContent();
        }

        private PagedCollection<T> GetUsers<T>(UserResourceParameters userResourceParameters) where T : class
        {
            var pagingInfo = new PagingInfo()
            {
                PageNumber = userResourceParameters.PageNumber,
                PageSize = userResourceParameters.PageSize
            };

            var orderby = Extensions.GetOrderBy<User>(userResourceParameters.OrderBy, "asc");

            var predicate = PredicateBuilder.New<User>(true);

            if (!String.IsNullOrWhiteSpace(userResourceParameters.SearchTerm))
            {
                predicate = predicate.And(f => f.UserName.Contains(userResourceParameters.SearchTerm.Trim()) ||
                            f.FirstName.Contains(userResourceParameters.SearchTerm.Trim()) ||
                            f.LastName.Contains(userResourceParameters.SearchTerm.Trim()));
            }

            var pagedUsersFromRepo = _userRepository.List(pagingInfo, predicate, orderby, "");
            if (pagedUsersFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedUsers = PagedCollection<T>.Create(_mapper.Map<PagedCollection<T>>(pagedUsersFromRepo),
                    pagingInfo.PageNumber,
                    pagingInfo.PageSize,
                    pagedUsersFromRepo.TotalCount);

                // Prepare pagination data for response
                var paginationMetadata = new
                {
                    totalCount = mappedUsers.TotalCount,
                    pageSize = mappedUsers.PageSize,
                    currentPage = mappedUsers.CurrentPage,
                    totalPages = mappedUsers.TotalPages,
                };

                Response.Headers.Add("X-Pagination",
                    JsonConvert.SerializeObject(paginationMetadata));

                // Add HATEOAS links to each individual resource
                //mappedUsers.ForEach(dto => CreateLinksForFacility(dto));

                return mappedUsers;
            }

            return null;
        }

        private async Task<T> GetUserAsync<T>(long id) where T : class
        {
            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);

            if (userFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedUser = _mapper.Map<T>(userFromRepo);

                return mappedUser;
            }

            return null;
        }

        private LinkedResourceBaseDto CreateLinksForUsers(
            LinkedResourceBaseDto wrapper,
            UserResourceParameters userResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            wrapper.Links.Add(
               new LinkDto(
                   _linkGeneratorService.CreateUsersResourceUri(ResourceUriType.Current, userResourceParameters),
                   "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateUsersResourceUri(ResourceUriType.NextPage, userResourceParameters),
                       "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                   new LinkDto(
                       _linkGeneratorService.CreateUsersResourceUri(ResourceUriType.PreviousPage, userResourceParameters),
                       "previousPage", "GET"));
            }

            return wrapper;
        }

        /// <summary>
        ///  Prepare HATEOAS links for a single resource
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private UserIdentifierDto CreateLinksForUser<T>(T dto)
        {
            UserIdentifierDto identifier = (UserIdentifierDto)(object)dto;

            identifier.Links.Add(new LinkDto(_linkGeneratorService.CreateResourceUri("User", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private UserDetailDto CustomUserMap(UserDetailDto dto)
        {
            //dto.Roles = _unitOfWork.Repository<UserRole>().Queryable()
            //    .Where(ur => ur.User.Id == dto.Id).Select(s => s.Role.Name).ToArray();

            dto.Facilities = _unitOfWork.Repository<UserFacility>().Queryable()
                .Where(uf => uf.User.Id == dto.Id).Select(s => s.Facility.FacilityName).ToArray();

            return dto;
        }

        /// <summary>
        ///  Handle the updating of roles for an existing usser
        /// </summary>
        /// <param name="userForUpdate">The payload containing the list of roles</param>
        /// <param name="userFromRepo">The user entity that is being updated</param>
        /// <returns></returns>
        private void UpdateUserRoles(UserForUpdateDto userForUpdate, User userFromRepo)
        {
            //var roleNames = userForUpdate.Roles.ToArray();

            //var roles = _unitOfWork.Repository<Role>().Queryable()
            //    .Where(r => roleNames.Contains(r.Name))
            //    .Select(rk => rk.Key)
            //    .ToArray();

            //// Determine what has been removed
            //ArrayList deleteCollection = new ArrayList();
            //ArrayList addCollection = new ArrayList();
            //foreach (var userRole in _unitOfWork.Repository<UserRole>().Queryable()
            //    .Include(i1 => i1.Role)
            //    .Where(r => r.User.Id == userFromRepo.Id))
            //{
            //    if (!roles.Contains(userRole.Role.Key))
            //    {
            //        deleteCollection.Add(userRole);
            //    }
            //}

            //// Determine what needs to be added
            //foreach (string role in roles)
            //{
            //    UserRole userRole = _unitOfWork.Repository<UserRole>().Queryable()
            //        .SingleOrDefault(ur => ur.User.Id == userFromRepo.Id && ur.Role.Key == role);
            //    if (userRole == null)
            //    {
            //        var newRole = _roleRepository.Get(r => r.Key == role);
            //        userRole = new UserRole() { 
            //            Role = newRole, 
            //            User = userFromRepo
            //        };
            //        addCollection.Add(userRole);
            //    }
            //}

            //foreach (UserRole userRole in deleteCollection)
            //{
            //    _userRoleRepository.Delete(userRole);
            //}

            //foreach (UserRole userRole in addCollection)
            //{
            //    _userRoleRepository.Save(userRole);
            //}
        }

        /// <summary>
        ///  Handle the updating of facilities for an existing usser
        /// </summary>
        /// <param name="userForUpdate">The payload containing the list of facilities</param>
        /// <param name="userFromRepo">The user entity that is being updated</param>
        /// <returns></returns>
        private async Task UpdateUserFacilitiesAsync(UserForUpdateDto userForUpdate, User userFromRepo)
        {
            var facilityNames = userForUpdate.Facilities.ToArray();

            var facilities = _unitOfWork.Repository<Facility>().Queryable()
                .Where(r => facilityNames.Contains(r.FacilityName))
                .Select(rk => rk.FacilityName)
                .ToArray();

            // Determine what has been removed
            ArrayList deleteCollection = new ArrayList();
            ArrayList addCollection = new ArrayList();
            var userFacilities = await _userFacilityRepository.ListAsync(r => r.User.Id == userFromRepo.Id, null, new string[] { "Facility" });

            foreach (var userFacility in userFacilities)
            {
                if (!facilities.Contains(userFacility.Facility.FacilityName))
                {
                    deleteCollection.Add(userFacility);
                }
            }

            // Determine what needs to be added
            foreach (string facility in facilities)
            {
                UserFacility userFacility = _unitOfWork.Repository<UserFacility>().Queryable()
                    .SingleOrDefault(uf => uf.User.Id == userFromRepo.Id && uf.Facility.FacilityName == facility);
                if (userFacility == null)
                {
                    var newFacility = _facilityRepository.Get(r => r.FacilityName == facility);
                    userFacility = new UserFacility()
                    {
                        Facility = newFacility,
                        User = userFromRepo
                    };
                    addCollection.Add(userFacility);
                }
            }

            foreach (UserFacility userFacility in deleteCollection)
            {
                _userFacilityRepository.Delete(userFacility);
            }
            foreach (UserFacility userFacility in addCollection)
            {
                _userFacilityRepository.Save(userFacility);
            }
        }
    }
}
