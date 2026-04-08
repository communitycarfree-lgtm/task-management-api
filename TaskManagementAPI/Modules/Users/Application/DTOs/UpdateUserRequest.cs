namespace TaskManagementAPI.Modules.Users.Application.DTOs;

/// <summary>
/// Request model for updating user information.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// The user's full name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
}
