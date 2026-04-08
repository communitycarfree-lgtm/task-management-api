namespace TaskManagementAPI.Shared.Domain;

/// <summary>
/// Abstract base class for all domain entities.
/// Provides common properties for entity identification, audit tracking, and soft deletion.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the entity was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when the entity was last updated (UTC).
    /// Null if the entity has never been updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicates whether the entity has been soft-deleted.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Timestamp when the entity was soft-deleted (UTC).
    /// Null if the entity has not been deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
