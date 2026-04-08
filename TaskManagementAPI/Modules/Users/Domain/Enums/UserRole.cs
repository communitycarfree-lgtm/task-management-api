namespace TaskManagementAPI.Modules.Users.Domain.Enums;

/// <summary>
/// Enumeration of user roles in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrator with full system access.
    /// </summary>
    Admin = 0,

    /// <summary>
    /// Manager with project and team management capabilities.
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Developer with task execution capabilities.
    /// </summary>
    Developer = 2,

    /// <summary>
    /// Viewer with read-only access.
    /// </summary>
    Viewer = 3
}
