using GiftShuffle.Domain.Entities;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Repository abstraction for ShuffleHistory persistence.</summary>
public interface IShuffleHistoryRepository
{
    Task<List<ShuffleHistory>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddRangeAsync(List<ShuffleHistory> histories, CancellationToken ct = default);
    Task DeleteByUserIdAsync(Guid userId, CancellationToken ct = default);
}
