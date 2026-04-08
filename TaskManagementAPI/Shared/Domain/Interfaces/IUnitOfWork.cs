namespace TaskManagementAPI.Shared.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface for managing transactions and repositories.
/// Provides a contract for coordinating multiple repositories within a transaction.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets or creates a repository for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type, must inherit from BaseEntity.</typeparam>
    /// <returns>A repository instance for the entity type.</returns>
    IRepository<T> Repository<T>() where T : BaseEntity;

    /// <summary>
    /// Saves all changes made to entities in the current transaction.
    /// </summary>
    /// <returns>The number of entities affected.</returns>
    Task<int> SaveChangesAsync();
}
