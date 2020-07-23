using PVIMS.API.Auth;
using PVIMS.Core.Entities;
using System.Threading.Tasks;

namespace PVIMS.API.Services
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(User user, UserRole[] roles, UserFacility[] facilities);
    }
}
