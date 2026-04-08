namespace TaskManagementAPI.Modules.Tasks.Application.DTOs;

/// <summary>
/// Response DTO for a paginated list of tasks.
/// </summary>
public class TaskListResponse
{
    /// <summary>
    /// The list of tasks.
    /// </summary>
    public IEnumerable<TaskDto> Data { get; set; } = new List<TaskDto>();

    /// <summary>
    /// The total count of tasks (ignoring pagination).
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
