using Data.Data;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<RefreshToken> _refreshTokens;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
        _refreshTokens = _context.Set<RefreshToken>();
    }

    public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
    {
        await _refreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        var refreshToken = await _refreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && !rt.IsUsed);

        if (refreshToken != null)
        {
            refreshToken.IsUsed = true;
            _context.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
        return refreshToken;
    }

    public async Task<IEnumerable<RefreshToken>> FindInvalidTokensForUserAsync(Guid userId)
    {
        return await _refreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId && (rt.IsRevoked || rt.IsUsed || rt.Expires < DateTime.UtcNow))
            .ToListAsync();
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId)
    {
        return await _refreshTokens
            .AsNoTracking()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsUsed && rt.Expires > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task RemoveRangeAsync(IEnumerable<RefreshToken> refreshTokens)
    {
        _context.RemoveRange(refreshTokens);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> UpdateAsync(RefreshToken refreshToken)
    {
        _refreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<bool> ValidateTokenAsync(string token, Guid userId)
    {
        var refreshToken = await _refreshTokens.FirstOrDefaultAsync(rt => rt.Token == token && rt.UserId == userId);
        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.IsUsed || refreshToken.Expires < DateTime.UtcNow)
        {
            return false;
        }
        
        refreshToken.IsUsed = true;
        await _context.SaveChangesAsync();
        return true;
    }
}

    