using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Core.Entities;

namespace PortfolioTracker.Infrastructure.Data
{
    /// <summary>
    /// The application's database context for managing data access to the Portfolio Tracker application.
    /// This class manages the connection to the PostgreSQL database and provides methods for querying and saving data / access to all entities.
    /// <remarks>
    /// DbContext is the primary class for interacting with the database using Entity Framework Core.
    /// Represents a session with the database and can be used to query and save instances of our entities.
    /// </remarks>
    /// </summary>

    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Constructor that accepts DbContextOptions.
        /// This allows us to configure the database connection via dependency injection.
        /// </summary>
        /// <param name="options">Configuration options for the DbContext</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }

        // DbSets for each entity in the application would go here.
        // DbSet properties - Each represents a table in the database.
        // These are used to query and save instances of the entities.

        /// <summary>
        /// Users table - stores user information.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Portfolios table - Stores investment portfolios
        /// </summary>
        public DbSet<Portfolio> Portfolios { get; set; }

        /// <summary>
        /// Holdings table - Stores individual stock/ETF positions.
        /// </summary>
        public DbSet<Holding> Holdings { get; set; }

        /// <summary>
        /// Securities table - Master data for stocks/ETFs.
        /// </summary>
        public DbSet<Security> Securities { get; set; }

        /// <summary>
        /// Transactions table - Buy/sell transaction history.
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }

        /// <summary>
        /// PriceHistory table - Historical price data cache.
        /// </summary>
        public DbSet<PriceHistory> PriceHistory { get; set; }

        /// <summary>
        /// Dividends table - Dividend payment records.
        /// </summary>
        public DbSet<Dividend> Dividends { get; set; }

        /// <summary>
        /// PortfolioSnapshots table - Daily portfolio value snapshots.
        /// </summary>
        public DbSet<PortfolioSnapshot> PortfolioSnapshots { get; set; }

        
    }
}
