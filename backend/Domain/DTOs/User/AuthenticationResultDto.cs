using Data.Models;

namespace Domain.DTOs.User;

public class AuthenticationResultDto
{
    public string? AccessToken { get; set; }
    public DateTime? AccessTokenExpiration { get; set; }
    public string? RefreshToken { get; set; }
    public UserDto? UserDto { get; set; }

}