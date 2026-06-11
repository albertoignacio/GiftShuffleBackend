using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace GiftShuffle.Api.Tests;

public class ShuffleControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ShuffleControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var email = $"shuffle{Guid.NewGuid():N}@test.com";
        await _client.PostAsync("/api/auth/register",
            new { name = "Test", lastName = "User", email, password = "Pass123" }.ToJson());

        var loginResponse = await _client.PostAsync("/api/auth/login",
            new { email, password = "Pass123" }.ToJson());
        var auth = await loginResponse.ReadAsAsync<AuthResponse>();
        return auth.Token;
    }

    private async Task<Guid> CreateFriendAsync(string token, string name)
    {
        _client.WithToken(token);
        var response = await _client.PostAsync("/api/friends/create",
            new { name, lastName = "Test", email = $"{name.ToLowerInvariant()}@test.com" }.ToJson());
        var friend = await response.ReadAsAsync<FriendResponse>();
        return friend.Id;
    }

    [Fact]
    public async Task ExecuteShuffle_WithoutAuth_ReturnsUnauthorized()
    {
        var body = new { friendIds = new[] { Guid.NewGuid() }, giftAmount = 100m };
        var response = await _client.PostAsync("/api/shuffle", body.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ExecuteShuffle_WithLessThan2Friends_ReturnsBadRequest()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);

        var id1 = await CreateFriendAsync(token, "Solo");

        var body = new { friendIds = new[] { id1 }, giftAmount = 50m };
        var response = await _client.PostAsync("/api/shuffle", body.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ExecuteShuffle_WithValidParticipants_ReturnsSuccess()
    {
        var token = await RegisterAndLoginAsync();

        var ids = new List<Guid>();
        for (int i = 1; i <= 4; i++)
        {
            ids.Add(await CreateFriendAsync(token, $"Friend{i}"));
        }

        _client.WithToken(token);
        var body = new { friendIds = ids, giftAmount = 150m };
        var response = await _client.PostAsync("/api/shuffle", body.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsAsync<ShuffleResult>();
        result.Shuffled.Should().BeTrue();
        result.ParticipantCount.Should().Be(4);
        result.GiftAmount.Should().Be(150m);
    }

    [Fact]
    public async Task ExecuteShuffle_WithIncludeCurrentUser_ReturnsIncreasedCount()
    {
        var token = await RegisterAndLoginAsync();

        var ids = new List<Guid>();
        for (int i = 1; i <= 3; i++)
        {
            ids.Add(await CreateFriendAsync(token, $"Friend{i}"));
        }

        _client.WithToken(token);
        var body = new { friendIds = ids, giftAmount = 100m, includeCurrentUser = true };
        var response = await _client.PostAsync("/api/shuffle", body.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.ReadAsAsync<ShuffleResult>();
        result.Shuffled.Should().BeTrue();
        result.ParticipantCount.Should().Be(4);
        result.GiftAmount.Should().Be(100m);
    }

    [Fact]
    public async Task ClearHistory_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.DeleteAsync("/api/shuffle/history");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ClearHistory_WithAuth_ReturnsOk()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);
        var response = await _client.DeleteAsync("/api/shuffle/history");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private record AuthResponse(string Token, string Name, string LastName, string Email);
    private record FriendResponse(Guid Id, string Name, string LastName, string Email);
    private record ShuffleResult(bool Shuffled, int ParticipantCount, decimal GiftAmount);
}
