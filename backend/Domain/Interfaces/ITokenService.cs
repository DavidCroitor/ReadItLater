using System.Security.Claims;
using Data.Models;

namespace Domain.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        Task<RefreshToken> CreateRefreshTokenAsync(User user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
        Task<bool> ValidateRefreshTokenAsync(string token, Guid userId);
    }
}