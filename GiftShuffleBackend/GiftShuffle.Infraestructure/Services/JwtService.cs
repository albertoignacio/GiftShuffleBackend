using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Infraestructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace GiftShuffle.Infraestructure.Services;

public class JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger) : IJwtService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public string GenerateToken(TokenUserInfo user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("lastName", user.LastName)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpireMinutes),
            signingCredentials: credentials);

        logger.LogInformation("Generated JWT for user {UserId}", user.Id);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}