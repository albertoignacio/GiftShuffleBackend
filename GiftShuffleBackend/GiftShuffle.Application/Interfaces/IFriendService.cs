using GiftShuffle.Application.DTOs;

namespace GiftShuffle.Application.Interfaces;

/// <summary>Friend CRUD operations scoped to the authenticated user.</summary>
public interface IFriendService
{
    Task<List<FriendResponse>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<FriendResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task<FriendResponse> CreateAsync(Guid userId, CreateFriendRequest request, CancellationToken ct = default);
    Task<FriendResponse> UpdateAsync(Guid id, Guid userId, UpdateFriendRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}