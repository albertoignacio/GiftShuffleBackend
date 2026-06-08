using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GiftShuffle.Infraestructure.Data;

public class ShuffleHistoryRepository : IShuffleHistoryRepository
{
    private readonly AppDbContext _context;

    public ShuffleHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ShuffleHistory>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ShuffleHistories
            .Where(h => h.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddRangeAsync(List<ShuffleHistory> histories)
    {
        _context.ShuffleHistories.AddRange(histories);
        await _context.SaveChangesAsync();
    }
}
