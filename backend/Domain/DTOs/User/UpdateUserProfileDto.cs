using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User;

public class UpdateUserProfileDto
{   
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters long.")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters and numbers.")]
    public string? Username { get; set; }
}