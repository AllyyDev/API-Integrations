namespace TransactionsBackend.Models.Dtos;

/// <summary>Login credentials submitted to obtain a JWT.</summary>
public record LoginDto(string Email, string Password);

/// <summary>JWT token and basic user info returned after successful authentication.</summary>
public record AuthResponseDto(string Token, Guid UserId, string Email, string Role, DateTime ExpiresAt);
