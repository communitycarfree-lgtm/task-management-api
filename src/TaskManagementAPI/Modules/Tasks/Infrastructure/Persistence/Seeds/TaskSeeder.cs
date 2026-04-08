using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for initial task data.
/// </summary>
public class TaskSeeder : ISeeder
{
    /// <summary>
    /// Seeds initial tasks into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    public async Task SeedAsync(DbContext context)
    {
        var tasksContext = context as TasksDbContext;
        if (tasksContext == null)
            return;

        // Check if tasks already exist
        if (await tasksContext.Tasks.AnyAsync())
            return;

        // Create sample tasks
        var tasks = new List<WorkTask>
        {
            new WorkTask
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "Design database schema",
                Description = "Create the database schema for the task management system",
                Status = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.Completed,
                Priority = TaskPriority.High,
                AssigneeId = "user1",
                DueDate = DateTime.UtcNow.AddDays(-5),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new WorkTask
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111112"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "Implement API endpoints",
                Description = "Create REST API endpoints for task management",
                Status = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.InProgress,
                Priority = TaskPriority.High,
                AssigneeId = "user2",
                DueDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new WorkTask
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111113"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "Write unit tests",
                Description = "Write comprehensive unit tests for all modules",
                Status = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.New,
                Priority = TaskPriority.Medium,
                AssigneeId = null,
                DueDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new WorkTask
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111114"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Title = "Fix critical bug in authentication",
                Description = "Resolve the JWT token validation issue",
                Status = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.Blocked,
                Priority = TaskPriority.Critical,
                AssigneeId = "user1",
                DueDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new WorkTask
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111115"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Title = "Create user documentation",
                Description = "Write comprehensive user guide and API documentation",
                Status = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus.New,
                Priority = TaskPriority.Low,
                AssigneeId = null,
                DueDate = DateTime.UtcNow.AddDays(20),
                CreatedAt = DateTime.UtcNow
            }
        };

        await tasksContext.Tasks.AddRangeAsync(tasks);
        await tasksContext.SaveChangesAsync();
    }
}
