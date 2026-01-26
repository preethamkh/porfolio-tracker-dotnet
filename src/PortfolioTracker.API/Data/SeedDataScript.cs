using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Core.Entities;
using PortfolioTracker.Core.Enums;
using PortfolioTracker.Infrastructure.Data;

namespace PortfolioTracker.API.Data;

/// <summary>
/// Seed data script for development/testing.
/// Creates a test user with portfolio, securities, holdings, and transactions.
/// </summary>
public static class SeedDataScript
{
    public static async Task SeedTestDataAsync(ApplicationDbContext context)
    {
        // Check if we already have data
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("Database already has data. Skipping seed.");
            return;
        }

        // ========================================================================
        // 1. CREATE TEST USER
        // ========================================================================

        // Seed initial data
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            CreatedAt = DateTime.UtcNow,
            LastLogin = null
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Created user: {user.Email}");

        // ========================================================================
        // 2. CREATE DEFAULT PORTFOLIO
        // ========================================================================

        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "My Investment Portfolio",
            Description = "Primary investment portfolio for long-term growth",
            Currency = "USD", // Changed from AUD
            IsDefault = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Portfolios.AddAsync(portfolio);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Created portfolio: {portfolio.Name}");

        // ========================================================================
        // 3. CREATE SECURITIES (US ETFs - API Compatible)
        // ========================================================================

        var securities = new List<Security>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Symbol = "VTI",
                Name = "Vanguard Total Stock Market ETF",
                Exchange = "NYSE",
                SecurityType = "ETF",
                Currency = "USD",
                Sector = "Diversified",
                Industry = "Index Fund",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Symbol = "VOO",
                Name = "Vanguard S&P 500 ETF",
                Exchange = "NYSE",
                SecurityType = "ETF",
                Currency = "USD",
                Sector = "Diversified",
                Industry = "Index Fund",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Symbol = "SCHD",
                Name = "Schwab US Dividend Equity ETF",
                Exchange = "NYSE",
                SecurityType = "ETF",
                Currency = "USD",
                Sector = "Diversified",
                Industry = "Index Fund",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Symbol = "QQQ",
                Name = "Invesco QQQ Trust (NASDAQ 100)",
                Exchange = "NASDAQ",
                SecurityType = "ETF",
                Currency = "USD",
                Sector = "Technology",
                Industry = "Index Fund",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Symbol = "VEA",
                Name = "Vanguard FTSE Developed Markets ETF",
                Exchange = "NYSE",
                SecurityType = "ETF",
                Currency = "USD",
                Sector = "Diversified",
                Industry = "Index Fund",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Securities.AddRangeAsync(securities);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Created {securities.Count} securities");

        // ========================================================================
        // 4. CREATE HOLDINGS WITH TRANSACTIONS
        // ========================================================================

        // HOLDING 1: VTI (50 shares @ ~$250)
        var vtiHolding = new Holding
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            SecurityId = securities[0].Id, // VTI
            TotalShares = 50,
            AverageCost = 245.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var vtiTransactions = new List<Transaction>
{
            new()
            {
                Id = Guid.NewGuid(),
                HoldingId = vtiHolding.Id,
                TransactionType = TransactionType.Buy,
                Shares = 25,
                PricePerShare = 240.00m,
                TotalAmount = 6000.00m,
                Fees = 5.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-6),
                Notes = "Initial purchase",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                HoldingId = vtiHolding.Id,
                TransactionType = TransactionType.Buy,
                Shares = 25,
                PricePerShare = 251.00m,
                TotalAmount = 6275.00m,
                Fees = 5.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-3),
                Notes = "Adding to position",
                CreatedAt = DateTime.UtcNow
            }
        };

        // HOLDING 2: VOO (20 shares @ ~$500)
        var vooHolding = new Holding
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            SecurityId = securities[1].Id, // VOO
            TotalShares = 20,
            AverageCost = 495.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var vooTransactions = new List<Transaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                HoldingId = vooHolding.Id,
                TransactionType = TransactionType.Buy,
                Shares = 10,
                PricePerShare = 480.00m,
                TotalAmount = 4800.00m,
                Fees = 5.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-8),
                Notes = "Initial purchase",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                HoldingId = vooHolding.Id,
                TransactionType = TransactionType.Buy,
                Shares = 10,
                PricePerShare = 510.00m,
                TotalAmount = 5100.00m,
                Fees = 5.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-4),
                Notes = "Second purchase",
                CreatedAt = DateTime.UtcNow
            }
        };

        // HOLDING 3: SCHD (100 shares @ ~$80)
        var schdHolding = new Holding
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            SecurityId = securities[2].Id, // SCHD
            TotalShares = 100,
            AverageCost = 78.50m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var schdTransactions = new List<Transaction>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        HoldingId = schdHolding.Id,
                        TransactionType = TransactionType.Buy,
                        Shares = 100,
                        PricePerShare = 78.50m,
                        TotalAmount = 7850.00m,
                        Fees = 5.00m,
                        TransactionDate = DateTime.UtcNow.AddMonths(-5),
                        Notes = "Dividend income focus",
                        CreatedAt = DateTime.UtcNow
                    }
                };

        // HOLDING 4: QQQ (15 shares @ ~$450)
        var qqqHolding = new Holding
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            SecurityId = securities[3].Id, // QQQ
            TotalShares = 15,
            AverageCost = 445.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var qqqTransactions = new List<Transaction>
        {
            new()
            {
                Id = Guid.NewGuid(),
                HoldingId = qqqHolding.Id,
                TransactionType = TransactionType.Buy,
                Shares = 15,
                PricePerShare = 445.00m,
                TotalAmount = 6675.00m,
                Fees = 5.00m,
                TransactionDate = DateTime.UtcNow.AddMonths(-2),
                Notes = "Tech exposure",
                CreatedAt = DateTime.UtcNow
            }
        };

        // Add all holdings and transactions
        await context.Holdings.AddRangeAsync(vtiHolding, vooHolding, schdHolding, qqqHolding);
        await context.Transactions.AddRangeAsync(vtiTransactions);
        await context.Transactions.AddRangeAsync(vooTransactions);
        await context.Transactions.AddRangeAsync(schdTransactions);
        await context.Transactions.AddRangeAsync(qqqTransactions);

        // Add all holdings and transactions
        await context.Holdings.AddRangeAsync(vtiHolding, vooHolding, schdHolding, qqqHolding);
        await context.Transactions.AddRangeAsync(vtiTransactions);
        await context.Transactions.AddRangeAsync(vooTransactions);
        await context.Transactions.AddRangeAsync(schdTransactions);
        await context.Transactions.AddRangeAsync(qqqTransactions);

        await context.SaveChangesAsync();
        Console.WriteLine("✓ Created 4 holdings with transactions");

        // ========================================================================
        // 5. CREATE PRICE HISTORY (Optional - for charts later)
        // ========================================================================

        // Add some historical prices for VGS.AX (last 30 days)
        var priceHistory = new List<PriceHistory>();
        var baseDate = DateTime.UtcNow.AddDays(-30);
        var basePrice = 145.00m;

        for (int i = 0; i < 30; i++)
        {
            var date = baseDate.AddDays(i);
            // Skip weekends
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                continue;

            // Simulate price movement
            var randomChange = (decimal)(new Random().NextDouble() * 4 - 2); // -2 to +2
            basePrice += randomChange;

            priceHistory.Add(new PriceHistory
            {
                Id = Guid.NewGuid(),
                SecurityId = securities[0].Id, // VGS.AX
                Price = basePrice,
                OpenPrice = basePrice - 0.5m,
                HighPrice = basePrice + 1.0m,
                LowPrice = basePrice - 1.0m,
                ClosePrice = basePrice,
                Volume = 1000000 + new Random().Next(500000),
                PriceDate = date,
                CreatedAt = DateTime.UtcNow
            });
        }

        await context.PriceHistory.AddRangeAsync(priceHistory);
        await context.SaveChangesAsync();
        Console.WriteLine($"✓ Created {priceHistory.Count} price history records");

        // ========================================================================
        // SUMMARY
        // ========================================================================

        Console.WriteLine("\n=== SEED DATA SUMMARY ===");
        Console.WriteLine($"User: {user.Email} / password123");
        Console.WriteLine($"Portfolio: {portfolio.Name}");
        Console.WriteLine($"Securities: {securities.Count}");
        Console.WriteLine($"Holdings: 4");
        Console.WriteLine($"Transactions: {vtiTransactions.Count + vooTransactions.Count + schdTransactions.Count + qqqTransactions.Count}");
        Console.WriteLine($"\nTotal Portfolio Cost: ~A${(100 * 147.74m + 228 * 66.00m + 150 * 85.50m + 75 * 32.20m):N2}");
        Console.WriteLine("\n✓ Seed data complete!");
        Console.WriteLine("\nNOTE: This seed data does NOT include current prices.");
        Console.WriteLine("You'll need to fetch live prices from Alpha Vantage or manually set them.");
    }
}
