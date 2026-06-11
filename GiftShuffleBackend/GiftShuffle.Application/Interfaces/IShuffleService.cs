using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Executes the Secret Santa shuffle algorithm.</summary>
public interface IShuffleService
{
    Task<ShuffleResponse> ExecuteShuffleAsync(Guid userId, ShuffleRequest request,
        string? currentUserName = null, string? currentUserLastName = null, string? currentUserEmail = null,
        CancellationToken ct = default);

    /// <summary>Clears all shuffle history for the user, allowing previous pairs to be reassigned.</summary>
    Task ClearHistoryAsync(Guid userId, CancellationToken ct = default);
}
