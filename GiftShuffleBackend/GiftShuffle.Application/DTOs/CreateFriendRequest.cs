using System.ComponentModel.DataAnnotations;

namespace GiftShuffle.Application.DTOs;

public record CreateFriendRequest(
    [param: Required] string Name,
    [param: Required] string LastName,
    [param: Required, EmailAddress] string Email
);