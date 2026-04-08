using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Users.Domain.Entities;
using TaskManagementAPI.Modules.Users.Infrastructure.Persistence;

namespace TaskManagementAPI.Modules.Users.Infrastructure.Services;

/// <summary>
/// Repository implementation for user operations.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The Users DbContext.</param>
    public UserRepository(UsersDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a user by ID.
    /// </summary>
    public async System.Threading.Tasks.Task<ApplicationUser?> GetByIdAsync(string userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Gets a user by email.
    /// </summary>
    public async System.Threading.Tasks.Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    public async System.Threading.Tasks.Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    /// <summary>
    /// Adds a new user.
    /// </summary>
    public async System.Threading.Tasks.Task<ApplicationUser> AddAsync(ApplicationUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    public async System.Threading.Tasks.Task<ApplicationUser> UpdateAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Deletes a user (soft delete).
    /// </summary>
    public async System.Threading.Tasks.Task<bool> DeleteAsync(string userId)
    {
        var user = await GetByIdAsync(userId);
        if (user == null)
            return false;

        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        await UpdateAsync(user);
        return true;
    }
}
