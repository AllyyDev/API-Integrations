using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransactionsBackend.Constants;
using TransactionsBackend.Models.Dtos;
using TransactionsBackend.Services.Interfaces;

namespace TransactionsBackend.Controllers;

/// <summary>
/// CRUD API for user personal information.
/// Admin: full CRUD on all users. User: read/update own profile only.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Lists users. Admins see all; regular users see only themselves.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(IReadOnlyList<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var (userId, role) = GetCurrentUser();
        var users = await _userService.GetAllAsync(userId, role);
        return Ok(users);
    }

    /// <summary>
    /// Gets a user by id. Admins: any user. Users: own profile only.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var (userId, role) = GetCurrentUser();
        var user = await _userService.GetByIdAsync(id, userId, role);

        if (user is null)
        {
            return role == Roles.Admin || userId == id
                ? NotFound(new { message = "User not found." })
                : Forbid();
        }

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user account. Admin only.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);

        if (user is null)
        {
            return BadRequest(new { message = "Unable to create user. Check email uniqueness and role value." });
        }

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Updates personal information. Admins: any user. Users: own profile only.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.User}")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        var (userId, role) = GetCurrentUser();
        var user = await _userService.UpdateAsync(id, dto, userId, role);

        if (user is null)
        {
            return role == Roles.Admin || userId == id
                ? NotFound(new { message = "User not found or email already in use." })
                : Forbid();
        }

        return Ok(user);
    }

    /// <summary>
    /// Deletes a user and all related transactions. Admin only.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new { message = "User not found." });
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
