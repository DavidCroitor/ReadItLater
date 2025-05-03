using Data.Data;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Category> _categories;
    private readonly DbSet<Article> _articleMetadatas;
    private readonly ILogger<CategoryRepository> _logger;
    public CategoryRepository(
        ApplicationDbContext context,
        ILogger<CategoryRepository> logger)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context), "Database connection cannot be null.");

        if (logger == null)
            throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");

        _context = context;
        _categories = context.Set<Category>();
        _articleMetadatas = context.Set<Article>();
        _logger = logger;
    }

    public async Task<Category?> GetByIdAsync(Guid categoryId, Guid userId)
    {
        return await _categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);
    }

    public async Task<Category?> GetByNameAsync(string name, Guid userId)
    {
        return await _categories.FirstOrDefaultAsync(c => c.Name == name && c.UserId == userId);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        
        _logger.LogInformation("Fetching all categories for user with ID: {UserId}", userId);
        return await _categories.Where(c => c.UserId == userId).ToListAsync();
    }

    public async Task<Category> AddAsync(Category entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity), "Category cannot be null.");
        
        _logger.LogInformation("Adding a new category: {Category}", entity.Name);
        try 
        {
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Category added successfully: {Category}", entity.Name);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding Category with Name: {CategoryName}", entity.Name);
            throw;
        }
    }

    public async Task<Category> DeleteAsync(Guid categoryId, Guid userId)
    {
        var entityToDelete = await _categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);
        if (entityToDelete == null)
        {
            _logger.LogWarning("Attempted to delete a non-existing category or category does not belong to the user");
            throw new KeyNotFoundException($"Category with ID '{categoryId}' not found or does not belong to the user.");
        }
        _logger.LogInformation("Deleting category with ID: {Id}", categoryId);
        _context.Categories.Remove(entityToDelete);
        await _context.SaveChangesAsync();

        return entityToDelete;
    }

    public async Task<Category> UpdateAsync(Guid categoryId, Guid userId, Category entity)
    {
        if(entity == null)
            throw new ArgumentNullException(nameof(entity), "Category cannot be null.");
        
        var existingEntity = await _categories.FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);
        if (existingEntity == null)
        {
            _logger.LogWarning("Attempted to update a non-existing category or category does not belong to the user");
            throw new KeyNotFoundException($"Category with ID '{categoryId}' not found or does not belong to the user.");
        }
        _logger.LogInformation("Updating category with ID: {Id}", categoryId);

        existingEntity.Name = entity.Name;
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated Category with ID: {CategoryId}", categoryId);
            return existingEntity;
        }
        catch (DbUpdateException ex)
        {
             // Check for unique constraint violation on Name during update
            if (ex.InnerException?.Message.Contains("Cannot insert duplicate key row") == true &&
                ex.InnerException.Message.Contains("IX_Categories_Name") == true)
            {
                _logger.LogWarning("Attempted to update Category ID {CategoryId} to duplicate Name: {CategoryName}", categoryId, entity.Name);
                throw new InvalidOperationException($"A category with the name '{entity.Name}' already exists.", ex);
            }
             _logger.LogError(ex, "Error updating Category with ID: {CategoryId}", categoryId);
            throw; // Re-throw other DbUpdateExceptions
        }
    }

    public async Task<IEnumerable<Article>> GetArticlesInCategoryForUserAsync(Guid categoryId, Guid userId)
    {
        var articles = await _articleMetadatas
            .Include(ua => ua.Categories)
            .Where(ua => ua.UserId == userId)
            .Where(ua => ua.Categories.Any(c => c.Id == categoryId))
            .AsNoTracking()
            .ToListAsync();

        return articles;
    }

    public async Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<Guid> ids, Guid userId)
    {
        // 1. Handle null or empty input collection
        if (ids == null || !ids.Any())
        {
            _logger.LogInformation("GetByIdsAsync called with null or empty ID list.");
            return Enumerable.Empty<Category>();
        }

        // 2. Ensure IDs are distinct to potentially optimize the SQL query
        var distinctIds = ids.Distinct().ToList(); 

        _logger.LogInformation("Attempting to fetch Categories for {IdCount} distinct IDs.", distinctIds.Count);

        try
        {
            var categories = await _categories
                .Where(category => distinctIds.Contains(category.Id) && category.UserId == userId)
                .ToListAsync();

            _logger.LogInformation("Successfully fetched {FoundCount} Categories.", categories.Count);


            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching Categories by IDs. IDs: {CategoryIds}", string.Join(", ", distinctIds));
            throw;
        }
    }
     public async Task<Article> AddArticleToCategoryAsync(Guid categoryId, Guid userArticleId)
    {
        var category = await _categories.FindAsync(categoryId);
        var article = await _articleMetadatas
                            .Include(ua => ua.Categories)
                            .FirstOrDefaultAsync(ua => ua.Id == userArticleId);
        
        if (category == null)
            throw new KeyNotFoundException($"Category with ID '{categoryId}' not found.");
        if (article == null)
            throw new KeyNotFoundException($"Article with ID '{userArticleId}' not found.");
        
        if (article.Categories.Any(c => c.Id == categoryId))
            throw new InvalidOperationException($"Article with ID '{userArticleId}' already exists in category '{categoryId}'.");
        
        _logger.LogInformation("Adding article with ID: {ArticleId} to category with ID: {CategoryId}", userArticleId, categoryId);
        article.Categories.Add(category);

        try 
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added article with ID: {ArticleId} to category with ID: {CategoryId}", userArticleId, categoryId);
            return article;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error adding article with ID: {ArticleId} to category with ID: {CategoryId}", userArticleId, categoryId);
            throw; // Re-throw other DbUpdateExceptions
        }
        
    }

}
