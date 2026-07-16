using Microsoft.EntityFrameworkCore;
using TransactionsBackend.Data;
using TransactionsBackend.Models.Dtos;
using TransactionsBackend.Security;
using TransactionsBackend.Services.Interfaces;

namespace TransactionsBackend.Services;

/// <summary>
/// Authenticates users and issues JWT tokens.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(AppDbContext context, JwtTokenGenerator tokenGenerator)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return null;
        }

        var (token, expiresAt) = _tokenGenerator.GenerateToken(user);

        return new AuthResponseDto(token, user.Id, user.Email, user.Role, expiresAt);
    }
}
