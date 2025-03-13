using ControlGestionAPI.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken(User user);
        Task<User> Authenticate(string username, string password);
        int GetRefreshTokenExpirationDays();
        ClaimsPrincipal VerifyRefreshToken(string refreshToken);
    }
}