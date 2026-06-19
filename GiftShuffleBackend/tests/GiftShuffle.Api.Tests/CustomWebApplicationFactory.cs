using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using GiftShuffle.Application.Interfaces;
using GiftShuffle.Infraestructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GiftShuffle.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "TestKey_AtLeast32CharactersLongForHmacSha256!",
                ["Jwt:Issuer"] = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience",
                ["Jwt:ExpireMinutes"] = "60",
                ["Logging:LogLevel:Default"] = "None",
                ["Logging:LogLevel:Microsoft"] = "None",
                ["RateLimiting:PermitLimit"] = "1000"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddScoped<IEmailService, StubEmailService>();

            var appDbDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (appDbDescriptor != null) services.Remove(appDbDescriptor);

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        try { _connection?.Close(); } catch { }
        try { _connection?.Dispose(); } catch { }
        await base.DisposeAsync();
    }
}

public class StubEmailService : IEmailService
{
    public Task SendAssignmentEmailAsync(string toEmail, string toName, string receiverName, decimal giftAmount, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}

public static class TestHelpers
{
    public static StringContent ToJson(this object obj)
        => new(System.Text.Json.JsonSerializer.Serialize(obj), System.Text.Encoding.UTF8, "application/json");

    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    public static HttpClient WithToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}


