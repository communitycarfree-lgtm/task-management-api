using Microsoft.AspNetCore.Identity;
using TaskManagementAPI.Modules.Users.Application.DTOs;
using TaskManagementAPI.Modules.Users.Domain.Entities;

namespace TaskManagementAPI.Modules.Users.Application.Services;

/// <summary>
/// Service for managing user operations.
/// </summary>
public class UserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    /// <summary>
    /// Initializes a new instance of the UserService class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="roleManager">The role manager.</param>
    public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <param name="fullName">The user's full name.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>The created user, or null if creation failed.</returns>
    public async System.Threading.Tasks.Task<ApplicationUser?> CreateUserAsync(string email, string fullName, string password)
    {
        // Validate password complexity
        if (!ValidatePasswordComplexity(password))
            throw new ArgumentException("Password does not meet complexity requirements.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }

    /// <summary>
    /// Updates a user's profile.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="fullName">The new full name.</param>
    /// <returns>The updated user, or null if not found.</returns>
    public async System.Threading.Tasks.Task<ApplicationUser?> UpdateUserAsync(string userId, string fullName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return null;

        user.FullName = fullName;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="currentPassword">The current password.</param>
    /// <param name="newPassword">The new password.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public async System.Threading.Tasks.Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Validate password complexity
        if (!ValidatePasswordComplexity(newPassword))
            throw new ArgumentException("New password does not meet complexity requirements.");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="roleName">The role name.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public async System.Threading.Tasks.Task<bool> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
            return false;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The user, or null if not found.</returns>
    public async System.Threading.Tasks.Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    /// <summary>
    /// Gets a user by email.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>The user, or null if not found.</returns>
    public async System.Threading.Tasks.Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A collection of users.</returns>
    public IEnumerable<ApplicationUser> GetAllUsers()
    {
        return _userManager.Users.Where(u => !u.IsDeleted).ToList();
    }

    /// <summary>
    /// Validates password complexity requirements.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if password meets requirements; otherwise false.</returns>
    private static bool ValidatePasswordComplexity(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        bool hasUpperCase = password.Any(char.IsUpper);
        bool hasLowerCase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

        return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
    }
}
