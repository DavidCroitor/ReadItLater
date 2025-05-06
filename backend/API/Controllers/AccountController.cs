using System.Security.Claims;
using Data.Models;
using Domain.DTOs.Category;
using Domain.DTOs.User;
using Domain.Interfaces;
using Domain.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailService _emailService;

    public AccountController(
        UserManager<User> userManager,
        ITokenService tokenService,
        SignInManager<User> signInManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User
            (
                registerUserDto.Username,
                registerUserDto.Email
            );

            var createdUser = await _userManager.CreateAsync(user, registerUserDto.Password);
            if(createdUser.Succeeded)
            {
                System.Console.WriteLine("User created successfully");
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if(roleResult.Succeeded)
                {
                    var token = _tokenService.CreateToken(user);
                    Console.WriteLine(token);
                    return Ok(
                        new NewUserDto
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            Token = token
                        }
                    );
                }
                else
                {
                    Console.WriteLine("Failed to add user to role");
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                foreach (var error in createdUser.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return StatusCode(500, createdUser.Errors);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginRequestDto)
    {
        try
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginRequestDto.LogInIdentifier || u.Email == loginRequestDto.LogInIdentifier);

            if (user == null)
                return Unauthorized("Invalid username or email!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, false);

            if(!result.Succeeded)
                return Unauthorized("User not found or password is incorrect");

            var accessToken = _tokenService.CreateToken(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var accessTokenExpiration = jwtToken.ValidTo;

            return Ok(
                new AuthenticationResultDto
                {
                    UserDto = user.ToUserDto(),
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    AccessTokenExpiration = accessTokenExpiration
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if(user == null)
                return Ok("If your email exists in our system, a password reset link has been sent.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action(
                action: "ResetPassword",
                controller: "Account",
                values: new { token },
                protocol: Request.Scheme);

            await _emailService.SendPasswordResetEmailAsync(user.Email, callbackUrl);

            return Ok("If your email exists in our system, a password reset link has been sent.");
        }
        catch(Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("reset-password")]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            return BadRequest("Invalid password reset token");


        var redirectUrl = $"https://localhost:3000/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
        return Redirect(redirectUrl);

    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordConfirmAsync([FromBody] ResetPasswordDto resetPasswordDto)
    {
            try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
                
            // Find the user
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid request");
                
            // Reset the password using the token
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            
            if (result.Succeeded)
                return Ok("Password has been reset successfully");
            else
                return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RequestRefreshToken requestRefreshToken)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var principal = _tokenService.GetPrincipalFromExpiredToken(requestRefreshToken.AccessToken);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)));

            if (user == null)
                return Unauthorized("Invalid token");

            if(!await _tokenService.ValidateRefreshTokenAsync(requestRefreshToken.RefreshToken, user.Id))
                return Unauthorized("Invalid refresh token");

            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);
            var newAccessToken = _tokenService.CreateToken(user);

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(newAccessToken);
            var accessTokenExpiration = jwtToken.ValidTo;



            return Ok(new AuthenticationResultDto
            {
                UserDto = user.ToUserDto(),
                AccessToken = newAccessToken,
                RefreshToken = refreshToken.Token,
                AccessTokenExpiration = accessTokenExpiration
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }

}