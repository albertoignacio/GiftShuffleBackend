using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

public interface IFriendService
{
    Task<List<FriendResponse>> GetAllAsync(Guid userId);
    Task<FriendResponse?> GetByIdAsync(Guid id, Guid userId);
    Task<FriendResponse> CreateAsync(Guid userId, CreateFriendRequest request);
    Task<FriendResponse> UpdateAsync(Guid id, Guid userId, UpdateFriendRequest request);
    Task DeleteAsync(Guid id, Guid userId);
}
