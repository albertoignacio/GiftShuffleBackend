using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

public interface IShuffleService
{
    Task<ShuffleResponse> ExecuteShuffleAsync(Guid userId, ShuffleRequest request);
}
