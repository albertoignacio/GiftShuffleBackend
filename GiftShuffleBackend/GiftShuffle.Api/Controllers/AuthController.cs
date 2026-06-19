using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GiftShuffle.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Tags("Auth")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [EndpointSummary("Registra un nuevo usuario")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var response = await authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    [EndpointSummary("Inicia sesion y devuelve un token JWT")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpGet("verify-email")]
    [EndpointSummary("Confirma el email del usuario")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        await authService.VerifyEmailAsync(userId, token);
        return Ok(new { message = "Email verified successfully" });
    }
}
