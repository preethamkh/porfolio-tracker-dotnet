using Moq;
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
    }
}
