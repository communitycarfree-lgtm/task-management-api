using Microsoft.AspNetCore.Identity;

namespace TaskManagementAPI.Modules.Users.Domain.Entities;

/// <summary>
/// Represents an application user extending ASP.NET Core Identity IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// The user's full name.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// The date the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The date the user was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Whether the user is deleted (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// The date the user was deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
