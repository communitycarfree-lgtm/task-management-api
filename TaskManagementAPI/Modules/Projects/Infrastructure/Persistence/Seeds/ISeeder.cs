using Microsoft.EntityFrameworkCore;

namespace TaskManagementAPI.Modules.Projects.Infrastructure.Persistence.Seeds;

/// <summary>
/// Interface for seeding data into the database.
/// </summary>
public interface ISeeder
{
    /// <summary>
    /// Seeds data into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SeedAsync(DbContext context);
}
