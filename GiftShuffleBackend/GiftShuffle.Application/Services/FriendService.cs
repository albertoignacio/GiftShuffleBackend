using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GiftShuffle.Application.Services;

public class FriendService(IFriendRepository repository, ILogger<FriendService> logger) : IFriendService
{
    public async Task<List<FriendResponse>> GetAllAsync(Guid userId, CancellationToken ct = default)
    {
        var friends = await repository.GetByUserIdAsync(userId, ct);
        logger.LogInformation("Retrieved {Count} friends for user {UserId}", friends.Count, userId);
        return friends.Select(f => new FriendResponse(f.Id, f.Name, f.LastName, f.Email)).ToList();
    }

    public async Task<FriendResponse?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var friend = await repository.GetByIdAsync(id, ct);
        if (friend == null || friend.UserId != userId) return null;
        return new FriendResponse(friend.Id, friend.Name, friend.LastName, friend.Email);
    }

    public async Task<FriendResponse> CreateAsync(Guid userId, CreateFriendRequest request, CancellationToken ct = default)
    {
        var friend = new Friend
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email
        };

        var created = await repository.CreateAsync(friend, ct);
        logger.LogInformation("Created friend {FriendId} for user {UserId}", created.Id, userId);
        return new FriendResponse(created.Id, created.Name, created.LastName, created.Email);
    }

    public async Task<FriendResponse> UpdateAsync(Guid id, Guid userId, UpdateFriendRequest request, CancellationToken ct = default)
    {
        var friend = await repository.GetByIdAsync(id, ct);
        if (friend == null || friend.UserId != userId)
            throw new KeyNotFoundException("Friend not found");

        friend.Name = request.Name;
        friend.LastName = request.LastName;
        friend.Email = request.Email;

        var updated = await repository.UpdateAsync(friend, ct);
        logger.LogInformation("Updated friend {FriendId} for user {UserId}", id, userId);
        return new FriendResponse(updated.Id, updated.Name, updated.LastName, updated.Email);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var friend = await repository.GetByIdAsync(id, ct);
        if (friend == null || friend.UserId != userId)
            throw new KeyNotFoundException("Friend not found");

        await repository.DeleteAsync(friend, ct);
        logger.LogInformation("Deleted friend {FriendId} for user {UserId}", id, userId);
    }
}