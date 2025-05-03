using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Category;

public class CreateCategoryRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Name should be between 1 and 50 characters long")]
    public string Name { get; set; } = String.Empty;
}
