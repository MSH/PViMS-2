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
using Extensions = PVIMS.Core.Utilities.Extensions;
using PVIMS.Core.Repositories;
using PVIMS.Core.Paging;
using PVIMS.Infrastructure.Identity.Entities;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PVIMS.API.Application.Commands.UserAggregate;
using MediatR;
using PVIMS.API.Application.Queries.UserAggregate;
using PVIMS.Core.Aggregates.UserAggregate;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IRepositoryInt<UserFacility> _userFacilityRepository;
        private readonly IRepositoryInt<Facility> _facilityRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILinkGeneratorService _linkGeneratorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator,
            ITypeHelperService typeHelperService,
            IMapper mapper,
            ILinkGeneratorService linkGeneratorService,
            IRepositoryInt<User> userRepository,
            IRepositoryInt<UserFacility> userFacilityRepository,
            IRepositoryInt<Facility> facilityRepository,
            IUnitOfWorkInt unitOfWork,
            UserManager<ApplicationUser> userManager,
            ILogger<UsersController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _typeHelperService = typeHelperService ?? throw new ArgumentNullException(nameof(typeHelperService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _linkGeneratorService = linkGeneratorService ?? throw new ArgumentNullException(nameof(linkGeneratorService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userFacilityRepository = userFacilityRepository ?? throw new ArgumentNullException(nameof(userFacilityRepository));
            _facilityRepository = facilityRepository ?? throw new ArgumentNullException(nameof(facilityRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        public async Task<ActionResult<LinkedCollectionResourceWrapperDto<UserIdentifierDto>>> GetUsersByIdentifier(
            [FromQuery] UserResourceParameters userResourceParameters)
        {
            if (!_typeHelperService.TypeHasProperties<UserIdentifierDto>
                (userResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            var query = new UsersIdentifierQuery(
                userResourceParameters.OrderBy,
                userResourceParameters.PageNumber,
                userResourceParameters.PageSize,
                userResourceParameters.SearchTerm);

            _logger.LogInformation(
                "----- Sending query: UsersIdentifierQuery");

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            // Prepare pagination data for response
            var paginationMetadata = new
            {
                totalCount = queryResult.RecordCount,
                pageSize = userResourceParameters.PageSize,
                currentPage = userResourceParameters.PageNumber,
                totalPages = queryResult.PageCount
            };

            Response.Headers.Add("X-Pagination",
                JsonConvert.SerializeObject(paginationMetadata));

            return Ok(queryResult);
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
        /// <returns>An ActionResult of type UserIdentifierDto</returns>
        [HttpGet("{id}", Name = "GetUserByIdentifier")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<UserIdentifierDto>> GetUserByIdentifier(int id)
        {
            var query = new UserIdentifierQuery(id);

            _logger.LogInformation(
                "----- Sending query: UserIdentifierQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
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
        public async Task<ActionResult<UserDetailDto>> GetUserByDetail(int id)
        {
            var query = new UserDetailQuery(id);

            _logger.LogInformation(
                "----- Sending query: UserDetailQuery - {id}",
                id);

            var queryResult = await _mediator.Send(query);

            if (queryResult == null)
            {
                return BadRequest("Query not created");
            }

            return Ok(queryResult);
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

            var command = new AddUserCommand(userForCreation.FirstName, userForCreation.LastName, userForCreation.Email, userForCreation.UserName, userForCreation.Password, userForCreation.ConfirmPassword, userForCreation.Roles, userForCreation.Facilities);

            _logger.LogInformation(
                "----- Sending command: AddUserCommand - {userName}",
                command.UserName);

            var commandResult = await _mediator.Send(command);

            if (commandResult == null)
            {
                return BadRequest("Command not created");
            }

            return CreatedAtAction("GetUserByIdentifier",
                new
                {
                    id = commandResult.Id
                }, commandResult);
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
                    userFacility = new UserFacility(newFacility, userFromRepo);
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
