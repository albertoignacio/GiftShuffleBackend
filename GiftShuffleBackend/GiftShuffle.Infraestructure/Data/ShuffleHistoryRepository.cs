using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GiftShuffle.Infraestructure.Data;

public class ShuffleHistoryRepository(AppDbContext context) : IShuffleHistoryRepository
{
    public async Task<List<ShuffleHistory>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await context.ShuffleHistories
            .Where(h => h.UserId == userId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task AddRangeAsync(List<ShuffleHistory> histories, CancellationToken ct = default)
    {
        context.ShuffleHistories.AddRange(histories);
        await context.SaveChangesAsync(ct);
    }
}