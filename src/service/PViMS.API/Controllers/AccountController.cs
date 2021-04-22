using LinqKit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PViMS.Infrastructure.Identity.Entities;
using PVIMS.API.Infrastructure.Attributes;
using PVIMS.API.Infrastructure.Auth;
using PVIMS.API.Infrastructure.Services;
using PVIMS.API.Models;
using PVIMS.API.Models.Account;
using PVIMS.Core.Entities;
using PVIMS.Core.Entities.Accounts;
using PVIMS.Core.Repositories;
using PVIMS.Core.Utilities;
using PVIMS.Core.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PVIMS.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly ITokenFactory _tokenFactory;
        private readonly IJwtFactory _jwtFactory;
        private readonly IRepositoryInt<RefreshToken> _refreshTokenRepository;
        private readonly IRepositoryInt<AuditLog> _auditLogRepository;
        private readonly IRepositoryInt<ReportInstance> _reportInstanceRepository;
        private readonly IRepositoryInt<Config> _configRepository;
        private readonly IRepositoryInt<User> _userRepository;
        private readonly IUnitOfWorkInt _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(ITokenFactory tokenFactory,
            IJwtFactory jwtFactory,
            IRepositoryInt<RefreshToken> refreshTokenRepository,
            IRepositoryInt<AuditLog> auditLogRepository,
            IRepositoryInt<ReportInstance> reportInstanceRepository,
            IRepositoryInt<Config> configRepository,
            IRepositoryInt<User> userRepository,
            IUnitOfWorkInt unitOfWork,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _tokenFactory = tokenFactory ?? throw new ArgumentNullException(nameof(tokenFactory));
            _jwtFactory = jwtFactory ?? throw new ArgumentNullException(nameof(jwtFactory));
            _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
            _auditLogRepository = auditLogRepository ?? throw new ArgumentNullException(nameof(auditLogRepository));
            _reportInstanceRepository = reportInstanceRepository ?? throw new ArgumentNullException(nameof(reportInstanceRepository));
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Ping service to check if API endpoint is available
        /// </summary>
        [HttpGet("ping", Name = "Ping")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Ping()
        {
            return Ok();
        }

        /// <summary>
        /// Ping service to check if API endpoint is available (must be authenticated)
        /// </summary>
        [HttpGet("pingauth", Name = "PingAuth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + "," + ApiKeyAuthenticationOptions.DefaultScheme)]
        public ActionResult PingAuth()
        {
            return Ok();
        }

        /// <summary>
        /// Authentication provider
        /// </summary>
        [HttpPost("login", Name = "Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Consumes("application/json")]
        public async Task<ActionResult<LoginResponseDto>> Login(
            [FromBody] LoginRequestDto request)
        {
            if (request == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for login request");
                return BadRequest(ModelState);
            }

            var userFromManager = await _userManager.FindByNameAsync(request.UserName);
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == request.UserName);
            if (userFromManager != null && userFromRepo != null)
            {
                if (await _userManager.CheckPasswordAsync(userFromManager, request.Password))
                {
                    if (userFromManager.Active)
                    {

                        var audit = new AuditLog()
                        {
                            AuditType = AuditType.UserLogin,
                            User = userFromRepo,
                            ActionDate = DateTime.Now,
                            Details = "User logged in to PViMS"
                        };
                        _auditLogRepository.Save(audit);

                        //var isAdmin = IsAdmin(user);
                        //if (!isAdmin) return RedirectToLocal(returnUrl);
                        //var pendingScriptsExist = AnyPendingScripts();

                        //// Send user to deployment page
                        //if (pendingScriptsExist)
                        //{
                        //    return RedirectToAction("Index", "Deployment");
                        //}

                        var refreshToken = _tokenFactory.GenerateToken();

                        userFromRepo.AddRefreshToken(refreshToken, HttpContext?.Connection?.RemoteIpAddress?.ToString());
                        _userRepository.Update(userFromRepo);
                        _unitOfWork.Complete();

                        return Ok(new LoginResponseDto(await _jwtFactory.GenerateEncodedToken(userFromRepo), refreshToken, userFromRepo.EulaAcceptanceDate == null, userFromRepo.AllowDatasetDownload));
                    }
                    else 
                    {
                        ModelState.AddModelError("Message", "User is not active.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Message", "Invalid password specified.");
                }
            }
            else
            {
                ModelState.AddModelError("Message", "Invalid username specified.");
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Refresh user token
        /// </summary>
        /// <remarks>Expired access tokens are valid. Refresh token can only be used once in order to obtain a new access token.</remarks>
        /// <param name="request">Exchange refresh token request model</param>
        /// <response code="200">Success</response>
        /// <response code="400">Bad Request</response>
        [HttpPost("refreshtoken", Name = "RefreshToken")]
        public async Task<ActionResult<ExchangeRefreshTokenResponseModel>> RefreshToken([FromBody] ExchangeRefreshTokenRequest request)
        {
            if (request == null)
            {
                ModelState.AddModelError("Message", "Unable to locate payload for refresh token request");
                return BadRequest(ModelState);
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromManager = await _userManager.FindByNameAsync(userName);
            var userFromRepo = await _userRepository.GetAsync(u => u.UserName == userName);

            if (userFromManager != null && userFromRepo != null)
            {
                if (userFromManager.Active == false)
                {
                    return StatusCode(401, "User no longer active");
                }

                if (userFromRepo.HasValidRefreshToken(request.RefreshToken))
                {

                    var jwtToken = await _jwtFactory.GenerateEncodedToken(userFromRepo);

                    // delete existing refresh token
                    _refreshTokenRepository.Delete(userFromRepo.RefreshTokens.Single(a => a.Token == request.RefreshToken));

                    // generate refresh token
                    var refreshToken = _tokenFactory.GenerateToken();
                    userFromRepo.AddRefreshToken(refreshToken, HttpContext?.Connection?.RemoteIpAddress?.ToString());

                    _userRepository.Update(userFromRepo);
                    _unitOfWork.Complete();

                    return new ExchangeRefreshTokenResponseModel() { AccessToken = jwtToken, RefreshToken = refreshToken };
                }
            }

            return StatusCode(404, "User does not have valid refresh token");
        }

        /// <summary>
        /// Get all notifications using a valid media type 
        /// </summary>
        /// <returns>An ActionResult of type LinkedCollectionResourceWrapperDto of NotificationDto</returns>
        [HttpGet("notifications", Name = "GetNotifications")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        [RequestHeaderMatchesMediaType("Accept",
            "application/vnd.pvims.identifier.v1+json", "application/vnd.pvims.identifier.v1+xml")]
        public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromManager = await _userManager.FindByNameAsync(userName);

            var config = _configRepository.Get(c => c.ConfigType == ConfigType.ReportInstanceNewAlertCount);
            List<NotificationDto> notifications = new List<NotificationDto>();

            if (config != null && userFromManager != null)
            {
                if (!String.IsNullOrEmpty(config.ConfigValue))
                {
                    var alertCount = Convert.ToInt32(config.ConfigValue);

                    // How many instances within the last alertcount
                    var compareDate = DateTime.Now.AddDays(alertCount * -1);

                    if (await _userManager.IsInRoleAsync(userFromManager, Constants.Role.Clinician))
                    {
                        notifications.AddRange(PrepareNotificationsForClinician(compareDate));
                    }
                    if (await _userManager.IsInRoleAsync(userFromManager, Constants.Role.PVSpecialist))
                    {
                        notifications.AddRange(PrepareNotificationsForAnalyst(compareDate));
                    }
                }
            }

            return Ok(notifications);
        }

        /// <summary>
        /// Get notification messages for clinician
        /// </summary>
        /// <returns></returns>
        private List<NotificationDto> PrepareNotificationsForClinician(DateTime compareDate)
        {
            List<NotificationDto> notifications = new List<NotificationDto>();

            // New active report causality notification
            var notification = CreateNotificationForActiveCausality(compareDate);
            if (notification != null)
            {
                notifications.Add(notification);
            }

            return notifications;
        }

        /// <summary>
        /// Get notification messages for analyst
        /// </summary>
        /// <returns></returns>
        private List<NotificationDto> PrepareNotificationsForAnalyst(DateTime compareDate)
        {
            List<NotificationDto> notifications = new List<NotificationDto>();

            // New active report notification
            var notification = CreateNotificationForActiveReports(compareDate);
            if (notification != null)
            {
                notifications.Add(notification);
            }

            // New spontaneous report notification
            notification = CreateNotificationForSpontaneousReports(compareDate);
            if (notification != null)
            {
                notifications.Add(notification);
            }

            return notifications;
        }

        /// <summary>
        ///  Prepare notification for active reports
        /// </summary>
        /// <param name="compDate">The paramterised date to check against</param>
        /// <returns></returns>
        private NotificationDto CreateNotificationForActiveReports(DateTime compDate)
        {
            var workFlowGuid = new Guid("892F3305-7819-4F18-8A87-11CBA3AEE219");

            var predicate = PredicateBuilder.New<ReportInstance>(true);

            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
            predicate = predicate.And(f => f.Created >= compDate);

            var newAnalyserNotificationCount = _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
            if (newAnalyserNotificationCount > 0)
            {
                return new NotificationDto()
                {
                    Identifier = "ActiveNotification",
                    Color = "primary",
                    Icon = "timer",
                    Message = $"New ACTIVE reports for analysis ({newAnalyserNotificationCount})",
                    Route = $"/analytical/reportsearch/{workFlowGuid.ToString().ToUpper()}",
                    Time = DateTime.Now.ToString()
                };
            }
            return null;
        }

        /// <summary>
        ///  Prepare notification for spontaneous reports
        /// </summary>
        /// <param name="compDate">The paramterised date to check against</param>
        /// <returns></returns>
        private NotificationDto CreateNotificationForSpontaneousReports(DateTime compDate)
        {
            var workFlowGuid = new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986");

            var predicate = PredicateBuilder.New<ReportInstance>(true);
            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == workFlowGuid);
            predicate = predicate.And(f => f.Created >= compDate);

            var newAnalyserNotificationCount = _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
            if (newAnalyserNotificationCount > 0)
            {
                return new NotificationDto()
                {
                    Identifier = "SpontaneousNotification",
                    Color = "primary",
                    Icon = "timer",
                    Message = $"New SPONTANEOUS reports for analysis ({newAnalyserNotificationCount})",
                    Route = $"/analytical/reportsearch/{workFlowGuid.ToString().ToUpper()}",
                    Time = DateTime.Now.ToString()
                };
            }
            return null;
        }

        /// <summary>
        ///  Prepare notification for active causality reports
        /// </summary>
        /// <param name="compDate">The paramterised date to check against</param>
        /// <returns></returns>
        private NotificationDto CreateNotificationForActiveCausality(DateTime compDate)
        {
            var activeWorkFlowGuid = new Guid("892F3305-7819-4F18-8A87-11CBA3AEE219");

            var predicate = PredicateBuilder.New<ReportInstance>(true);

            predicate = predicate.And(f => f.WorkFlow.WorkFlowGuid == activeWorkFlowGuid);
            predicate = predicate.And(f => f.Activities.Any(a => a.QualifiedName == "Set MedDRA and Causality" && a.CurrentStatus.Description == "CAUSALITYSET" && a.Created >= compDate));

            var newAnalyserNotificationCount = _reportInstanceRepository.List(predicate, null, new string[] { "" }).Count;
            if (newAnalyserNotificationCount > 0)
            {
                return new NotificationDto()
                {
                    Identifier = "ActiveCausality",
                    Color = "primary",
                    Icon = "timer",
                    Message = $"New ACTIVE reports with causality and terminology set ({newAnalyserNotificationCount})",
                    Route = "/clinical/feedbacksearch",
                    Time = DateTime.Now.ToString()
                };
            }
            return null;
        }
    }
}
