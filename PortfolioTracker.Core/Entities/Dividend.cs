using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioTracker.Core.Entities;

/// <summary>
/// Represents dividend payments received from holdings.
/// Tracks passive income from investments.
/// </summary>
public class Dividend
{
    /// <summary>
    /// Unique identifier for the dividend record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the holding that paid the dividend.
    /// </summary>
    [Required]
    public Guid HoldingId { get; set; }

    /// <summary>
    /// Dividend amount per share.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal AmountPerShare { get; set; }

    /// <summary>
    /// Total dividend received (AmountPerShare × Shares held).
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Date the dividend was paid.
    /// </summary>
    [Required]
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// Ex-dividend date (date by which you must own shares to receive dividend).
    /// </summary>
    public DateTime? ExDividendDate { get; set; }

    /// <summary>
    /// When this dividend record was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties

    /// <summary>
    /// The holding that paid this dividend.
    /// </summary>
    [ForeignKey(nameof(HoldingId))]
    public virtual Holding Holding { get; set; } = null!;
}