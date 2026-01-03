using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PortfolioTracker.Core.Interfaces.Services;

namespace PortfolioTracker.API.Controllers;

// Temporary controller for testing Redis and Stock Data Service integration.
// For smoke tests.
// todo: DELETE after confirming everything works (or later :-D)
[ApiController]
[Route("api/[controller]")]
public class TestController(IStockDataService stockDataService, IDistributedCache distributedCache, ILogger<TestController> logger) : ControllerBase
{
    /// <summary>
    /// Test Redis connection.
    /// GET /api/test/redis
    /// </summary>
    [HttpGet("redis")]
    public async Task<IActionResult> TestRedis()
    {
        try
        {
            const string testKey = "test:connection";
            var testValue = $"Redis connected at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";

            // Write to Redis
            await distributedCache.SetStringAsync(testKey, testValue, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1440)
            });

            // Read from Redis
            var retrieved = await distributedCache.GetStringAsync(testKey);

            return Ok(new
            {
                Status = "Success",
                Written = testValue,
                Retrieved = retrieved,
                Match = testValue == retrieved,
                Message = testValue == retrieved
                    ? "Redis is working correctly!"
                    : "Redis read/write mismatch - check configuration"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Redis test failed");
            return StatusCode(500, new
            {
                Status = "Failed",
                Error = ex.Message,
                Message = "Redis connection test failed. Ensure Redis container is running."
            });
        }
    }

    /// <summary>
    /// Test Stock Data Service with caching.
    /// GET /api/test/stock-data?symbol=AAPL
    /// 
    /// First call: Cache MISS -> calls Alpha Vantage API -> stores in Redis
    /// Second call: Cache HIT -> returns from Redis (no API call)
    ///
    /// Check console logs to see "Cache HIT" vs "Cache MISS"
    /// </summary>
    [HttpGet("stock-data")]
    public async Task<IActionResult> TestStockData([FromQuery] string symbol)
    {
        try
        {
            var firstCallStartTime = DateTime.UtcNow;

            // first call - cache MISS expected (unless recently called)
            var stockQuote1 = await stockDataService.GetQuoteAsync(symbol);
            var firstCallDuration = (DateTime.UtcNow - firstCallStartTime).TotalMilliseconds;

            if (stockQuote1 == null)
            {
                return NotFound(new
                {
                    Status = "NotFound",
                    Symbol = symbol,
                    Message = "Stock data not found. Check if Alpha Vantage API key is configured correctly."
                });
            }

            await Task.Delay(1000);

            // second call - cache HIT expected
            var secondCallStartTime = DateTime.UtcNow;
            var stockQuote2 = await stockDataService.GetQuoteAsync(symbol);
            var secondCallDuration = (DateTime.UtcNow - secondCallStartTime).TotalMilliseconds;

            return Ok(new
            {
                Status = "Success",
                Symbol = symbol,
                FirstCall = new
                {
                    Data = stockQuote1,
                    DurationMs = firstCallDuration,
                    Note = "Check logs for 'Cache MISS' - this likely called the API"
                },
                SecondCall = new
                {
                    Data = stockQuote2,
                    DurationMs = secondCallDuration,
                    Note = "Check logs for 'Cache HIT' - this should be from Redis"
                },
                CacheWorking = secondCallDuration < firstCallDuration,
                SpeedImprovement = firstCallDuration > 0
                    ? $"{(firstCallDuration / secondCallDuration):F1}x faster"
                    : "N/A",
                Message = "Check console logs to confirm cache HIT/MISS behavior"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Stock data test failed for symbol {Symbol}", symbol);
            return StatusCode(500, new
            {
                Status = "Failed",
                Symbol = symbol,
                Error = ex.Message,
                Message = "Failed to fetch stock data. Check Alpha Vantage API key and Redis configuration."
            });
        }
    }

    // <summary>
    /// View all cached keys in Redis.
    /// GET /api/test/cache-keys
    /// 
    /// Note: This uses redis-cli command, so it only works if you can execute shell commands.
    /// Better to use Redis CLI directly: docker exec -it <container-name> redis-cli KEYS "PortfolioTracker:*"
    /// </summary>
    [HttpGet("cache-keys")]
    public IActionResult GetCacheInfo()
    {
        return Ok(new
        {
            Status = "Info",
            Message = "To view cached keys, use Redis CLI:",
            Commands = new[]
            {
                "docker exec -it portfolio-redis redis-cli",
                "KEYS PortfolioTracker:*",
                "GET PortfolioTracker:quote:AAPL",
                "TTL PortfolioTracker:quote:AAPL"
            },
            Note = "TTL shows remaining seconds before cache expiration"
        });
    }
}
