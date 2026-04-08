using TaskManagementAPI.Modules.Users.Domain.Entities;

namespace TaskManagementAPI.Modules.Users.Infrastructure.Services;

/// <summary>
/// Repository interface for user operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>The user, or null if not found.</returns>
    System.Threading.Tasks.Task<ApplicationUser?> GetByIdAsync(string userId);

    /// <summary>
    /// Gets a user by email.
    /// </summary>
    /// <param name="email">The user's email.</param>
    /// <returns>The user, or null if not found.</returns>
    System.Threading.Tasks.Task<ApplicationUser?> GetByEmailAsync(string email);

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A collection of users.</returns>
    System.Threading.Tasks.Task<IEnumerable<ApplicationUser>> GetAllAsync();

    /// <summary>
    /// Adds a new user.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The added user.</returns>
    System.Threading.Tasks.Task<ApplicationUser> AddAsync(ApplicationUser user);

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <returns>The updated user.</returns>
    System.Threading.Tasks.Task<ApplicationUser> UpdateAsync(ApplicationUser user);

    /// <summary>
    /// Deletes a user (soft delete).
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>True if successful; otherwise false.</returns>
    System.Threading.Tasks.Task<bool> DeleteAsync(string userId);
}
