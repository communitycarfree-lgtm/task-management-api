using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for task dependency data.
/// </summary>
public class TaskDependencySeeder : ISeeder
{
    /// <summary>
    /// Seeds initial task dependencies into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    public async Task SeedAsync(DbContext context)
    {
        var tasksContext = context as TasksDbContext;
        if (tasksContext == null)
            return;

        // Check if dependencies already exist
        if (await tasksContext.TaskDependencies.AnyAsync())
            return;

        // Create sample dependencies
        var dependencies = new List<TaskDependency>
        {
            new TaskDependency
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                TaskId = Guid.Parse("11111111-1111-1111-1111-111111111114"), // "Fix critical bug" is blocked by
                BlockedByTaskId = Guid.Parse("11111111-1111-1111-1111-111111111112"), // "Implement API endpoints"
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        await tasksContext.TaskDependencies.AddRangeAsync(dependencies);
        await tasksContext.SaveChangesAsync();
    }
}
