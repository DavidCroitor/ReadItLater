using Domain.DTOs.Category;
using Domain.Interfaces;
using Domain.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace API.Controllers;


[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IUserContext _userContext;

    public CategoryController(
        ICategoryService categoryService,
        IUserContext userContext)
    {
        _categoryService = categoryService;
        _userContext = userContext;
    }

    private Guid? GetUserId()
    {
        var userId = _userContext.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        return userId.Value;
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CreateCategoryRequestDto categoryModel)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var category = await _categoryService.AddCategoryAsync(categoryModel, userId.Value);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);
        var categories = await _categoryService.GetCategoriesAsync(userId.Value);
        var categoriesDto = categories.Select(CategoryMapper.ToCategoryDto);

        return Ok(categoriesDto);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetById([FromRoute]Guid categoryId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");
        var category = await _categoryService.GetCategoryByIdAsync(categoryId, userId.Value);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpGet("{categoryId:guid}/articles")]
    public async Task<IActionResult> GetArticlesInCategory(Guid categoryId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");
        var articles = await _categoryService.GetArticlesInCategoryAsync(categoryId, userId.Value);
        var articleDtos = articles.Select(a => a.ToArticleDto());
        return Ok(articleDtos);
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);
        var category = await _categoryService.UpdateCategoryAsync(categoryId, userId.Value, updateCategoryDto);
        if (category == null) return NotFound();
        return Ok(category);
    }


    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> DeleteCategory([FromRoute]Guid categoryId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);
        var category = await _categoryService.DeleteCategoryAsync(categoryId, userId.Value);
        if (category == null) return NotFound();
        return Ok(category);
    }

}
