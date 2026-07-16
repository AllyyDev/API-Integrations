using TransactionsBackend.Models.Dtos;

namespace TransactionsBackend.Services.Interfaces;

/// <summary>
/// Handles user authentication and JWT issuance.
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
}
