using Microsoft.EntityFrameworkCore;
using TransactionsBackend.Constants;
using TransactionsBackend.Data;
using TransactionsBackend.Models.Dtos;
using TransactionsBackend.Models.Entities;
using TransactionsBackend.Services.Interfaces;

namespace TransactionsBackend.Services;

/// <summary>
/// Implements user profile CRUD with role-based access rules.
/// Admin: full access. User: read/update own profile only.
/// </summary>
public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync(Guid requesterId, string requesterRole)
    {
        // Regular users may only see their own profile.
        if (requesterRole != Roles.Admin)
        {
            var own = await _context.Users.AsNoTracking()
                .Where(u => u.Id == requesterId)
                .Select(u => new UserResponseDto(
                    u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber, u.Role, u.CreatedAt, u.UpdatedAt))
                .ToListAsync();

            return own;
        }

        return await _context.Users.AsNoTracking()
            .OrderBy(u => u.LastName)
            .Select(u => new UserResponseDto(
                u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber, u.Role, u.CreatedAt, u.UpdatedAt))
            .ToListAsync();
    }

    public async Task<UserResponseDto?> GetByIdAsync(Guid id, Guid requesterId, string requesterRole)
    {
        if (requesterRole != Roles.Admin && requesterId != id)
        {
            return null;
        }

        return await _context.Users.AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserResponseDto(
                u.Id, u.Email, u.FirstName, u.LastName, u.PhoneNumber, u.Role, u.CreatedAt, u.UpdatedAt))
            .FirstOrDefaultAsync();
    }

    /// <summary>Admin-only: create a new user account.</summary>
    public async Task<UserResponseDto?> CreateAsync(CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return null;
        }

        if (dto.Role != Roles.Admin && dto.Role != Roles.User)
        {
            return null;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto, Guid requesterId, string requesterRole)
    {
        if (requesterRole != Roles.Admin && requesterId != id)
        {
            return null;
        }

        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id))
            {
                return null;
            }

            user.Email = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
        {
            user.FirstName = dto.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(dto.LastName))
        {
            user.LastName = dto.LastName;
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            user.PhoneNumber = dto.PhoneNumber;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToDto(user);
    }

    /// <summary>Admin-only: permanently remove a user and their transactions.</summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    private static UserResponseDto MapToDto(User user) =>
        new(user.Id, user.Email, user.FirstName, user.LastName, user.PhoneNumber, user.Role, user.CreatedAt, user.UpdatedAt);
}
