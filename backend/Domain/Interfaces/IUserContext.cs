using System.Security.Claims;

namespace Domain.Interfaces;
public interface IUserContext
{
    Guid? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
    Claim? FindClaim(string claimType);
}