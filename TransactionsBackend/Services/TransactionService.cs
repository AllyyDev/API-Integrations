using Microsoft.EntityFrameworkCore;
using TransactionsBackend.Constants;
using TransactionsBackend.Data;
using TransactionsBackend.Models.Dtos;
using TransactionsBackend.Models.Entities;
using TransactionsBackend.Services.Interfaces;

namespace TransactionsBackend.Services;

/// <summary>
/// Implements transaction CRUD with role-based access rules.
/// Admin: manage all transactions. User: manage own transactions only.
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TransactionResponseDto>> GetAllAsync(Guid requesterId, string requesterRole)
    {
        var query = _context.Transactions.AsNoTracking();

        if (requesterRole != Roles.Admin)
        {
            query = query.Where(t => t.UserId == requesterId);
        }

        return await query
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new TransactionResponseDto(
                t.Id, t.UserId, t.Description, t.Amount, t.Category, t.TransactionDate, t.CreatedAt, t.UpdatedAt))
            .ToListAsync();
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(Guid id, Guid requesterId, string requesterRole)
    {
        var transaction = await _context.Transactions.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction is null)
        {
            return null;
        }

        if (requesterRole != Roles.Admin && transaction.UserId != requesterId)
        {
            return null;
        }

        return MapToDto(transaction);
    }

    public async Task<TransactionResponseDto?> CreateAsync(CreateTransactionDto dto, Guid requesterId, string requesterRole)
    {
        // Admins may assign transactions to any user; regular users always use their own id.
        var targetUserId = requesterRole == Roles.Admin && dto.UserId.HasValue
            ? dto.UserId.Value
            : requesterId;

        if (!await _context.Users.AnyAsync(u => u.Id == targetUserId))
        {
            return null;
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = targetUserId,
            Description = dto.Description,
            Amount = dto.Amount,
            Category = dto.Category,
            TransactionDate = dto.TransactionDate ?? DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return MapToDto(transaction);
    }

    public async Task<TransactionResponseDto?> UpdateAsync(Guid id, UpdateTransactionDto dto, Guid requesterId, string requesterRole)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction is null)
        {
            return null;
        }

        if (requesterRole != Roles.Admin && transaction.UserId != requesterId)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            transaction.Description = dto.Description;
        }

        if (dto.Amount.HasValue)
        {
            transaction.Amount = dto.Amount.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Category))
        {
            transaction.Category = dto.Category;
        }

        if (dto.TransactionDate.HasValue)
        {
            transaction.TransactionDate = dto.TransactionDate.Value;
        }

        transaction.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(transaction);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid requesterId, string requesterRole)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction is null)
        {
            return false;
        }

        if (requesterRole != Roles.Admin && transaction.UserId != requesterId)
        {
            return false;
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    private static TransactionResponseDto MapToDto(Transaction transaction) =>
        new(
            transaction.Id,
            transaction.UserId,
            transaction.Description,
            transaction.Amount,
            transaction.Category,
            transaction.TransactionDate,
            transaction.CreatedAt,
            transaction.UpdatedAt);
}
