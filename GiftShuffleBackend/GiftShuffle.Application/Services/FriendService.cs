using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;

namespace GiftShuffle.Application.Services;

public class FriendService : IFriendService
{
    private readonly IFriendRepository _repository;

    public FriendService(IFriendRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<FriendResponse>> GetAllAsync(Guid userId)
    {
        var friends = await _repository.GetByUserIdAsync(userId);
        return friends.Select(f => new FriendResponse(f.Id, f.Name, f.LastName, f.Email)).ToList();
    }

    public async Task<FriendResponse?> GetByIdAsync(Guid id, Guid userId)
    {
        var friend = await _repository.GetByIdAsync(id);
        if (friend == null || friend.UserId != userId) return null;
        return new FriendResponse(friend.Id, friend.Name, friend.LastName, friend.Email);
    }

    public async Task<FriendResponse> CreateAsync(Guid userId, CreateFriendRequest request)
    {
        var friend = new Friend
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email
        };

        var created = await _repository.CreateAsync(friend);
        return new FriendResponse(created.Id, created.Name, created.LastName, created.Email);
    }

    public async Task<FriendResponse> UpdateAsync(Guid id, Guid userId, UpdateFriendRequest request)
    {
        var friend = await _repository.GetByIdAsync(id);
        if (friend == null || friend.UserId != userId)
            throw new KeyNotFoundException("Friend not found");

        friend.Name = request.Name;
        friend.LastName = request.LastName;
        friend.Email = request.Email;

        var updated = await _repository.UpdateAsync(friend);
        return new FriendResponse(updated.Id, updated.Name, updated.LastName, updated.Email);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var friend = await _repository.GetByIdAsync(id);
        if (friend == null || friend.UserId != userId)
            throw new KeyNotFoundException("Friend not found");

        await _repository.DeleteAsync(friend);
    }
}
