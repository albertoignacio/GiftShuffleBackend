using GiftShuffle.Domain.Entities;

namespace GiftShuffle.Application.Interfaces;

public interface IFriendRepository
{
    Task<List<Friend>> GetByUserIdAsync(Guid userId);
    Task<Friend?> GetByIdAsync(Guid id);
    Task<Friend> CreateAsync(Friend friend);
    Task<Friend> UpdateAsync(Friend friend);
    Task DeleteAsync(Friend friend);
}
