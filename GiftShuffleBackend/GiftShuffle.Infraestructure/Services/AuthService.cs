using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Infraestructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace GiftShuffle.Infraestructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Email already registered");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));

        var token = _jwtService.GenerateToken(new TokenUserInfo(
            user.Id, user.Name, user.LastName, user.Email!));

        return new AuthResponse(token, user.Name, user.LastName, user.Email!);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = _jwtService.GenerateToken(new TokenUserInfo(
            user.Id, user.Name, user.LastName, user.Email!));

        return new AuthResponse(token, user.Name, user.LastName, user.Email!);
    }
}
