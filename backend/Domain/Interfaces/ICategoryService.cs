using Data.Models;
using Domain.DTOs.Category;

namespace Domain.Interfaces;

public interface ICategoryService
{
    Task<Category> AddCategoryAsync(CreateCategoryRequestDto createCategoryRequestDto, Guid userId);
    Task<IEnumerable<Category>> GetCategoriesAsync(Guid userId);
    Task<Category?> GetCategoryByIdAsync(Guid categoryId, Guid userId);
    Task<IEnumerable<Article>> GetArticlesInCategoryAsync(Guid categoryId, Guid userId);
    Task<Category> UpdateCategoryAsync(Guid categoryId, Guid userId, UpdateCategoryDto categoryModel);
    Task<Category> DeleteCategoryAsync(Guid categoryId, Guid userId);
}
