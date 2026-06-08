using GiftShuffle.Domain.Entities;

namespace GiftShuffle.Application.Interfaces;

public interface IShuffleHistoryRepository
{
    Task<List<ShuffleHistory>> GetByUserIdAsync(Guid userId);
    Task AddRangeAsync(List<ShuffleHistory> histories);
}
