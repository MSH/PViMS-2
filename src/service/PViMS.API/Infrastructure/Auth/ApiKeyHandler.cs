using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PVIMS.API.Infrastructure.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Auth
{
    public class ApiKeyHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        
        public ApiKeyHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) 
                : base(options, logger, encoder, clock) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            var configuration = Request.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var configAuthSettings = configuration.GetSection(nameof(AuthSettings));
            var apiKey = configAuthSettings.GetValue<string>(key: "ApiKey");
            var apiClientName = configAuthSettings.GetValue<string>(key: "ApiClientName");

            if (!apiKey.Equals(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, apiClientName),
                new Claim(ClaimTypes.Role, "ClientApp")
            };

            var identities = new List<ClaimsIdentity> { new ClaimsIdentity(claims, Options.AuthenticationType) };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}
