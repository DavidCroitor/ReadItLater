using Data.Models;
using Domain.DTOs.Category;
using Domain.DTOs.User;
using Domain.Interfaces;
using Domain.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserContext _userContext;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    public ProfileController(
        IUserContext userContext,
        ITokenService tokenService,
        UserManager<User> userManager)
    {
        _userContext = userContext;
        _tokenService = tokenService;
        _userManager = userManager;
    }


    private Guid? GetUserId()
    {
        return _userContext.UserId;
    }


    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return NotFound("User not authenticated");
        }

        var user = await _userManager.FindByIdAsync(userId.Value.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }


        return Ok(user.ToUserDto());
    }
    
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if (userId == null)
            {
                return NotFound("User not authenticated");
            }
            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            var changeResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if(!changeResult.Succeeded)
            {
                return StatusCode(500, changeResult.Errors);
            }

            var token = _tokenService.CreateToken(user);
            return Ok(user.ToUserDto());
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Inter server error: {ex.Message}");
        }
        
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateUserProfileDto)
    {
        try
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            if(userId == null)
            {
                return NotFound("User not authenticated");
            }

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if(user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = updateUserProfileDto.Username;

            var updateResult = await _userManager.UpdateAsync(user);

            if(!updateResult.Succeeded)
                return StatusCode(500, updateResult.Errors);

            return Ok(user.ToUserDto());
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}