using Domain.DTOs.Category;
using Data.Models;

namespace Domain.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToCategoryDto(Category categoryModel) => new CategoryDto
    {
        Id = categoryModel.Id,
        Name = categoryModel.Name
    };

    public static Category ToCategoryFromCreateDto(CreateCategoryRequestDto createCategoryDto, Guid userId) => new Category(userId, createCategoryDto.Name);
    public static Category ToCategoryFromUpdateDto(Category category, UpdateCategoryDto updateCategoryDto)
    {
        category.Name = updateCategoryDto.Name;
        return category;
    }
}
