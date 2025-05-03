using System.ComponentModel.DataAnnotations;
using Domain.Validators;

namespace Domain.DTOs.Article;


public class CreateArticleRequestDto
{
    [Required(ErrorMessage = "URL is required.")]
    [MustBeValidUrl(requireHttps: false, uriKind: UriKind.Absolute, ErrorMessage = "The URL must be a valid and well-formed URL.")]
    public string URL { get; set; } = String.Empty;


}
