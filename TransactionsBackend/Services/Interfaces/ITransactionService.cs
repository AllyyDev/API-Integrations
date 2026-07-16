using TransactionsBackend.Models.Dtos;

namespace TransactionsBackend.Services.Interfaces;

/// <summary>
/// Business logic for transaction CRUD operations.
/// </summary>
public interface ITransactionService
{
    Task<IReadOnlyList<TransactionResponseDto>> GetAllAsync(Guid requesterId, string requesterRole);

    Task<TransactionResponseDto?> GetByIdAsync(Guid id, Guid requesterId, string requesterRole);

    Task<TransactionResponseDto?> CreateAsync(CreateTransactionDto dto, Guid requesterId, string requesterRole);

    Task<TransactionResponseDto?> UpdateAsync(Guid id, UpdateTransactionDto dto, Guid requesterId, string requesterRole);

    Task<bool> DeleteAsync(Guid id, Guid requesterId, string requesterRole);
}
