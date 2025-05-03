using Data.Models;

namespace Data.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category, Guid>
{
    Task<Category?> GetByNameAsync(string name, Guid userId);
    Task<IEnumerable<Article>> GetArticlesInCategoryForUserAsync(Guid id, Guid userId);
    Task <IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> Ids, Guid userId);

}