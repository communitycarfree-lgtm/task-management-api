using Microsoft.EntityFrameworkCore;

namespace TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence.Seeds;

/// <summary>
/// Interface for database seeders.
/// </summary>
public interface ISeeder
{
    /// <summary>
    /// Seeds data into the database.
    /// </summary>
    /// <param name="context">The DbContext.</param>
    Task SeedAsync(DbContext context);
}
