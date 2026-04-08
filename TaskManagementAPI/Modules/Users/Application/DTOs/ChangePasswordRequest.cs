namespace TaskManagementAPI.Modules.Users.Application.DTOs;

/// <summary>
/// Request model for changing user password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// The current password.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// The new password.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password.
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;
}
