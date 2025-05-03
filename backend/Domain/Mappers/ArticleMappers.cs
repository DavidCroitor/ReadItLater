using Domain.DTOs.Article;
using Data.Models;

namespace Domain.Mappers;

public static class ArticleMappers
{
    public static ArticleDto ToArticleDto(this Article article)
    {
        if (article == null) throw new ArgumentNullException(nameof(article));

        return new ArticleDto
        {
            ArticleId = article.Id,
            URL = article.URL,
            Title = article.Title,
            IsRead = article.IsRead,
            IsFavourite = article.IsFavourite,
            Content = article.Content.Length < 100 ? article.Content : article.Content.Substring(0, 100) + "...",
            SavedAt = article.SavedAt,
            CategoryNames = article.Categories.Select(c => c.Name).ToList()
        };
    }
    
}
