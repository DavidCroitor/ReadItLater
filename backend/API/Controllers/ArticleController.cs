using Domain.Interfaces;
using System.Linq;
using Domain.Mappers;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs.Article;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers;

[ApiController]
[Route("api/articles")]
[Authorize]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IUserContext _userContext;

    public ArticleController(
        IArticleService articleService,
        IUserContext userContext)
    {
        _articleService = articleService;
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
    public async Task<IActionResult> AddArticle([FromBody] CreateArticleRequestDto articleModel)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");

        var article = await _articleService.AddArticleForUserAsync(articleModel, userId.Value);
        if (article == null) return NotFound();
        return CreatedAtAction(nameof(GetById), new { articleId = article.ArticleId }, article);
        
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetArticlesForCurrentUser( )
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");

        var articles = await _articleService.GetArticlesForUserAsync(userId.Value);
        return Ok(articles);
    }

    [HttpGet("{articleId:guid}")]
    public async Task<IActionResult> GetById([FromRoute]Guid articleId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");

        if(!Guid.TryParse(articleId.ToString(), out _)) 
            return BadRequest("Invalid id");

        var article = await _articleService.GetArticleForUserAsync(articleId, userId.Value);
        if (article == null) return NotFound();
        return Ok(article);
    }

    [HttpPut("{articleId:guid}")]
    public async Task<IActionResult> Update([FromRoute]Guid articleId, [FromBody] UpdateUserArticleDto articleModel)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");

        if (!Guid.TryParse(articleId.ToString(), out _))
            return BadRequest("Invalid id");

        var article = await _articleService.UpdateArticleForUserAsync(articleId, userId.Value,  articleModel);
        if (article == null) return NotFound();
        return Ok(article);
    }

    [HttpDelete("{articleId:guid}")]
    public async Task<IActionResult> Delete([FromRoute]Guid articleId)
    {
        var userId = GetUserId();
        if (!userId.HasValue) return Unauthorized("User is not authenticated.");

        if (!Guid.TryParse(articleId.ToString(), out _))
            return BadRequest("Invalid id");

        var article = await _articleService.DeleteArticleForUserAsync(articleId, userId.Value);
        if (article == null) return NotFound();
        return Ok(article);
    }

}
