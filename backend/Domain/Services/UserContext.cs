using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Domain.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? CurrentPrincipal => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId 
    {
        get
        {
            
            var userIdClaim = CurrentPrincipal?.FindFirst(ClaimTypes.NameIdentifier);
            
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
    }

    public string? Username => CurrentPrincipal?.Identity?.Name;

    public bool IsAuthenticated => CurrentPrincipal?.Identity?.IsAuthenticated ?? false;

    public Claim? FindClaim(string claimType) => CurrentPrincipal?.FindFirst(claimType);
}