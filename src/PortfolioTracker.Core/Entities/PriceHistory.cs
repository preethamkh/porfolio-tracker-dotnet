using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioTracker.Core.Entities;

/// <summary>
/// Stores historical price data for securities.
/// Cached from external APIs to minimize API calls and enable charts.
/// </summary>
public class PriceHistory
{
    /// <summary>
    /// Unique identifier for the price record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the security.
    /// </summary>
    [Required]
    public Guid SecurityId { get; set; }

    /// <summary>
    /// Closing price for the day.
    /// This is the main price used for calculations.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal Price { get; set; }

    /// <summary>
    /// Opening price for the day.
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal? OpenPrice { get; set; }

    /// <summary>
    /// Highest price during the day.
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal? HighPrice { get; set; }

    /// <summary>
    /// Lowest price during the day.
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal? LowPrice { get; set; }

    /// <summary>
    /// Closing price (same as Price, but explicit).
    /// </summary>
    [Column(TypeName = "decimal(18,4)")]
    public decimal? ClosePrice { get; set; }

    /// <summary>
    /// Trading volume for the day.
    /// </summary>
    public long? Volume { get; set; }

    /// <summary>
    /// Date this price data is for.
    /// One record per security per day.
    /// </summary>
    [Required]
    public DateTime PriceDate { get; set; }

    /// <summary>
    /// When this price data was fetched/created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties

    /// <summary>
    /// The security this price data belongs to.
    /// </summary>
    [ForeignKey(nameof(SecurityId))]
    public virtual Security Security { get; set; } = null!;
}