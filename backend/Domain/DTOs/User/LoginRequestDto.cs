using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.User;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(100)]
    public string LogInIdentifier { get; set; } = string.Empty; //could be username or email
    
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

}