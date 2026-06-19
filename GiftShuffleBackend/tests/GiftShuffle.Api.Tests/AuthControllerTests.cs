using FluentAssertions;
using System.Net;
using System.Text.Json;

namespace GiftShuffle.Api.Tests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkAndToken()
    {
        var body = new { name = "Carlos", lastName = "Garcia", email = $"carlos{Guid.NewGuid():N}@test.com", password = "Pass123!!" };

        var response = await _client.PostAsync("/api/auth/register", body.ToJson());
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var auth = await response.ReadAsAsync<AuthResponse>();
        auth.Token.Should().NotBeNullOrEmpty();
        auth.Name.Should().Be("Carlos");
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var email = $"duplicate{Guid.NewGuid():N}@test.com";
        var body = new { name = "A", lastName = "B", email, password = "Pass123!!" };

        await _client.PostAsync("/api/auth/register", body.ToJson());
        var response = await _client.PostAsync("/api/auth/register", body.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        var email = $"login{Guid.NewGuid():N}@test.com";
        await _client.PostAsync("/api/auth/register",
            new { name = "Luis", lastName = "Perez", email, password = "Pass123!!" }.ToJson());

        var response = await _client.PostAsync("/api/auth/login",
            new { email, password = "Pass123!!" }.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var auth = await response.ReadAsAsync<AuthResponse>();
        auth.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var email = $"badpass{Guid.NewGuid():N}@test.com";
        await _client.PostAsync("/api/auth/register",
            new { name = "X", lastName = "Y", email, password = "Pass123!!" }.ToJson());

        var response = await _client.PostAsync("/api/auth/login",
            new { email, password = "WrongPassword" }.ToJson());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private record AuthResponse(string Token, string Name, string LastName, string Email);
}
