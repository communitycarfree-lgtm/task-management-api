using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;

namespace TaskManagementAPI.Modules.Users.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for initial user roles.
/// </summary>
public class UserRoleSeeder : ISeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the UserRoleSeeder class.
    /// </summary>
    /// <param name="roleManager">The role manager.</param>
    /// <param name="userManager">The user manager.</param>
    public UserRoleSeeder(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    /// <summary>
    /// Seeds initial roles and assigns them to users.
    /// </summary>
    public async System.Threading.Tasks.Task SeedAsync(Microsoft.EntityFrameworkCore.DbContext context)
    {
        var roles = new[] { "Admin", "Manager", "Developer", "Viewer" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Assign roles to users
        var adminUser = await _userManager.FindByEmailAsync("admin@example.com");
        if (adminUser != null && !await _userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var managerUser = await _userManager.FindByEmailAsync("manager@example.com");
        if (managerUser != null && !await _userManager.IsInRoleAsync(managerUser, "Manager"))
        {
            await _userManager.AddToRoleAsync(managerUser, "Manager");
        }

        var developerUser = await _userManager.FindByEmailAsync("developer@example.com");
        if (developerUser != null && !await _userManager.IsInRoleAsync(developerUser, "Developer"))
        {
            await _userManager.AddToRoleAsync(developerUser, "Developer");
        }

        await context.SaveChangesAsync();
    }
}
