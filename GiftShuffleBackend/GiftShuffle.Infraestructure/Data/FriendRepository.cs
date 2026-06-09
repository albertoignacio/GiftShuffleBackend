using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GiftShuffle.Infraestructure.Data;

public class FriendRepository(AppDbContext context) : IFriendRepository
{
    public async Task<List<Friend>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.Friends
            .Where(f => f.UserId == userId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Friend?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Friends.FindAsync([id], ct);
    }

    public async Task<Friend> CreateAsync(Friend friend, CancellationToken ct = default)
    {
        context.Friends.Add(friend);
        await context.SaveChangesAsync(ct);
        return friend;
    }

    public async Task<Friend> UpdateAsync(Friend friend, CancellationToken ct = default)
    {
        context.Friends.Update(friend);
        await context.SaveChangesAsync(ct);
        return friend;
    }

    public async Task DeleteAsync(Friend friend, CancellationToken ct = default)
    {
        context.Friends.Remove(friend);
        await context.SaveChangesAsync(ct);
    }
}