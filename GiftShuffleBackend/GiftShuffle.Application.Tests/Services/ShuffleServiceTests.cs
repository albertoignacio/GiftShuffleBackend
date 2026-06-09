using FluentAssertions;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Application.Services;
using GiftShuffle.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace GiftShuffle.Application.Tests.Services;

public class ShuffleServiceTests
{
    private readonly Mock<IFriendRepository> _friendRepo;
    private readonly Mock<IShuffleHistoryRepository> _historyRepo;
    private readonly Mock<IEmailService> _emailService;
    private readonly ShuffleService _sut;

    public ShuffleServiceTests()
    {
        _friendRepo = new Mock<IFriendRepository>();
        _historyRepo = new Mock<IShuffleHistoryRepository>();
        _emailService = new Mock<IEmailService>();
        var logger = Mock.Of<ILogger<ShuffleService>>();
        _sut = new ShuffleService(_friendRepo.Object, _historyRepo.Object, _emailService.Object, logger);
    }

    [Fact]
    public async Task ExecuteShuffleAsync_WithValidParticipants_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var friends = CreateFriends(userId, 4);
        var request = new ShuffleRequest(friends.Select(f => f.Id).ToList(), 100m);

        _friendRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friends);
        _historyRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _sut.ExecuteShuffleAsync(userId, request);

        result.Shuffled.Should().BeTrue();
        result.ParticipantCount.Should().Be(4);
        result.GiftAmount.Should().Be(100m);
    }

    [Fact]
    public async Task ExecuteShuffleAsync_WithLessThan2Participants_ThrowsInvalidOperation()
    {
        var userId = Guid.NewGuid();
        var friend = CreateFriend(userId, "A");
        var request = new ShuffleRequest([friend.Id], 50m);

        _friendRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([friend]);

        var act = () => _sut.ExecuteShuffleAsync(userId, request);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("At least 2 participants are required");
    }

    [Fact]
    public async Task ExecuteShuffleAsync_WithUnknownFriendIds_ThrowsKeyNotFound()
    {
        var userId = Guid.NewGuid();
        var friends = CreateFriends(userId, 2);
        var request = new ShuffleRequest([friends[0].Id, Guid.NewGuid()], 50m);

        _friendRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friends);

        var act = () => _sut.ExecuteShuffleAsync(userId, request);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task ExecuteShuffleAsync_NoOneGivesToSelf()
    {
        var userId = Guid.NewGuid();
        var friends = CreateFriends(userId, 10);
        var request = new ShuffleRequest(friends.Select(f => f.Id).ToList(), 50m);

        _friendRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friends);
        _historyRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Capture saved histories
        List<ShuffleHistory>? saved = null;
        _historyRepo.Setup(r => r.AddRangeAsync(It.IsAny<List<ShuffleHistory>>(), It.IsAny<CancellationToken>()))
            .Callback<List<ShuffleHistory>, CancellationToken>((h, _) => saved = h);

        await _sut.ExecuteShuffleAsync(userId, request);

        saved.Should().NotBeNull();
        saved!.Should().HaveCount(10);

        // Verify no giver == receiver
        foreach (var h in saved)
        {
            h.GiverFriendId.Should().NotBe(h.ReceiverFriendId);
        }
    }

    [Fact]
    public async Task ExecuteShuffleAsync_AllParticipantsAreAssigned()
    {
        var userId = Guid.NewGuid();
        var friends = CreateFriends(userId, 6);
        var request = new ShuffleRequest(friends.Select(f => f.Id).ToList(), 75m);

        _friendRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friends);
        _historyRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        List<ShuffleHistory>? saved = null;
        _historyRepo.Setup(r => r.AddRangeAsync(It.IsAny<List<ShuffleHistory>>(), It.IsAny<CancellationToken>()))
            .Callback<List<ShuffleHistory>, CancellationToken>((h, _) => saved = h);

        await _sut.ExecuteShuffleAsync(userId, request);

        saved.Should().NotBeNull();

        // Each participant appears exactly once as giver and once as receiver
        var givers = saved!.Select(h => h.GiverFriendId);
        var receivers = saved.Select(h => h.ReceiverFriendId);

        givers.Should().BeEquivalentTo(friends.Select(f => f.Id));
        receivers.Should().BeEquivalentTo(friends.Select(f => f.Id));
    }

    [Fact]
    public async Task ExecuteShuffleAsync_WithPreviousPairs_ExcludesRepeatedPairs()
    {
        var userId = Guid.NewGuid();
        var friends = CreateFriends(userId, 3);
        var request = new ShuffleRequest(friends.Select(f => f.Id).ToList(), 50m);

        // Block A?B pair
        var history = new List<ShuffleHistory>
        {
            new() { Id = Guid.NewGuid(), UserId = userId, GiverFriendId = friends[0].Id, ReceiverFriendId = friends[1].Id, ShuffleDate = DateTime.UtcNow }
        };

        _friendRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friends);
        _historyRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(history);

        List<ShuffleHistory>? saved = null;
        _historyRepo.Setup(r => r.AddRangeAsync(It.IsAny<List<ShuffleHistory>>(), It.IsAny<CancellationToken>()))
            .Callback<List<ShuffleHistory>, CancellationToken>((h, _) => saved = h);

        await _sut.ExecuteShuffleAsync(userId, request);

        saved.Should().NotBeNull();
        // A should NOT give to B
        saved!.Any(h => h.GiverFriendId == friends[0].Id && h.ReceiverFriendId == friends[1].Id)
            .Should().BeFalse();
    }

    private static List<Friend> CreateFriends(Guid userId, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateFriend(userId, $"Friend{i}"))
            .ToList();
    }

    private static Friend CreateFriend(Guid userId, string name)
    {
        return new Friend
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            LastName = "Test",
            Email = $"{name.ToLowerInvariant()}@test.com"
        };
    }
}