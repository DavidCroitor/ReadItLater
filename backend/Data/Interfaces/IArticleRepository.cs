using Data.Models;

namespace Data.Interfaces;

public interface IArticleRepository : IGenericRepository<Article, Guid>
{
    Task<Article?> GetByUrlAsync(string url, Guid userId);
    Task<Article?> GetByIdWithCategoriesAsync(Guid articleId, Guid userId);
}