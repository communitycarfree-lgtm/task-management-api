namespace TaskManagementAPI.Modules.Projects.Domain.Enums;

/// <summary>
/// Represents the role of a member within a project.
/// </summary>
public enum ProjectMemberRole
{
    /// <summary>
    /// Project owner with full control.
    /// </summary>
    Owner = 0,

    /// <summary>
    /// Project manager with administrative permissions.
    /// </summary>
    Manager = 1,

    /// <summary>
    /// Developer with task creation and modification permissions.
    /// </summary>
    Developer = 2,

    /// <summary>
    /// Viewer with read-only permissions.
    /// </summary>
    Viewer = 3
}
