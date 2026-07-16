namespace TransactionsBackend.Models.Dtos;

/// <summary>User profile returned by the API (no sensitive fields).</summary>
public record UserResponseDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

/// <summary>Payload for creating a new user (admin-only).</summary>
public record CreateUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Role);

/// <summary>Payload for updating personal information.</summary>
public record UpdateUserDto(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Email);
