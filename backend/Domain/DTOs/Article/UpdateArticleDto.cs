namespace Domain.DTOs.Article;

public class UpdateUserArticleDto
{
    public bool? IsRead { get; set; }
    public bool? IsFavourite { get; set; }
    public List<Guid>? CategoryIds { get; set; } 
    
}
