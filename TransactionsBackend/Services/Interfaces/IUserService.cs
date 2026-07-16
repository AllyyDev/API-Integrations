using TransactionsBackend.Models.Dtos;

namespace TransactionsBackend.Services.Interfaces;

/// <summary>
/// Business logic for user personal information CRUD operations.
/// </summary>
public interface IUserService
{
    Task<IReadOnlyList<UserResponseDto>> GetAllAsync(Guid requesterId, string requesterRole);

    Task<UserResponseDto?> GetUserByIdAsync(Guid id, Guid requesterId, string requesterRole);

    Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto);

    Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto, Guid requesterId, string requesterRole);

    Task<bool> DeleteAsync(Guid id);
}
