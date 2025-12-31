using System.Net;
using FluentAssertions;
using PortfolioTracker.Core.DTOs.User;
using PortfolioTracker.IntegrationTests.Fixtures;
using PortfolioTracker.IntegrationTests.Helpers;

namespace PortfolioTracker.IntegrationTests.API;

public class UsersControllerTests : IntegrationTestBase
{
    public UsersControllerTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetUsers_WhenNoUsers_ReturnsEmptyList()
    {
        // Act
        var response = await Client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.ReadAsJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsers_WhenUsersExist_ReturnsUserList()
    {
        // Arrange
        // Create test users directly in the database
        await TestDataBuilder.CreateUser(Context, email: "user1@test.com");
        await TestDataBuilder.CreateUser(Context, email: "user2@test.com");
        await TestDataBuilder.CreateUser(Context, email: "user3@test.com");

        // Act
        // Point to Note:
        // The test code (direct DB access) and the API (via HTTP calls) each resolve their own DbContext from their own DI container, and if the database names differ, they do not share data.
        var response = await Client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.ReadAsJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
        users.Should().HaveCount(3);

        // Verify returned users match created users
        users.Should().Contain(u => u.Email == "user1@test.com");
        users.Should().Contain(u => u.Email == "user2@test.com");
        users.Should().Contain(u => u.Email == "user3@test.com");
    }

    [Fact]
    public async Task GetUser_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = await TestDataBuilder.CreateUser(Context, email: "preetham@test.com", fullName: "Preetham K H");

        // Act
        var response = await Client.GetAsync($"/api/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnedUser = await response.ReadAsJsonAsync<UserDto>();
        returnedUser.Should().NotBeNull();
        returnedUser.Id.Should().Be(user.Id);
        returnedUser.Email.Should().Be("preetham@test.com");
        returnedUser.FullName.Should().Be("Preetham K H");
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/users/{nonExistentUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}