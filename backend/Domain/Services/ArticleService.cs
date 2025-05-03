using Domain.DTOs.Article;
using Domain.Interfaces;
using Data.Interfaces;
using Data.Models;
using Domain.Mappers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace Domain.Services;


public class ArticleService : IArticleService
{
    // private readonly IArticleContentRepository _articleContentRepository;
    // private readonly IArticleMetadataRepository _articleMetadataRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ArticleService> _logger;

    public ArticleService(
        // IArticleContentRepository articleContentRepository, 
        // IArticleMetadataRepository articleMetadatas,
        IArticleRepository articleRepository,
        ICategoryRepository categoryRepository,
        ILogger<ArticleService> logger)
    {
        // _articleContentRepository = articleContentRepository;
        // _articleMetadataRepository = articleMetadatas;
        _articleRepository = articleRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    public async Task<ArticleDto?> AddArticleForUserAsync(CreateArticleRequestDto articleModel, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authorized");
            throw new UnauthorizedAccessException("User is not authorized");
        }

        string normalizedUrl = NormalizeUrl(articleModel.URL);

        if(!Uri.TryCreate(normalizedUrl, UriKind.Absolute, out _))
        {
            _logger.LogError($"Invalid URL format: {normalizedUrl}");
            throw new ArgumentException($"Invalid URL format: {normalizedUrl}");
        }

        var existingArticle = await _articleRepository.GetByUrlAsync(normalizedUrl, userId);
        if (existingArticle != null)
        {
            return existingArticle.ToArticleDto();
        }     

        var scrapedContent = await TryScrapeArticleContentAsync(normalizedUrl);
        if (string.IsNullOrWhiteSpace(scrapedContent))
        {
           throw new Exception($"Failed to scrape content for URL: {normalizedUrl}");
        }  
        var title = await TryGetTitleAsync(normalizedUrl);
        if (string.IsNullOrWhiteSpace(title))
        {
            _logger.LogWarning($"Failed to get title for URL: {normalizedUrl}");
            title = "Untitled"; // Fallback title
        }

        _logger.LogInformation("Attempting to create article. Title: {0}, Scraped Content Snippet: {1}", 
            title, 
            scrapedContent.Length > 100 ? scrapedContent.Substring(0, 100) + "..." : scrapedContent);

        var article = new Article(
            userId,
            normalizedUrl,
            title,
            scrapedContent
        );

        await _articleRepository.AddAsync(article);
        _logger.LogInformation($"Added article {article.Id} for user {userId}");

        _logger.LogInformation($"Added article {article.Id} for user {userId}");

        return article.ToArticleDto();
    }
    
    public async Task<Article?> GetArticleForUserAsync(Guid articleId, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User does not exist");
            throw new UnauthorizedAccessException("User does not exist");
        }
        
        var article = await _articleRepository.GetByIdAsync(articleId, userId);

        if (article == null)
        {
            _logger.LogWarning("User {UserId} has not saved Article {ArticleId}", userId, articleId);
            return null;
        }

        return article;
    }
    
    public async Task<IEnumerable<ArticleDto>> GetArticlesForUserAsync(Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User does not exist");
            throw new UnauthorizedAccessException("User does not exist");
        }
        var userArticles = await _articleRepository.GetAllAsync(userId);
        return userArticles.Select(ua => ua.ToArticleDto());
    }
    
    public async Task<ArticleDto?> UpdateArticleForUserAsync(Guid articleId, Guid userId, UpdateUserArticleDto articleModel)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User does not exist");
            throw new UnauthorizedAccessException("User does not exist");
        }

        var article = await _articleRepository.GetByIdWithCategoriesAsync(articleId, userId);
        if (article == null)
        {
            _logger.LogError($"User {userId} has not saved Article {articleId}");
            throw new Exception($"User {userId} has not saved Article {articleId}");
        }
        

        bool needsUpdate = false;
        // Update the article metadata
        if(articleModel.IsRead.HasValue && article.IsRead != articleModel.IsRead.Value)
        {
            article.IsRead = articleModel.IsRead.Value;
            needsUpdate = true;
        }
        if(articleModel.IsFavourite.HasValue && article.IsFavourite != articleModel.IsFavourite.Value)
        {
            article.IsFavourite = articleModel.IsFavourite.Value;
            needsUpdate = true;
        }

        if(articleModel.CategoryIds != null)
        {
            needsUpdate = true;

            var currentCategoryIds = article.Categories.Select(c => c.Id).ToHashSet();
            var desiredCategoryIds = articleModel.CategoryIds.ToHashSet();

            // Find Categories to ADD
            var idsToAdd = desiredCategoryIds.Except(currentCategoryIds).ToList();
            if (idsToAdd.Any())
            {
                var categoriesToAdd = await _categoryRepository.GetByIdsAsync(idsToAdd, userId);
                if (categoriesToAdd.Count() != idsToAdd.Count) {
                    _logger.LogWarning("Some category IDs provided for addition were not found.");
                }
                foreach (var category in categoriesToAdd)
                {
                    article.Categories.Add(category);
                }
            }

            // Find Categories to REMOVE
            var idsToRemove = currentCategoryIds.Except(desiredCategoryIds).ToList();
            if (idsToRemove.Any())
            {
                var categoriesToRemove = article.Categories
                                            .Where(c => idsToRemove.Contains(c.Id))
                                            .ToList();
                foreach (var category in categoriesToRemove)
                {
                    article.Categories.Remove(category);
                }
            }

        }

         if (needsUpdate)
            {
                await _articleRepository.UpdateAsync(article.Id, userId, article);
                _logger.LogInformation("Updated ArticleMetadata {ArticleId} for User {UserId}", articleId, userId);
            }
        else
            {
                _logger.LogInformation("No updates required for ArticleMetadata {ArticleId} for User {UserId}", articleId, userId);
            }

        return article.ToArticleDto();
        
    }

    public async Task<ArticleDto?> DeleteArticleForUserAsync(Guid articleId, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User does not exist");
            throw new UnauthorizedAccessException("User does not exist");
        }

        var article = await _articleRepository.GetByIdAsync(articleId, userId);
        if (article == null)
        {
            _logger.LogError($"User {userId} has not saved Article {articleId}");
            return null;
        }

        await _articleRepository.DeleteAsync(article.Id, userId);
        _logger.LogInformation($"Deleted article {articleId} for user {userId}");
        return article.ToArticleDto();
    }


    /* ===== HELPERS METHODS ===== */
    private string NormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;

        try
        {
            // Ensure scheme
             if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url; // Default to https
            }

            var uri = new Uri(url, UriKind.Absolute);

            var builder = new UriBuilder(uri);
            builder.Scheme = builder.Scheme.ToLowerInvariant();
            builder.Host = builder.Host.ToLowerInvariant();
            if (uri.IsDefaultPort) builder.Port = -1; // Remove default port

            string path = builder.Path.TrimEnd('/');
            if (string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(builder.Host)) path = "/";

            builder.Path = path;
            return builder.Uri.AbsoluteUri;
        }
        catch(UriFormatException ex)
        {
            _logger.LogWarning("Could not normalize URL '{OriginalUrl}': {ErrorMessage}", url, ex.Message);
            return url.Trim();
        }
    }
    
    private async Task<string> ScrapeArticleContentAsync(string url)
    {
        // Find the path to the scripts directory
        var scriptDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "scripts");
        scriptDirectory = Path.GetFullPath(scriptDirectory);
        
        // Path to the Python script
        var scriptPath = Path.Combine(scriptDirectory, "scraper.py");
        
        // Path to the Python executable in the virtual environment
        var pythonPath = Path.Combine(scriptDirectory, "venv", "Scripts", "python.exe");
        
        // Check if the virtual environment exists
        if (!File.Exists(pythonPath))
        {
            _logger.LogWarning($"Virtual environment Python executable not found at {pythonPath}, falling back to system Python");
            pythonPath = "python"; // Fall back to system Python
        }
        
        // Create a new process to run the Python script
        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\" \"{url}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = scriptDirectory,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        startInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8"; // Set the encoding to UTF-8

        using var process = new Process { StartInfo = startInfo };
        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += (sender, e) => {
            if (e.Data != null)
                output.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) => {
            if (e.Data != null)
                error.AppendLine(e.Data);
        };

        _logger.LogInformation($"Running scraper for URL: {url}");
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            _logger.LogError($"Error running scraper: {error}");
            throw new Exception($"Failed to scrape content: {error}");
        }

        var result = output.ToString();
        
        // Extract the content between the markdown content markers
        int startMarker = result.IndexOf("--- Cleaned Markdown Content ---");
        int endMarker = result.IndexOf("--- End of Content ---");
        
        if (startMarker >= 0 && endMarker > startMarker)
        {
            // Add length of the marker plus newline
            startMarker += "--- Cleaned Markdown Content ---".Length + 1;
            return result.Substring(startMarker, endMarker - startMarker).Trim();
        }

        return result.Trim();
    }
    
    private async Task<string> TryScrapeArticleContentAsync(string url)
    {
        try
        {
            return await ScrapeArticleContentAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping article content for URL {Url}", url);
            return string.Empty; // Or handle as needed
        }
    }
    
    private async Task<string> GetTitleAsync(string url)
    {
        // Find the path to the scripts directory
        var scriptDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "scripts");
        scriptDirectory = Path.GetFullPath(scriptDirectory);
        
        // Path to the Python script
        var scriptPath = Path.Combine(scriptDirectory, "scraper.py");
        
        // Path to the Python executable in the virtual environment
        var pythonPath = Path.Combine(scriptDirectory, "venv", "Scripts", "python.exe");
        
        // Check if the virtual environment exists
        if (!File.Exists(pythonPath))
        {
            _logger.LogWarning($"Virtual environment Python executable not found at {pythonPath}, falling back to system Python");
            pythonPath = "python"; // Fall back to system Python
        }
        
        // Create a new process to run the Python script
        var startInfo = new ProcessStartInfo
        {
            FileName = pythonPath,
            Arguments = $"\"{scriptPath}\" \"{url}\" --title",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = scriptDirectory,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        startInfo.EnvironmentVariables["PYTHONIOENCODING"] = "utf-8"; // Set the encoding to UTF-8

        using var process = new Process { StartInfo = startInfo };
        var output = new StringBuilder();
        var error = new StringBuilder();

        process.OutputDataReceived += (sender, e) => {
            if (e.Data != null)
                output.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) => {
            if (e.Data != null)
                error.AppendLine(e.Data);
        };

        _logger.LogInformation($"Running scraper for URL: {url}");
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            _logger.LogError($"Error running scraper: {error}");
            throw new Exception($"Failed to scrape content: {error}");
        }

        var result = output.ToString();

        int startMarker = result.IndexOf("--- Title ---");
        int endMarker = result.IndexOf("--- End of Title ---");
        
        if (startMarker >= 0 && endMarker > startMarker)
        {
            // Add length of the marker plus newline
            startMarker += "--- Title ---".Length + 1;
            return result[startMarker..endMarker].Trim();
        }

        return result.Trim();   


    }

    private async Task<string> TryGetTitleAsync(string url)
    {
        try
        {
            return await GetTitleAsync(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting title for URL {Url}", url);
            return string.Empty; // Or handle as needed
        }
    }

}
