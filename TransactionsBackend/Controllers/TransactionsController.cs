using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsBackend.Constants;
using TransactionsBackend.Models.Dtos;
using TransactionsBackend.Services.Interfaces;

namespace TransactionsBackend.Controllers;

/// <summary>
/// CRUD API for financial transactions.
/// Admin: full CRUD on all transactions. User: CRUD on own transactions only.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Lists transactions. Admins see all; users see only their own.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(IReadOnlyList<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var (userId, role) = GetCurrentUser();
        var transactions = await _transactionService.GetAllAsync(userId, role);
        return Ok(transactions);
    }

    /// <summary>
    /// Gets a transaction by id. Admins: any. Users: own only.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var (userId, role) = GetCurrentUser();
        var transaction = await _transactionService.GetByIdAsync(id, userId, role);

        if (transaction is null)
        {
            return NotFound(new { message = "Transaction not found or access denied." });
        }

        return Ok(transaction);
    }

    /// <summary>
    /// Creates a transaction. Users create for themselves; admins may set UserId.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {
        var (userId, role) = GetCurrentUser();
        var transaction = await _transactionService.CreateAsync(dto, userId, role);

        if (transaction is null)
        {
            return BadRequest(new { message = "Unable to create transaction. Verify target user exists." });
        }

        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    /// <summary>
    /// Updates a transaction. Admins: any. Users: own only.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTransactionDto dto)
    {
        var (userId, role) = GetCurrentUser();
        var transaction = await _transactionService.UpdateAsync(id, dto, userId, role);

        if (transaction is null)
        {
            return NotFound(new { message = "Transaction not found or access denied." });
        }

        return Ok(transaction);
    }

    /// <summary>
    /// Deletes a transaction. Admins: any. Users: own only.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var (userId, role) = GetCurrentUser();
        var deleted = await _transactionService.DeleteAsync(id, userId, role);

        if (!deleted)
        {
            return NotFound(new { message = "Transaction not found or access denied." });
        }

        return NoContent();
    }

    private (Guid UserId, string Role) GetCurrentUser()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        return (userId, role);
    }
}
