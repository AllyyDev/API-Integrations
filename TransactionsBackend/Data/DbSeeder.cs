using Microsoft.EntityFrameworkCore;
using TransactionsBackend.Constants;
using TransactionsBackend.Models.Entities;

namespace TransactionsBackend.Data;

/// <summary>
/// Seeds demo admin and user accounts plus sample transactions on startup.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Users.AnyAsync())
        {
            return;
        }

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@transactions.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName = "System",
            LastName = "Administrator",
            PhoneNumber = "+1-555-0100",
            Role = Roles.Admin
        };

        var demoUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@transactions.demo",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "+1-555-0200",
            Role = Roles.User
        };

        context.Users.AddRange(admin, demoUser);

        context.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = demoUser.Id,
                Description = "Monthly salary",
                Amount = 4500.00m,
                Category = "Income",
                TransactionDate = DateTime.UtcNow.AddDays(-15)
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = demoUser.Id,
                Description = "Grocery shopping",
                Amount = -127.45m,
                Category = "Food",
                TransactionDate = DateTime.UtcNow.AddDays(-3)
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = demoUser.Id,
                Description = "Electric bill",
                Amount = -89.99m,
                Category = "Utilities",
                TransactionDate = DateTime.UtcNow.AddDays(-1)
            });

        await context.SaveChangesAsync();
    }
}
