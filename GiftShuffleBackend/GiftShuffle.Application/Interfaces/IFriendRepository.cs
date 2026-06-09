using GiftShuffle.Domain.Entities;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Repository abstraction for Friend persistence.</summary>
public interface IFriendRepository
{
    Task<List<Friend>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Friend?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Friend> CreateAsync(Friend friend, CancellationToken ct = default);
    Task<Friend> UpdateAsync(Friend friend, CancellationToken ct = default);
    Task DeleteAsync(Friend friend, CancellationToken ct = default);
}