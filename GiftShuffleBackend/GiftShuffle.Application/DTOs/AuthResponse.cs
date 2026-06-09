namespace GiftShuffle.Application.DTOs;

public record AuthResponse(string Token, string Name, string LastName, string Email);