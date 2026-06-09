using System.ComponentModel.DataAnnotations;

namespace GiftShuffle.Application.DTOs;

public record LoginRequest(
    [param: Required, EmailAddress] string Email,
    [param: Required] string Password
);