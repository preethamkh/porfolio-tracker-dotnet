using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortfolioTracker.Infrastructure.Data;

namespace PortfolioTracker.IntegrationTests.Fixtures;

/// <summary>
/// Alternative: Factory using REAL PostgreSQL via Testcontainers.
/// Use this for tests that need real database behavior.
/// </summary>
/// <remarks>
/// This version spins up a real PostgreSQL container using Docker.
/// 
/// When to use this vs in-memory:
/// - In-Memory: Fast, simple tests (~90% of tests)
/// - Testcontainers: Tests that need real PostgreSQL features:
///   Complex queries
///   Stored procedures
///   Database-specific functions
///   Transaction behavior
///   Concurrency testing
/// 
/// Requirements:
/// - Docker must be running
/// - Testcontainers NuGet package installed
/// 
/// Performance:
/// - Slower than in-memory (needs to start container)
/// - Container is reused across tests (shared fixture)
/// - Still much faster than manual testing
/// </remarks>
public class PostgresIntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // Test container that manages PostgreSQL Docker container
    private Testcontainers.PostgreSql.PostgreSqlContainer? _postgresContainer;
    private string? _connectionString;

    /// <summary>
    /// Called ONCE before any tests run.
    /// Starts the PostgreSQL container.
    /// </summary>
    public async Task InitializeAsync()
    {
        // Create PostgreSQL container configuration
        _postgresContainer = new Testcontainers.PostgreSql.PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        // Start the container (this takes a few seconds first time)
        await _postgresContainer.StartAsync();

        // Get connection string to the running container
        _connectionString = _postgresContainer.GetConnectionString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            // Remove production database
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database (REAL PostgreSQL in container!)
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_connectionString);
                // ↑ This connects to REAL PostgreSQL running in Docker
            });

            // Run migrations on test database
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply migrations (creates tables based on your migrations)
            dbContext.Database.Migrate();
            // ↑ This runs actual EF Core migrations
            //   Tests real database schema!
        });
    }

    /// <summary>
    /// Called once after all tests complete.
    /// Stops and removes the PostgreSQL container.
    /// </summary>
#pragma warning disable CS0108, CS0114
    public async Task DisposeAsync()
#pragma warning restore CS0108, CS0114
    {
        if (_postgresContainer != null)
        {
            await _postgresContainer.StopAsync();
            await _postgresContainer.DisposeAsync();
        }
    }
}

// Note about IAsyncLifetime:
// - xUnit interface for async setup/teardown
// - InitializeAsync() runs before tests
// - DisposeAsync() runs after tests
// - Ensures container is started once and shared across test class