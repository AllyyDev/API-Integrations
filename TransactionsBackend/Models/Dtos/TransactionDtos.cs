namespace TransactionsBackend.Models.Dtos;

/// <summary>Transaction returned by the API.</summary>
public record TransactionResponseDto(
    Guid Id,
    Guid UserId,
    string Description,
    decimal Amount,
    string Category,
    DateTime TransactionDate,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>Payload for creating a new transaction.</summary>
public record CreateTransactionDto(
    string Description,
    decimal Amount,
    string Category,
    DateTime? TransactionDate,
    Guid? UserId);

/// <summary>Payload for updating an existing transaction.</summary>
public record UpdateTransactionDto(
    string? Description,
    decimal? Amount,
    string? Category,
    DateTime? TransactionDate);
