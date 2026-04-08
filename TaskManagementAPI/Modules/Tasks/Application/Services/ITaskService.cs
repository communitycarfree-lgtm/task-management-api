using TaskManagementAPI.Modules.Tasks.Application.DTOs;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Application.Services;

/// <summary>
/// Interface for task management service operations.
/// Defines the contract for all task-related business logic.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description.</param>
    /// <param name="priority">The task priority.</param>
    /// <param name="dueDate">The task due date.</param>
    /// <returns>The created task.</returns>
    System.Threading.Tasks.Task<WorkTask> CreateTaskAsync(Guid projectId, string title, string? description, TaskPriority priority, DateTime? dueDate);

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <returns>The task, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> GetTaskByIdAsync(Guid id);

    /// <summary>
    /// Gets a task by its SEO-friendly slug within a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="slug">The task slug.</param>
    /// <returns>The task, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> GetTaskBySlugAsync(Guid projectId, string slug);

    /// <summary>
    /// Updates a task.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="title">The new title.</param>
    /// <param name="description">The new description.</param>
    /// <param name="priority">The new priority.</param>
    /// <param name="dueDate">The new due date.</param>
    /// <returns>The updated task, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> UpdateTaskAsync(Guid id, string title, string? description, TaskPriority? priority, DateTime? dueDate);

    /// <summary>
    /// Deletes a task (soft delete).
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <returns>True if successful; otherwise false.</returns>
    System.Threading.Tasks.Task<bool> DeleteTaskAsync(Guid id);

    /// <summary>
    /// Gets tasks for a project with optional filters and pagination.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="assigneeId">Optional assignee filter.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>A tuple containing the tasks and total count.</returns>
    System.Threading.Tasks.Task<(List<WorkTask> tasks, int totalCount)> GetProjectTasksAsync(
        Guid projectId,
        TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus? status = null,
        TaskPriority? priority = null,
        string? assigneeId = null,
        int pageNumber = 1,
        int pageSize = 20);

    /// <summary>
    /// Updates a task's status.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="status">The new status.</param>
    /// <returns>The updated task, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> UpdateTaskStatusAsync(Guid id, TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus status);

    /// <summary>
    /// Assigns a task to a user.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="assigneeId">The assignee user ID.</param>
    /// <returns>The updated task, or null if not found.</returns>
    System.Threading.Tasks.Task<WorkTask?> AssignTaskAsync(Guid id, string assigneeId);

    /// <summary>
    /// Adds a time tracking entry to a task.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="hours">The number of hours.</param>
    /// <param name="date">The date of the entry.</param>
    /// <returns>The created time tracking entry.</returns>
    System.Threading.Tasks.Task<TimeTrackingEntry> AddTimeTrackingEntryAsync(Guid taskId, string userId, decimal hours, DateTime date);

    /// <summary>
    /// Adds a dependency between two tasks.
    /// </summary>
    /// <param name="taskId">The task ID that is blocked.</param>
    /// <param name="blockedByTaskId">The task ID that blocks the first task.</param>
    /// <returns>The created dependency.</returns>
    System.Threading.Tasks.Task<TaskDependency> AddTaskDependencyAsync(Guid taskId, Guid blockedByTaskId);
}
