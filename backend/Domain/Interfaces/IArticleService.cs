using Domain.DTOs.Article;
using Data.Models;


namespace Domain.Interfaces;


public interface IArticleService
{
    Task<ArticleDto?> AddArticleForUserAsync(CreateArticleRequestDto requestDto, Guid userId);
    Task<Article?> GetArticleForUserAsync(Guid articleId, Guid userId);
    Task<IEnumerable<ArticleDto>> GetArticlesForUserAsync(Guid userId);
    Task<ArticleDto?> UpdateArticleForUserAsync(Guid articleId, Guid userId, UpdateUserArticleDto updateDto);
    Task<ArticleDto?> DeleteArticleForUserAsync(Guid articleId, Guid userId);
}
