using Data.Models;

namespace Data.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> FindInvalidTokensForUserAsync(Guid userId);
    Task<RefreshToken?> UpdateAsync(RefreshToken refreshToken);
    Task RemoveRangeAsync(IEnumerable<RefreshToken> refreshTokens);
    Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId);
    Task<bool> ValidateTokenAsync(string token, Guid userId); 
}