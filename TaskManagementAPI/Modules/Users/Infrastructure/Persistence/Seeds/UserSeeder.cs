using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;
using TaskManagementAPI.Modules.Users.Domain.Entities;

namespace TaskManagementAPI.Modules.Users.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for initial user data.
/// </summary>
public class UserSeeder : ISeeder
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the UserSeeder class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    public UserSeeder(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Seeds initial users into the database.
    /// </summary>
    public async System.Threading.Tasks.Task SeedAsync(Microsoft.EntityFrameworkCore.DbContext context)
    {
        var usersDbContext = context as UsersDbContext;
        if (usersDbContext == null)
            return;

        // Check if users already exist
        if (usersDbContext.Users.Any())
            return;

        var users = new[]
        {
            new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                FullName = "Admin User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                UserName = "manager@example.com",
                Email = "manager@example.com",
                FullName = "Manager User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            },
            new ApplicationUser
            {
                UserName = "developer@example.com",
                Email = "developer@example.com",
                FullName = "Developer User",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var user in users)
        {
            var result = await _userManager.CreateAsync(user, "Password123!");
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to seed user {user.Email}");
        }

        await usersDbContext.SaveChangesAsync();
    }
}
