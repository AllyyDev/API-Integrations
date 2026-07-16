using TransactionsBackend.Models.Dtos;

namespace TransactionsBackend.Services.Interfaces;

/// <summary>
/// Business logic for transaction CRUD operations.
/// </summary>
public interface ITransactionService
{
    Task<IReadOnlyList<TransactionResponseDto>> GetAllTransactionsAsync(Guid requesterId, string requesterRole);

    Task<TransactionResponseDto?> GetTransactionByIdAsync(Guid id, Guid requesterId, string requesterRole);

    Task<TransactionResponseDto?> AddTransactionAsync(CreateTransactionDto dto, Guid requesterId, string requesterRole);

    Task<TransactionResponseDto?> UpdateTransactionAsync(Guid id, UpdateTransactionDto dto, Guid requesterId, string requesterRole);

    Task<bool> DeleteTransactionAsync(Guid id, Guid requesterId, string requesterRole);
}
