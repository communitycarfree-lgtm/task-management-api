namespace TaskManagementAPI.Modules.Projects.Domain.Enums;

/// <summary>
/// Represents the status of a project.
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Project is currently active.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Project is archived and no longer active.
    /// </summary>
    Archived = 1,

    /// <summary>
    /// Project has been deleted (soft delete).
    /// </summary>
    Deleted = 2
}
