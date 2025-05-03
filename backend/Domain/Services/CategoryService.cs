using Domain.Interfaces;
using Data.Models;
using Domain.DTOs.Category;
using Data.Interfaces;
using Domain.Mappers;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class CategoryService : ICategoryService
{
    private readonly IArticleRepository _articleRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IArticleRepository userArticleRepository,
        ILogger<CategoryService> logger
    )
    {
        _categoryRepository = categoryRepository;
        _articleRepository = userArticleRepository;
        _logger = logger;
    }

    public async Task<Category> AddCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var category = CategoryMapper.ToCategoryFromCreateDto(createCategoryRequestDto, userId);
        return await _categoryRepository.AddAsync(category);

    }

    public async Task<Category> DeleteCategoryAsync(Guid categoryId, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        return await _categoryRepository.DeleteAsync(categoryId, userId);
    }

    public async Task<Category?> GetCategoryByIdAsync(Guid categoryId, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        return await _categoryRepository.GetByIdAsync(categoryId, userId);
    }

    public async Task<IEnumerable<Article>> GetArticlesInCategoryAsync(Guid categoryId, Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        return await _categoryRepository.GetArticlesInCategoryForUserAsync(categoryId, userId);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(Guid userId)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        return await _categoryRepository.GetAllAsync(userId);
    }

    public async Task<Category> UpdateCategoryAsync(Guid categoryId, Guid userId, UpdateCategoryDto categoryModel)
    {
        if(userId == Guid.Empty)
        {
            _logger.LogError("User is not authenticated");
            throw new UnauthorizedAccessException("User is not authenticated");
        }

        var existingCategory = await _categoryRepository.GetByIdAsync(categoryId, userId);
        if (existingCategory == null)
            throw new Exception("Category not found");
        var category = CategoryMapper.ToCategoryFromUpdateDto(existingCategory ,categoryModel);
        return await _categoryRepository.UpdateAsync(categoryId, userId, category);
    }

}
