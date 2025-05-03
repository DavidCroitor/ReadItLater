using Data.Data;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;
public class ArticleRepository : IArticleRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<Article> _articles;
    private readonly ILogger<ArticleRepository> _logger;

    public ArticleRepository(
        ApplicationDbContext context,
        ILogger<ArticleRepository> logger)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context), "Database connection cannot be null.");

        if (logger == null)
            throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");

        _context = context;
        _articles = context.Set<Article>();
        _logger = logger;
    }
    
    public async Task<IEnumerable<Article>> GetAllAsync(Guid userId)
    {
        return await _articles.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task<Article?> GetByIdAsync(Guid articleId, Guid userId)
    {
        return await _articles.FirstOrDefaultAsync(a => a.Id == articleId && a.UserId == userId);
    }

    public async Task<Article> AddAsync(Article entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity), "Article cannot be null.");

        _logger.LogInformation("Adding a new article: {Article}", entity.Title);
        try 
        {
            await _articles.AddAsync(entity);
            _context.SaveChanges();
            _logger.LogInformation("Article added successfully: {Article}", entity.Title);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding article: {Article}", entity.Title);
            throw;
        }
    }

    public async Task<Article> DeleteAsync(Guid id, Guid userId)
    {
        var articleToDelete = _articles.FirstOrDefault(a => a.Id == id && a.UserId == userId);
        if (articleToDelete == null)
            throw new ArgumentException("Article not found.", nameof(id));
        
        _context.Articles.Remove(articleToDelete);
        await _context.SaveChangesAsync();

        return articleToDelete;
    }

    public async Task<Article> UpdateAsync(Guid id, Guid userId, Article entity)
    {
        var articleToUpdate = _articles.FirstOrDefault(a => a.Id == id && a.UserId == userId);
        if (articleToUpdate == null)
            throw new ArgumentException("Article not found.", nameof(id));

        _context.Entry(articleToUpdate).CurrentValues.SetValues(entity);
        _context.SaveChanges();
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Article?> GetByUrlAsync(string url, Guid userId)
    {
       return await _articles.FirstOrDefaultAsync(a => a.URL == url && a.UserId == userId);
    }

    public async Task<Article?> GetByIdWithCategoriesAsync(Guid articleId, Guid userId)
    {
        return await _articles
            .Include(a => a.Categories)
            .FirstOrDefaultAsync(a => a.Id == articleId && a.UserId == userId);
    }
}