using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data.Models;
using Data.Interfaces;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository        
    )
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
        _issuer = _configuration["JWT:Issuer"];
        _audience = _configuration["JWT:Audience"];
        _refreshTokenRepository = refreshTokenRepository;
    }
   

    public string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var expirationInMinutes = _configuration.GetValue<int>("JWT:ExpirationInMinutes", 15);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddMinutes(expirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _issuer,
            Audience = _audience
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(User user)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));  
        var expirationInDays = _configuration.GetValue<int>("JWT:RefreshTokenExpirationInDays", 7);


        var refreshToken = new RefreshToken
        {
            Token = token,
            Expires = DateTime.UtcNow.AddDays(expirationInDays),
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id,
            IsRevoked = false,
            IsUsed = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);

        return refreshToken;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JWT:Issuer"],
            ValidAudience = _configuration["JWT:Audience"],
            IssuerSigningKey = _key
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

    public bool ValidateToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JWT:Issuer"],
            ValidAudience = _configuration["JWT:Audience"],
            IssuerSigningKey = _key
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, Guid userId)
    {
        return await _refreshTokenRepository.ValidateTokenAsync(token, userId);
    }
}