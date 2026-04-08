using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Projects.Domain.Entities;
using TaskManagementAPI.Modules.Projects.Domain.Enums;

namespace TaskManagementAPI.Modules.Projects.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder for initial project member data.
/// </summary>
public class ProjectMemberSeeder : ISeeder
{
    /// <summary>
    /// Seeds initial project members into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedAsync(DbContext context)
    {
        var dbContext = context as ProjectsDbContext;
        if (dbContext == null)
            return;

        // Check if members already exist
        if (await dbContext.ProjectMembers.AnyAsync())
            return;

        var members = new[]
        {
            // Website Redesign project members
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UserId = "user-001",
                Role = ProjectMemberRole.Owner,
                JoinedAt = DateTime.UtcNow.AddDays(-30)
            },
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UserId = "user-002",
                Role = ProjectMemberRole.Manager,
                JoinedAt = DateTime.UtcNow.AddDays(-28)
            },
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UserId = "user-003",
                Role = ProjectMemberRole.Developer,
                JoinedAt = DateTime.UtcNow.AddDays(-25)
            },

            // Mobile App Development project members
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                UserId = "user-001",
                Role = ProjectMemberRole.Manager,
                JoinedAt = DateTime.UtcNow.AddDays(-60)
            },
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                UserId = "user-004",
                Role = ProjectMemberRole.Developer,
                JoinedAt = DateTime.UtcNow.AddDays(-55)
            },
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                UserId = "user-005",
                Role = ProjectMemberRole.Developer,
                JoinedAt = DateTime.UtcNow.AddDays(-50)
            },

            // API Integration project members
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000007"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                UserId = "user-002",
                Role = ProjectMemberRole.Owner,
                JoinedAt = DateTime.UtcNow.AddDays(-15)
            },
            new ProjectMember
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000008"),
                ProjectId = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                UserId = "user-003",
                Role = ProjectMemberRole.Developer,
                JoinedAt = DateTime.UtcNow.AddDays(-12)
            }
        };

        await dbContext.ProjectMembers.AddRangeAsync(members);
        await dbContext.SaveChangesAsync();
    }
}
