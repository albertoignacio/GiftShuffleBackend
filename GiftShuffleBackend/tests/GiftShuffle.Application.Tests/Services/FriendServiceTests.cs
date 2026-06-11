using FluentAssertions;
using GiftShuffle.Application.DTOs;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Application.Services;
using GiftShuffle.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace GiftShuffle.Application.Tests.Services;

public class FriendServiceTests
{
    private readonly Mock<IFriendRepository> _repository;
    private readonly FriendService _sut;
    private readonly Guid _userId;
    private readonly Friend _existingFriend;

    public FriendServiceTests()
    {
        _repository = new Mock<IFriendRepository>();
        var logger = Mock.Of<ILogger<FriendService>>();
        _sut = new FriendService(_repository.Object, logger);
        _userId = Guid.NewGuid();
        _existingFriend = new Friend
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
            Name = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFriendsForUser()
    {
        var friends = new List<Friend> { _existingFriend };
        _repository.Setup(r => r.GetByUserIdAsync(_userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(friends);

        var result = await _sut.GetAllAsync(_userId);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsFriend()
    {
        _repository.Setup(r => r.GetByIdAsync(_existingFriend.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingFriend);

        var result = await _sut.GetByIdAsync(_existingFriend.Id, _userId);

        result.Should().NotBeNull();
        result!.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithWrongUserId_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(_existingFriend.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingFriend);

        var result = await _sut.GetByIdAsync(_existingFriend.Id, Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        _repository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Friend?)null);

        var result = await _sut.GetByIdAsync(Guid.NewGuid(), _userId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsFriend()
    {
        var request = new CreateFriendRequest("Jane", "Smith", "jane@test.com");
        _repository.Setup(r => r.CreateAsync(It.IsAny<Friend>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Friend f, CancellationToken _) => f);

        var result = await _sut.CreateAsync(_userId, request);

        result.Name.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.Email.Should().Be("jane@test.com");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesFriend()
    {
        _repository.Setup(r => r.GetByIdAsync(_existingFriend.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingFriend);
        _repository.Setup(r => r.UpdateAsync(It.IsAny<Friend>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Friend f, CancellationToken _) => f);

        var request = new UpdateFriendRequest("Johnny", "Doe", "johnny@test.com");

        var result = await _sut.UpdateAsync(_existingFriend.Id, _userId, request);

        result.Name.Should().Be("Johnny");
        result.Email.Should().Be("johnny@test.com");
    }

    [Fact]
    public async Task UpdateAsync_WithWrongUser_ThrowsKeyNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(_existingFriend.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingFriend);

        var act = () => _sut.UpdateAsync(_existingFriend.Id, Guid.NewGuid(), new UpdateFriendRequest("X", "Y", "x@y.com"));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidData_DeletesFriend()
    {
        _repository.Setup(r => r.GetByIdAsync(_existingFriend.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingFriend);

        await _sut.DeleteAsync(_existingFriend.Id, _userId);

        _repository.Verify(r => r.DeleteAsync(_existingFriend, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithWrongUser_ThrowsKeyNotFound()
    {
        _repository.Setup(r => r.GetByIdAsync(_existingFriend.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_existingFriend);

        var act = () => _sut.DeleteAsync(_existingFriend.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}