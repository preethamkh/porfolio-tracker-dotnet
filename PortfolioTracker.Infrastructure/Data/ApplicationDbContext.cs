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


        /// <summary>
        /// OnModelCreating is called when the model is being created.
        /// This is where we configure entity relationships, indexes, constraints, etc.
        /// This is the "Fluent API" approach (alternative to data annotations).
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model</param>
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Apply all configurations from separate configuration classes
            // This keeps the DbContext clean and configurations organized
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Configure table names (optional - EF uses entity names by default)
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Portfolio>().ToTable("portfolios");
            modelBuilder.Entity<Holding>().ToTable("holdings");
            modelBuilder.Entity<Security>().ToTable("securities");
            modelBuilder.Entity<Transaction>().ToTable("transactions");
            modelBuilder.Entity<PriceHistory>().ToTable("priceHistory");
            modelBuilder.Entity<Dividend>().ToTable("dividends");
            modelBuilder.Entity<PortfolioSnapshot>().ToTable("portfolio_snapshots");

            // Configure indexes for better query performance
            ConfigureIndexes(modelBuilder);

            // Configure unique constraints
            ConfigureUniqueConstraints(modelBuilder);

            // Configure relationships between entities
            ConfigureRelationships(modelBuilder);

            // Configure default values and computed columns
            ConfigureDefaults(modelBuilder);
        }

        private void ConfigureDefaults(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void ConfigureUniqueConstraints(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
