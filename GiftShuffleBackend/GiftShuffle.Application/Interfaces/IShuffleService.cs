using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Executes the Secret Santa shuffle algorithm.</summary>
public interface IShuffleService
{
    Task<ShuffleResponse> ExecuteShuffleAsync(Guid userId, ShuffleRequest request, CancellationToken ct = default);
}