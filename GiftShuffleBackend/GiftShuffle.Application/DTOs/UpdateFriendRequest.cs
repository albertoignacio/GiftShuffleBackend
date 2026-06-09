using System.ComponentModel.DataAnnotations;

namespace GiftShuffle.Application.DTOs;

public record UpdateFriendRequest(
    [param: Required] string Name,
    [param: Required] string LastName,
    [param: Required, EmailAddress] string Email
);