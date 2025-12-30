using FluentAssertions;
using Moq;
using PortfolioTracker.Core.Entities;
using PortfolioTracker.Core.Interfaces.Repositories;
using PortfolioTracker.Core.Services;

namespace PortfolioTracker.UnitTests.Services
{
    /// <summary>
    /// Unit tests for PortfolioService.
    /// Tests business logic for portfolio management including authorization.
    /// </summary>
    /// <remarks>
    /// Key Differences from UserService Tests:
    /// 1. Authorization: Every method checks userId matches portfolio owner
    /// 2. Complex Business Rules: Default portfolio, duplicate names per user
    /// 3. Multi-entity: Tests interaction between User and Portfolio
    /// </remarks>
    public class PortfolioServiceTests : TestBase
    {
        private readonly Mock<IPortfolioRepository> _mockPortfolioRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly PortfolioService _portfolioService;

        public PortfolioServiceTests()
        {
            _mockPortfolioRepository = new Mock<IPortfolioRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _portfolioService = new PortfolioService(_mockPortfolioRepository.Object, _mockUserRepository.Object, CreateMockLogger<PortfolioService>());
        }

        [Fact]
        public async Task GetUserPortfoliosAsync_WhenPortfoliosExist_ShouldReturnAllUserPortfolios()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var portfolios = new List<Portfolio>()
            {
                new Portfolio
                {
                    Id = Guid.NewGuid(),
                    Name = "Retirement",
                    UserId = userId,
                    IsDefault = true,
                    Currency = "USD"
                },
                new Portfolio
                {
                    Id = Guid.NewGuid(),
                    Name = "Trading",
                    UserId = userId,
                    IsDefault = false,
                    Currency = "USD"
                }
            };

            _mockPortfolioRepository
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(portfolios);

            // Act
            var result = (await _portfolioService.GetUserPortfoliosAsync(userId)).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Retirement");
            result.First().IsDefault.Should().BeTrue();

            _mockPortfolioRepository.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserPortfoliosAsync_WhenNoPortfolios_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockPortfolioRepository
                .Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(new List<Portfolio>());

            // Act
            var result = (await _portfolioService.GetUserPortfoliosAsync(userId)).ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockPortfolioRepository.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
        }
    }
}
