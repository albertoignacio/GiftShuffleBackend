using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Domain.Entities;

namespace GiftShuffle.Application.Services;

public class ShuffleService : IShuffleService
{
    private readonly IFriendRepository _friendRepository;
    private readonly IShuffleHistoryRepository _historyRepository;
    private readonly IEmailService _emailService;

    public ShuffleService(
        IFriendRepository friendRepository,
        IShuffleHistoryRepository historyRepository,
        IEmailService emailService)
    {
        _friendRepository = friendRepository;
        _historyRepository = historyRepository;
        _emailService = emailService;
    }

    public async Task<ShuffleResponse> ExecuteShuffleAsync(Guid userId, ShuffleRequest request)
    {
        if (request.FriendIds.Count < 2)
            throw new InvalidOperationException("At least 2 participants are required");

        var friends = await _friendRepository.GetByUserIdAsync(userId);
        var selected = friends.Where(f => request.FriendIds.Contains(f.Id)).ToList();

        if (selected.Count != request.FriendIds.Count)
            throw new KeyNotFoundException("One or more friends not found");

        var previous = await _historyRepository.GetByUserIdAsync(userId);
        var previousPairs = previous
            .Select(h => (h.GiverFriendId, h.ReceiverFriendId))
            .ToHashSet();

        var assignment = GenerateAssignment(selected, previousPairs);

        var histories = assignment.Select(a => new ShuffleHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GiverFriendId = a.Giver.Id,
            ReceiverFriendId = a.Receiver.Id,
            ShuffleDate = DateTime.UtcNow
        }).ToList();

        await _historyRepository.AddRangeAsync(histories);

        foreach (var (giver, receiver) in assignment)
        {
            await _emailService.SendAssignmentEmailAsync(
                giver.Email, giver.Name, receiver.Name, request.GiftAmount);
        }

        return new ShuffleResponse(true, selected.Count, request.GiftAmount);
    }

    private static List<(Friend Giver, Friend Receiver)> GenerateAssignment(
        List<Friend> participants, HashSet<(Guid, Guid)> excludePairs)
    {
        var random = new Random();
        var maxAttempts = 100;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var shuffled = participants.OrderBy(_ => random.Next()).ToList();

            bool valid = true;
            for (int i = 0; i < shuffled.Count; i++)
            {
                var giver = shuffled[i];
                var receiver = shuffled[(i + 1) % shuffled.Count];

                if (giver.Id == receiver.Id || excludePairs.Contains((giver.Id, receiver.Id)))
                {
                    valid = false;
                    break;
                }
            }

            if (!valid) continue;

            var result = new List<(Friend, Friend)>();
            for (int i = 0; i < shuffled.Count; i++)
            {
                result.Add((shuffled[i], shuffled[(i + 1) % shuffled.Count]));
            }
            return result;
        }

        throw new InvalidOperationException(
            "Could not generate a valid assignment. Try adding more friends or clearing history.");
    }
}
