using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;
using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Services;

/// <summary>
/// Repository interface for WorkTask entity operations.
/// </summary>
public interface ITaskRepository : IRepository<WorkTask>
{
    /// <summary>
    /// Gets tasks for a specific project with pagination.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the tasks and total count.</returns>
    System.Threading.Tasks.Task<(IEnumerable<WorkTask> Tasks, int TotalCount)> GetProjectTasksPagedAsync(
        Guid projectId, int pageNumber, int pageSize);

    /// <summary>
    /// Gets tasks for a specific project with optional filters.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="assigneeId">Optional assignee filter.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the filtered tasks and total count.</returns>
    System.Threading.Tasks.Task<(IEnumerable<WorkTask> Tasks, int TotalCount)> GetProjectTasksWithFiltersAsync(
        Guid projectId, TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus? status = null, TaskPriority? priority = null,
        string? assigneeId = null, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets tasks assigned to a specific user.
    /// </summary>
    /// <param name="assigneeId">The assignee ID.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the tasks and total count.</returns>
    System.Threading.Tasks.Task<(IEnumerable<WorkTask> Tasks, int TotalCount)> GetUserTasksAsync(
        string assigneeId, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets overdue tasks.
    /// </summary>
    /// <returns>A collection of overdue tasks.</returns>
    System.Threading.Tasks.Task<IEnumerable<WorkTask>> GetOverdueTasksAsync();

    /// <summary>
    /// Gets tasks due within the specified number of days.
    /// </summary>
    /// <param name="days">The number of days to look ahead.</param>
    /// <returns>A collection of tasks due soon.</returns>
    System.Threading.Tasks.Task<IEnumerable<WorkTask>> GetTasksDueSoonAsync(int days = 1);

    /// <summary>
    /// Gets tasks with their dependencies.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>The task with dependencies, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> GetTaskWithDependenciesAsync(Guid taskId);

    /// <summary>
    /// Gets all tasks blocking a specific task.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>A collection of blocking tasks.</returns>
    System.Threading.Tasks.Task<IEnumerable<WorkTask>> GetBlockingTasksAsync(Guid taskId);

    /// <summary>
    /// Checks if a task has any incomplete blocking tasks.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>True if the task has incomplete blocking tasks; otherwise false.</returns>
    System.Threading.Tasks.Task<bool> HasIncompleteBlockingTasksAsync(Guid taskId);

    /// <summary>
    /// Gets a task by its SEO-friendly slug within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="slug">The task slug.</param>
    /// <returns>The task, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> GetBySlugAsync(Guid projectId, string slug);

    /// <summary>
    /// Gets all slugs for tasks in a specific project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>A collection of task slugs.</returns>
    System.Threading.Tasks.Task<IEnumerable<string>> GetProjectTaskSlugsAsync(Guid projectId);
}
