using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Domain.Specifications;

/// <summary>
/// Specifications for complex Task queries.
/// Provides reusable query logic for filtering, sorting, and pagination.
/// </summary>
public static class TaskSpecifications
{
    /// <summary>
    /// Filters tasks by project and status.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="status">The task status to filter by.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetByProjectAndStatus(
        IQueryable<WorkTask> query, Guid projectId, Enums.TaskStatus status)
    {
        return query
            .Where(t => t.ProjectId == projectId && t.Status == status)
            .OrderByDescending(t => t.CreatedAt);
    }

    /// <summary>
    /// Filters tasks by assignee.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <param name="assigneeId">The assignee ID to filter by.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetByAssignee(
        IQueryable<WorkTask> query, string assigneeId)
    {
        return query
            .Where(t => t.AssigneeId == assigneeId)
            .OrderByDescending(t => t.CreatedAt);
    }

    /// <summary>
    /// Filters tasks that are blocked by other tasks.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetBlocked(IQueryable<WorkTask> query)
    {
        return query
            .Where(t => t.Status == Enums.TaskStatus.Blocked)
            .OrderByDescending(t => t.CreatedAt);
    }

    /// <summary>
    /// Filters tasks that are overdue (due date in the past and not completed).
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetOverdue(IQueryable<WorkTask> query)
    {
        var now = DateTime.UtcNow;
        return query
            .Where(t => t.DueDate < now && t.Status != Enums.TaskStatus.Completed && t.Status != Enums.TaskStatus.Cancelled)
            .OrderBy(t => t.DueDate);
    }

    /// <summary>
    /// Filters tasks by project with optional status and priority filters.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="status">Optional task status to filter by.</param>
    /// <param name="priority">Optional task priority to filter by.</param>
    /// <param name="assigneeId">Optional assignee ID to filter by.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetByProjectWithFilters(
        IQueryable<WorkTask> query,
        Guid projectId,
        Enums.TaskStatus? status = null,
        TaskPriority? priority = null,
        string? assigneeId = null)
    {
        var result = query.Where(t => t.ProjectId == projectId);

        if (status.HasValue)
            result = result.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            result = result.Where(t => t.Priority == priority.Value);

        if (!string.IsNullOrEmpty(assigneeId))
            result = result.Where(t => t.AssigneeId == assigneeId);

        return result.OrderByDescending(t => t.CreatedAt);
    }

    /// <summary>
    /// Filters tasks due within the specified number of days (includes overdue tasks).
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <param name="days">The number of days to look ahead.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetDueSoon(IQueryable<WorkTask> query, int days = 1)
    {
        var now = DateTime.UtcNow;
        var futureDate = now.AddDays(days);

        return query
            .Where(t => t.DueDate <= futureDate && 
                        t.Status != Enums.TaskStatus.Completed && t.Status != Enums.TaskStatus.Cancelled)
            .OrderBy(t => t.DueDate);
    }

    /// <summary>
    /// Filters tasks by priority.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <param name="priority">The priority to filter by.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<WorkTask> GetByPriority(
        IQueryable<WorkTask> query, TaskPriority priority)
    {
        return query
            .Where(t => t.Priority == priority)
            .OrderByDescending(t => t.CreatedAt);
    }

    /// <summary>
    /// Applies pagination to a query.
    /// </summary>
    /// <param name="query">The base query.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The paginated query.</returns>
    public static IQueryable<WorkTask> ApplyPagination(
        IQueryable<WorkTask> query, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 20;

        if (pageSize > 100)
            pageSize = 100;

        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }
}
