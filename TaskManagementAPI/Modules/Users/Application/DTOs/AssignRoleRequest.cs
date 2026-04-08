namespace TaskManagementAPI.Modules.Users.Application.DTOs;

/// <summary>
/// Request model for assigning a role to a user.
/// </summary>
public class AssignRoleRequest
{
    /// <summary>
    /// The role name to assign.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}
