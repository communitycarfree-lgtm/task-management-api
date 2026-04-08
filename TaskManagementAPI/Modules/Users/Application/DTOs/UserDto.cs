namespace TaskManagementAPI.Modules.Users.Application.DTOs;

/// <summary>
/// Data transfer object for user information.
/// </summary>
public class UserDto
{
    /// <summary>
    /// The user's unique identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The user's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The user's full name.
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// The user's assigned roles.
    /// </summary>
    public IEnumerable<string> Roles { get; set; } = new List<string>();

    /// <summary>
    /// The date the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
