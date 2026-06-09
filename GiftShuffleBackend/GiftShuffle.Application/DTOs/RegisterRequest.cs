using System.ComponentModel.DataAnnotations;

namespace GiftShuffle.Application.DTOs;

public record RegisterRequest(
    [param: Required] string Name,
    [param: Required] string LastName,
    [param: Required, EmailAddress] string Email,
    [param: Required, MinLength(6)] string Password
);