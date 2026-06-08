using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GiftShuffle.Infraestructure.Data;

public class FriendRepository : IFriendRepository
{
    private readonly AppDbContext _context;

    public FriendRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Friend>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Friends
            .Where(f => f.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Friend?> GetByIdAsync(Guid id)
    {
        return await _context.Friends.FindAsync(id);
    }

    public async Task<Friend> CreateAsync(Friend friend)
    {
        _context.Friends.Add(friend);
        await _context.SaveChangesAsync();
        return friend;
    }

    public async Task<Friend> UpdateAsync(Friend friend)
    {
        _context.Friends.Update(friend);
        await _context.SaveChangesAsync();
        return friend;
    }

    public async Task DeleteAsync(Friend friend)
    {
        _context.Friends.Remove(friend);
        await _context.SaveChangesAsync();
    }
}
