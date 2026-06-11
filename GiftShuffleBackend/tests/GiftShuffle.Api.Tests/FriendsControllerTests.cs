using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace GiftShuffle.Api.Tests;

public class FriendsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FriendsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var email = $"user{Guid.NewGuid():N}@test.com";
        await _client.PostAsync("/api/auth/register",
            new { name = "Test", lastName = "User", email, password = "Pass123" }.ToJson());

        var loginResponse = await _client.PostAsync("/api/auth/login",
            new { email, password = "Pass123" }.ToJson());
        var auth = await loginResponse.ReadAsAsync<AuthResponse>();
        return auth.Token;
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/friends/getAll");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithAuth_ReturnsEmptyList()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);

        var response = await _client.GetAsync("/api/friends/getAll");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var friends = await response.ReadAsAsync<List<FriendResponse>>();
        friends.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreated()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);

        var body = new { name = "Ana", lastName = "Lopez", email = "ana@test.com" };
        var response = await _client.PostAsync("/api/friends/create", body.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var friend = await response.ReadAsAsync<FriendResponse>();
        friend.Name.Should().Be("Ana");
    }

    [Fact]
    public async Task GetById_WithExistingFriend_ReturnsFriend()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);

        var createResponse = await _client.PostAsync("/api/friends/create",
            new { name = "Pedro", lastName = "Ramirez", email = "pedro@test.com" }.ToJson());
        var created = await createResponse.ReadAsAsync<FriendResponse>();

        var getResponse = await _client.GetAsync($"/api/friends/{created.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var friend = await getResponse.ReadAsAsync<FriendResponse>();
        friend.Email.Should().Be("pedro@test.com");
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsUpdatedFriend()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);

        var createResponse = await _client.PostAsync("/api/friends/create",
            new { name = "Old", lastName = "Name", email = "old@test.com" }.ToJson());
        var created = await createResponse.ReadAsAsync<FriendResponse>();

        var updateResponse = await _client.PutAsync($"/api/friends/{created.Id}",
            new { name = "New", lastName = "Name", email = "new@test.com" }.ToJson());

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.ReadAsAsync<FriendResponse>();
        updated.Name.Should().Be("New");
        updated.Email.Should().Be("new@test.com");
    }

    [Fact]
    public async Task Delete_WithExistingFriend_ReturnsNoContent()
    {
        var token = await RegisterAndLoginAsync();
        _client.WithToken(token);

        var createResponse = await _client.PostAsync("/api/friends/create",
            new { name = "ToDelete", lastName = "X", email = "delete@test.com" }.ToJson());
        var created = await createResponse.ReadAsAsync<FriendResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/friends/{created.Id}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it's gone
        var getResponse = await _client.GetAsync($"/api/friends/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CannotAccess_OtherUsersFriends()
    {
        // User A creates a friend
        var tokenA = await RegisterAndLoginAsync();
        _client.WithToken(tokenA);
        var createResponse = await _client.PostAsync("/api/friends/create",
            new { name = "Mine", lastName = "X", email = "mine@test.com" }.ToJson());
        var created = await createResponse.ReadAsAsync<FriendResponse>();

        // User B tries to access it
        var tokenB = await RegisterAndLoginAsync();
        _client.WithToken(tokenB);
        var getResponse = await _client.GetAsync($"/api/friends/{created.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record AuthResponse(string Token, string Name, string LastName, string Email);
    private record FriendResponse(Guid Id, string Name, string LastName, string Email);
}