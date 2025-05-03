namespace Domain.DTOs.Article;

public class ArticleDto
{
    public Guid ArticleId { get; set; }
    public string URL { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsFavourite { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public List<string> CategoryNames { get; set; } = [];
}
