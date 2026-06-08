namespace GiftShuffle.Application.DTOs;

public record RegisterRequest(string Name, string LastName, string Email, string Password);
