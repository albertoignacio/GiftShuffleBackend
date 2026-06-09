using System.ComponentModel.DataAnnotations;

namespace GiftShuffle.Application.DTOs;

public record ShuffleRequest(
    [param: Required, MinLength(2)] List<Guid> FriendIds,
    [param: Range(0, double.MaxValue)] decimal GiftAmount
);