namespace TaskManagementAPI.Modules.Projects.Application.DTOs;

/// <summary>
/// Response DTO for a paginated list of projects.
/// </summary>
public class ProjectListResponse
{
    /// <summary>
    /// The projects in the current page.
    /// </summary>
    public IEnumerable<ProjectDto> Data { get; set; } = new List<ProjectDto>();

    /// <summary>
    /// The total number of projects.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The current page number.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The page size.
    /// </summary>
    public int PageSize { get; set; }
}
