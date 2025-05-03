namespace Data.Interfaces;

public interface IGenericRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(TKey id, TKey userId);
    Task<IEnumerable<TEntity>> GetAllAsync(TKey userId);
    Task<TEntity> UpdateAsync(TKey id, TKey userId, TEntity entity);
    Task<TEntity> DeleteAsync(TKey id, TKey userId);
}