using FluentAssertions;
using PortfolioTracker.Core.DTOs.Holding;
using PortfolioTracker.Core.Helpers;
using PortfolioTracker.IntegrationTests.Fixtures;
using PortfolioTracker.IntegrationTests.Helpers;
using System.Net;

namespace PortfolioTracker.IntegrationTests.API;

/// <summary>
/// Integration tests for HoldingsController API endpoints.
/// Tests holdings CRUD operations with authentication, authorization, and real DB operations.
/// </summary>
public class HoldingsControllerTests(IntegrationTestWebAppFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task GetPortfolioHoldings_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var portfolioId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/users/{userId}/portfolios/{portfolioId}/holdings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPortfolioHoldings_WhenUserOwnsPortfolio_ShouldReturnHoldings()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "AAPL", "Apple Inc.");
        await TestDataBuilder.CreateHolding(Context, portfolio.Id, security.Id, 10, 180.50m);

        // Act
        var response = await Client.GetAsync($"/api/users/{userId}/portfolios/{portfolio.Id}/holdings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var holdings = await response.ReadAsJsonAsync<List<HoldingDto>>();
        holdings.Should().NotBeNull();
        holdings.Should().HaveCount(1);
        holdings[0].Symbol.Should().Be("AAPL");
        holdings[0].TotalShares.Should().Be(10);
    }

    [Fact]
    public async Task GetPortfolioHoldings_WhenUserDoesNotOwnPortfolio_ShouldReturn403()
    {
        // Arrange
        var user1 = await TestDataBuilder.CreateUser(Context,"user1@example.com");

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, user1.Id, "User1 Portfolio");

        // Act - user2 tries to access user1's portfolio
        await RegisterAndAuthenticateAsync("user2@example.com", "Password123!");

        var response = await Client.GetAsync($"/api/users/{user1.Id}/portfolios/{portfolio.Id}/holdings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetHolding_WhenHoldingExists_ShouldReturnHolding()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "MSFT", "Microsoft");

        var holding = await TestDataBuilder.CreateHolding(Context, portfolio.Id, security.Id, 5, 380m);

        // Act
        var response = await Client.GetAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings/{holding.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.ReadAsJsonAsync<HoldingDto>();
        result.Should().NotBeNull();
        result.HoldingId.Should().Be(holding.Id);
        result.Symbol.Should().Be("MSFT");
    }

    [Fact]
    public async Task GetHolding_WhenHoldingNotFound_ShouldReturn404()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");

        var nonExistentHoldingId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings/{nonExistentHoldingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateHolding_WithValidData_ShouldCreate201()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "GOOGL", "Alphabet");

        var createDto = new CreateHoldingDto
        {
            SecurityId = security.Id,
            TotalShares = 15,
            AverageCost = 142.50m
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings",
            createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.ReadAsJsonAsync<HoldingDto>();
        result.Should().NotBeNull();
        result.SecurityId.Should().Be(security.Id);
        result.TotalShares.Should().Be(15);
        result.AverageCost.Should().Be(142.50m);

        // Verify it's in database
        var holding = await Context.Holdings.FindAsync(result.HoldingId);
        holding.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateHolding_WhenPortfolioNotFound_ShouldReturn400()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;


        var nonExistentPortfolio = Guid.NewGuid();
        var security = await TestDataBuilder.CreateSecurity(Context, "AAPL", "Apple");

        var createDto = new CreateHoldingDto
        {
            SecurityId = security.Id,
            TotalShares = 10,
            AverageCost = 180m
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/users/{userId}/portfolios/{nonExistentPortfolio}/holdings",
            createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);     
    }

    [Fact]
    public async Task CreateHolding_WhenSecurityNotFound_ShouldReturn400()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");

        var createDto = new CreateHoldingDto
        {
            SecurityId = Guid.NewGuid(), // Non-existent security
            TotalShares = 10,
            AverageCost = 180m
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings",
            createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateHolding_WhenHoldingAlreadyExists_ShouldReturn400()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "AAPL", "Apple");

        await TestDataBuilder.CreateHolding(Context, portfolio.Id, security.Id, 5);

        var createDto = new CreateHoldingDto
        {
            SecurityId = security.Id,
            TotalShares = 10,
            AverageCost = 180m
        };

        // Act
        var response = await Client.PostAsJsonAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings",
            createDto);

        // Assert
        // todo: also check the log message
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateHolding_WithValidData_ShouldReturn200()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "AAPL", "Apple");
        
        var holding = await TestDataBuilder.CreateHolding(Context, portfolio.Id, security.Id, 10, averageCost: 180m);  

        var updateDto = new UpdateHoldingDto
        {
            TotalShares = 15,
            AverageCost = 182m
        };

        // Act
        var response = await Client.PutAsJsonAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings/{holding.Id}",
            updateDto);

        Context.ChangeTracker.Clear();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.ReadAsJsonAsync<HoldingDto>();
        result.Should().NotBeNull();
        result.TotalShares.Should().Be(15);
        result.AverageCost.Should().Be(182m);

        // Verify database was updated
        var updatedHolding = await Context.Holdings.FindAsync(holding.Id);
        updatedHolding.Should().NotBeNull();
        updatedHolding.TotalShares.Should().Be(15);
        updatedHolding.AverageCost.Should().Be(182m);
    }

    [Fact]
    public async Task UpdateHolding_WhenHoldingNotFound_ShouldReturn404()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");

        var updateDto = new UpdateHoldingDto
        {
            TotalShares = 15,
            AverageCost = 182m
        };

        // Act
        var response = await Client.PutAsJsonAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings/{Guid.NewGuid()}",
            updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHolding_WhenHoldingExists_ShouldReturn204()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "AAPL", "Apple");

        var holding = await TestDataBuilder.CreateHolding(Context, portfolio.Id, security.Id, 10);

        // Act
        var response = await Client.DeleteAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings/{holding.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);


        // Verify it's deleted from database
        Context.ChangeTracker.Clear();
        var deletedHolding = await Context.Holdings.FindAsync(holding.Id);
        deletedHolding.Should().BeNull();
    }

    [Fact]
    public async Task DeleteHolding_WhenHoldingNotFound_ShouldReturn404()
    {
        // Arrange
        var authResponse = await RegisterAndAuthenticateAsync("preetham@test.com", "Password123!");
        var userId = authResponse.User.Id;

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, userId, "Test Portfolio");

        // Act
        var response = await Client.DeleteAsync(
            $"/api/users/{userId}/portfolios/{portfolio.Id}/holdings/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteHolding_WhenUserDoesNotOwnPortfolio_ShouldReturn403()
    {
        // Arrange
        var user1 = await TestDataBuilder.CreateUser(Context, "user1@example.com");

        var portfolio = await TestDataBuilder.CreatePortfolio(Context, user1.Id, "User1 Portfolio");
        var security = await TestDataBuilder.CreateSecurity(Context, "AAPL", "Apple");

        var holding = await TestDataBuilder.CreateHolding(Context, portfolio.Id, security.Id, 10);

        // Act - user2 tries to delete user1's holding
        await RegisterAndAuthenticateAsync("user2@example.com", "Password123!");
        var response = await Client.DeleteAsync(
            $"/api/users/{user1.Id}/portfolios/{portfolio.Id}/holdings/{holding.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
