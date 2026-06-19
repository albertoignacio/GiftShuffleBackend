using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Infraestructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GiftShuffle.Infraestructure.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtService jwtService,
    IPasswordHasher<AppUser> passwordHasher,
    IEmailService emailService,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await userManager.FindByEmailAsync(request.Email);
        if (existing != null)
            throw new InvalidOperationException("Registration failed");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            Name = request.Name,
            LastName = request.LastName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));

        var email = user.Email ?? request.Email;
        var token = jwtService.GenerateToken(new TokenUserInfo(
            user.Id, user.Name, user.LastName, email));

        var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(confirmationToken);
        var confirmationLink = $"http://localhost:5036/api/auth/verify-email?userId={user.Id}&token={encodedToken}";
        _ = emailService.SendVerificationEmailAsync(user.Email ?? request.Email, user.Name, confirmationLink);

        logger.LogInformation("User registered: {Email}", request.Email);
        return new AuthResponse(token, user.Name, user.LastName, email);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        bool valid;
        if (user == null)
        {
            passwordHasher.VerifyHashedPassword(
                new AppUser(),
                passwordHasher.HashPassword(new AppUser(), "dummy_value"),
                request.Password);
            valid = false;
        }
        else
        {
            valid = (await signInManager.CheckPasswordSignInAsync(user, request.Password, false)).Succeeded;
        }

        if (!valid)
            throw new UnauthorizedAccessException("Invalid credentials");

        var email = user!.Email ?? request.Email;
        var token = jwtService.GenerateToken(new TokenUserInfo(
            user.Id, user.Name, user.LastName, email));

        logger.LogInformation("User logged in: {Email}", request.Email);
        return new AuthResponse(token, user.Name, user.LastName, email);
    }

    public async Task VerifyEmailAsync(Guid userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new KeyNotFoundException("User not found");

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            throw new InvalidOperationException("Email verification failed");

        logger.LogInformation("Email verified for user {UserId}", userId);
    }
}

