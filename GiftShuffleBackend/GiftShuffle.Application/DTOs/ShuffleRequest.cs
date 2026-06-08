namespace GiftShuffle.Application.DTOs;

public record ShuffleRequest(List<Guid> FriendIds, decimal GiftAmount);
