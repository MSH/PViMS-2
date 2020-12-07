using AutoMapper;
using LinqKit;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PVIMS.API.Attributes;
using PVIMS.API.Helpers;
using PVIMS.API.Models;
using PVIMS.API.Models.Account;
using PVIMS.API.Models.Parameters;
using PVIMS.API.Services;
using PVIMS.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VPS.Common.Collections;
using VPS.Common.Repositories;
using Extensions = PVIMS.Core.Utilities.Extensions;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<Role> _roleRepository;
        private readonly IRepositoryInt<UserRole> _userRoleRepository;
        private readonly IRepositoryInt<UserFacility> _userFacilityRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly UserInfoManager _userManager;

        public UsersController(ITypeHelperService typeHelperService,
            IMapper mapper,
            IUrlHelper urlHelper,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<Role> roleRepository,
            IRepositoryInt<UserRole> userRoleRepository,
            IRepositoryInt<UserFacility> userFacilityRepository,
            IRepositoryInt<Facility> facilityRepository,
            IUnitOfWorkInt unitOfWork,
            UserInfoManager userManager)
        {
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
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

            return Ok(GetUserRoles(id));
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

            long id = 0;

            if (ModelState.IsValid)
            {
                var user = new UserInfo() { 
                    FirstName = userForCreation.FirstName, 
                    LastName = userForCreation.LastName, 
                    UserName = userForCreation.UserName, 
                    Email = userForCreation.Email, 
                    Password = userForCreation.Password 
                };
                IdentityResult result = await _userManager.CreateAsync(user, userForCreation.Password);
                if (result.Succeeded)
                {
                    id = user.Id;

                    var roleNames = userForCreation.Roles.ToArray();

                    var roles = _unitOfWork.Repository<Role>().Queryable()
                        .Where(r => roleNames.Contains(r.Name))
                        .Select(rk => rk.Key)
                        .ToArray();

                    IdentityResult roleResult = await _userManager.AddToRolesAsync(user.Id, roles);

                    if (roleResult.Succeeded)
                    {
                        var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);
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
                        _unitOfWork.Complete();

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
                            ModelState.AddModelError("Message", error);
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Message", error);
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
                userFromRepo.Email = userForUpdate.Email;
                userFromRepo.Active = (userForUpdate.Active == Models.ValueTypes.YesNoValueType.Yes);
                userFromRepo.AllowDatasetDownload = (userForUpdate.AllowDatasetDownload == Models.ValueTypes.YesNoValueType.Yes);

                _userRepository.Update(userFromRepo);

                UpdateUserRoles(userForUpdate, userFromRepo);
                UpdateUserFacilities(userForUpdate, userFromRepo);

                _unitOfWork.Complete();
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
                userFromRepo.SecurityStamp = Guid.NewGuid().ToString("D");

                String hashedNewPassword = _userManager.PasswordHasher.HashPassword(userForPasswordUpdate.Password);
                userFromRepo.PasswordHash = hashedNewPassword;
                _userRepository.Update(userFromRepo);
                _unitOfWork.Complete();
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
                _unitOfWork.Complete();
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
                var userRoleValues = _unitOfWork.Repository<UserRole>().Queryable().Where(c => c.User.Id == id).ToList();
                userRoleValues.ForEach(userRole => _userRoleRepository.Delete(userRole));

                var userFacilityValues = _unitOfWork.Repository<UserFacility>().Queryable().Where(c => c.User.Id == id).ToList();
                userFacilityValues.ForEach(userFacility => _userFacilityRepository.Delete(userFacility));

                _userRepository.Delete(userFromRepo);
                _unitOfWork.Complete();
            }

            return NoContent();
        }

        /// <summary>
        /// Get users from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="userResourceParameters">Standard parameters for representing resource</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get user roles from repository and auto map to Dto
        /// </summary>
        /// <param name="id">The unique id of the user being interrogated</param>
        /// <returns></returns>
        private UserRoleDto[] GetUserRoles(int id)
        {
            return _mapper.Map<UserRoleDto[]>(_unitOfWork.Repository<UserRole>().Queryable()
                            .Include(i1 => i1.Role)
                            .Where(r => r.User.Id == id).ToArray());
        }

        /// <summary>
        /// Get single user from repository and auto map to Dto
        /// </summary>
        /// <typeparam name="T">Identifier or detail Dto</typeparam>
        /// <param name="id">Resource id to search by</param>
        /// <returns></returns>
        private async Task<T> GetUserAsync<T>(long id) where T : class
        {
            var userFromRepo = await _userRepository.GetAsync(f => f.Id == id);

            if (userFromRepo != null)
            {
                // Map EF entity to Dto
                var mappedLabTest = _mapper.Map<T>(userFromRepo);

                return mappedLabTest;
            }

            return null;
        }

        /// <summary>
        /// Prepare HATEOAS links for a identifier based collection resource
        /// </summary>
        /// <param name="wrapper">The linked dto wrapper that will host each link</param>
        /// <param name="userResourceParameters">Standard parameters for representing resource</param>
        /// <param name="hasNext">Are there additional pages</param>
        /// <param name="hasPrevious">Are there previous pages</param>
        /// <returns></returns>
        private LinkedResourceBaseDto CreateLinksForUsers(
            LinkedResourceBaseDto wrapper,
            UserResourceParameters userResourceParameters,
            bool hasNext, bool hasPrevious)
        {
            // self 
            wrapper.Links.Add(
               new LinkDto(CreateResourceUriHelper.CreateUsersResourceUri(_urlHelper, ResourceUriType.Current, userResourceParameters),
               "self", "GET"));

            if (hasNext)
            {
                wrapper.Links.Add(
                  new LinkDto(CreateResourceUriHelper.CreateUsersResourceUri(_urlHelper, ResourceUriType.NextPage, userResourceParameters),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                wrapper.Links.Add(
                    new LinkDto(CreateResourceUriHelper.CreateUsersResourceUri(_urlHelper, ResourceUriType.PreviousPage, userResourceParameters),
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

            identifier.Links.Add(new LinkDto(CreateResourceUriHelper.CreateResourceUri(_urlHelper, "User", identifier.Id), "self", "GET"));

            return identifier;
        }

        /// <summary>
        ///  Map additional dto detail elements not handled through automapper
        /// </summary>
        /// <param name="dto">The dto that the link has been added to</param>
        /// <returns></returns>
        private UserDetailDto CustomUserMap(UserDetailDto dto)
        {
            dto.Roles = _unitOfWork.Repository<UserRole>().Queryable()
                .Where(ur => ur.User.Id == dto.Id).Select(s => s.Role.Name).ToArray();

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
            var roleNames = userForUpdate.Roles.ToArray();

            var roles = _unitOfWork.Repository<Role>().Queryable()
                .Where(r => roleNames.Contains(r.Name))
                .Select(rk => rk.Key)
                .ToArray();

            // Determine what has been removed
            ArrayList deleteCollection = new ArrayList();
            ArrayList addCollection = new ArrayList();
            foreach (var userRole in _unitOfWork.Repository<UserRole>().Queryable()
                .Include(i1 => i1.Role)
                .Where(r => r.User.Id == userFromRepo.Id))
            {
                if (!roles.Contains(userRole.Role.Key))
                {
                    deleteCollection.Add(userRole);
                }
            }

            // Determine what needs to be added
            foreach (string role in roles)
            {
                UserRole userRole = _unitOfWork.Repository<UserRole>().Queryable()
                    .SingleOrDefault(ur => ur.User.Id == userFromRepo.Id && ur.Role.Key == role);
                if (userRole == null)
                {
                    var newRole = _roleRepository.Get(r => r.Key == role);
                    userRole = new UserRole() { 
                        Role = newRole, 
                        User = userFromRepo
                    };
                    addCollection.Add(userRole);
                }
            }

            foreach (UserRole userRole in deleteCollection)
            {
                _userRoleRepository.Delete(userRole);
            }

            foreach (UserRole userRole in addCollection)
            {
                _userRoleRepository.Save(userRole);
            }
        }

        /// <summary>
        ///  Handle the updating of facilities for an existing usser
        /// </summary>
        /// <param name="userForUpdate">The payload containing the list of facilities</param>
        /// <param name="userFromRepo">The user entity that is being updated</param>
        /// <returns></returns>
        private void UpdateUserFacilities(UserForUpdateDto userForUpdate, User userFromRepo)
        {
            var facilityNames = userForUpdate.Facilities.ToArray();

            var facilities = _unitOfWork.Repository<Facility>().Queryable()
                .Where(r => facilityNames.Contains(r.FacilityName))
                .Select(rk => rk.FacilityName)
                .ToArray();

            // Determine what has been removed
            ArrayList deleteCollection = new ArrayList();
            ArrayList addCollection = new ArrayList();
            foreach (var userFacility in _unitOfWork.Repository<UserFacility>().Queryable()
                .Include(i1 => i1.Facility)
                .Where(r => r.User.Id == userFromRepo.Id))
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
