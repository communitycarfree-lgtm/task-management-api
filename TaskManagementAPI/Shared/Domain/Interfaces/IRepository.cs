namespace TaskManagementAPI.Shared.Domain.Interfaces;

/// <summary>
/// Generic repository interface for CRUD operations on entities.
/// Provides a contract for data access operations.
/// </summary>
/// <typeparam name="T">The entity type, must inherit from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <returns>The entity if found; otherwise null.</returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all entities.
    /// </summary>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The added entity.</returns>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Soft-deletes an entity by ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}
