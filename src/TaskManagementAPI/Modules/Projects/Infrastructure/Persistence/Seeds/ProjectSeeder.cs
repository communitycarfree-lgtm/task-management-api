using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for initial project data.
/// </summary>
public class ProjectSeeder : ISeeder
{
    /// <summary>
    /// Seeds initial projects into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync(DbContext context)
    {
        var dbContext = context as ProjectsDbContext;
        if (dbContext == null)
            return;

        // Check if projects already exist
        if (await dbContext.Projects.AnyAsync())
            return;

        var projects = new[]
        {
            new Project
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Website Redesign",
                Description = "Complete redesign of the company website with modern UI/UX",
                Status = ProjectStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Project
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Mobile App Development",
                Description = "Development of iOS and Android mobile applications",
                Status = ProjectStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-60)
            },
            new Project
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "API Integration",
                Description = "Integration with third-party APIs for data synchronization",
                Status = ProjectStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        await dbContext.Projects.AddRangeAsync(projects);
        await dbContext.SaveChangesAsync();
    }
}
