using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Handles user registration and login.</summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}