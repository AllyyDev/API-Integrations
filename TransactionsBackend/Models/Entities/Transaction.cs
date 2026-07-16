namespace TransactionsBackend.Models.Entities;

/// <summary>
/// Represents a financial transaction belonging to a user.
/// </summary>
public class Transaction
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    /// <summary>Positive for income, negative for expenses.</summary>
    public decimal Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
