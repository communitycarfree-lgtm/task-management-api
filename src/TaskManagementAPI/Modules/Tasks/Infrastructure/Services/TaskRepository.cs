using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;
using TaskManagementAPI.Modules.Tasks.Domain.Specifications;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence;
using TaskManagementAPI.Shared.Infrastructure.Repositories;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Services;

/// <summary>
/// Repository implementation for WorkTask entity operations.
/// </summary>
public class TaskRepository : GenericRepository<WorkTask>, ITaskRepository
{
    private new readonly TasksDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TaskRepository class.
    /// </summary>
    /// <param name="context">The Tasks DbContext.</param>
    public TaskRepository(TasksDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets tasks for a specific project with pagination.
    /// </summary>
    public async System.Threading.Tasks.Task<(IEnumerable<WorkTask> Tasks, int TotalCount)> GetProjectTasksPagedAsync(
        Guid projectId, int pageNumber, int pageSize)
    {
        var query = _context.Tasks.Where(t => t.ProjectId == projectId);
        var totalCount = await query.CountAsync();

        var tasks = await TaskSpecifications.ApplyPagination(query, pageNumber, pageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }

    /// <summary>
    /// Gets tasks for a specific project with optional filters.
    /// </summary>
    public async System.Threading.Tasks.Task<(IEnumerable<WorkTask> Tasks, int TotalCount)> GetProjectTasksWithFiltersAsync(
        Guid projectId, TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus? status = null, TaskPriority? priority = null,
        string? assigneeId = null, int pageNumber = 1, int pageSize = 20)
    {
        var query = TaskSpecifications.GetByProjectWithFilters(
            _context.Tasks, projectId, status, priority, assigneeId);

        var totalCount = await query.CountAsync();

        var tasks = await TaskSpecifications.ApplyPagination(query, pageNumber, pageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }

    /// <summary>
    /// Gets tasks assigned to a specific user.
    /// </summary>
    public async System.Threading.Tasks.Task<(IEnumerable<WorkTask> Tasks, int TotalCount)> GetUserTasksAsync(
        string assigneeId, int pageNumber = 1, int pageSize = 20)
    {
        var query = TaskSpecifications.GetByAssignee(_context.Tasks, assigneeId);
        var totalCount = await query.CountAsync();

        var tasks = await TaskSpecifications.ApplyPagination(query, pageNumber, pageSize)
            .ToListAsync();

        return (tasks, totalCount);
    }

    /// <summary>
    /// Gets overdue tasks.
    /// </summary>
    public async System.Threading.Tasks.Task<IEnumerable<WorkTask>> GetOverdueTasksAsync()
    {
        return await TaskSpecifications.GetOverdue(_context.Tasks)
            .ToListAsync();
    }

    /// <summary>
    /// Gets tasks due within the specified number of days.
    /// </summary>
    public async System.Threading.Tasks.Task<IEnumerable<WorkTask>> GetTasksDueSoonAsync(int days = 1)
    {
        return await TaskSpecifications.GetDueSoon(_context.Tasks, days)
            .ToListAsync();
    }

    /// <summary>
    /// Gets tasks with their dependencies.
    /// </summary>
    public async System.Threading.Tasks.Task<WorkTask?> GetTaskWithDependenciesAsync(Guid taskId)
    {
        return await _context.Tasks
            .Include(t => t.BlockedByDependencies)
            .Include(t => t.BlockingDependencies)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    /// <summary>
    /// Gets all tasks blocking a specific task.
    /// </summary>
    public async System.Threading.Tasks.Task<IEnumerable<WorkTask>> GetBlockingTasksAsync(Guid taskId)
    {
        return await _context.TaskDependencies
            .Where(d => d.TaskId == taskId)
            .Select(d => d.BlockedByTask!)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a task has any incomplete blocking tasks.
    /// </summary>
    public async System.Threading.Tasks.Task<bool> HasIncompleteBlockingTasksAsync(Guid taskId)
    {
        return await _context.TaskDependencies
            .Where(d => d.TaskId == taskId)
            .AnyAsync(d => d.BlockedByTask != null && 
                          d.BlockedByTask.Status != TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.Completed &&
                          d.BlockedByTask.Status != TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.Cancelled);
    }
}
