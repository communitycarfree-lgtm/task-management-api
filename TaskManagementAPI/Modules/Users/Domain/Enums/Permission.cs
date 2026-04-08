namespace TaskManagementAPI.Modules.Users.Domain.Enums;

/// <summary>
/// Enumeration of system permissions.
/// </summary>
public enum Permission
{
    /// <summary>
    /// Permission to create projects.
    /// </summary>
    CreateProject = 0,

    /// <summary>
    /// Permission to delete projects.
    /// </summary>
    DeleteProject = 1,

    /// <summary>
    /// Permission to create tasks.
    /// </summary>
    CreateTask = 2,

    /// <summary>
    /// Permission to delete tasks.
    /// </summary>
    DeleteTask = 3,

    /// <summary>
    /// Permission to manage users.
    /// </summary>
    ManageUsers = 4,

    /// <summary>
    /// Permission to view audit logs.
    /// </summary>
    ViewAuditLogs = 5,

    /// <summary>
    /// Permission to manage roles.
    /// </summary>
    ManageRoles = 6,

    /// <summary>
    /// Permission to view reports.
    /// </summary>
    ViewReports = 7
}
