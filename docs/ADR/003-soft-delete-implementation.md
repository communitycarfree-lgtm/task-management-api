# ADR-003: Soft Delete Implementation

**Date**: April 8, 2026  
**Status**: Accepted  
**Context**: Data retention and compliance requirements

## Problem

We need to:
- Comply with data retention policies
- Allow data recovery
- Maintain audit trails
- Prevent accidental data loss

## Decision

Implement **Soft Delete** pattern where deleted records are marked inactive but retained in the database.

### Implementation

```csharp
// BaseEntity with soft delete properties
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

// Soft delete in repository
public virtual async Task DeleteAsync(Guid id)
{
    var entity = await _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
    if (entity != null)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
    }
}

// Query filter in BaseDbContext
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var entityTypes = modelBuilder.Model.GetEntityTypes()
        .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType));
    
    foreach (var entityType in entityTypes)
    {
        var parameter = Expression.Parameter(entityType.ClrType, "e");
        var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
        var notDeleted = Expression.Not(isDeletedProperty);
        var lambda = Expression.Lambda(notDeleted, parameter);
        
        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
    }
}
```

## Rationale

### Advantages

1. **Data Recovery**
   - Deleted data can be restored
   - Accidental deletions can be undone
   - Historical data preserved

2. **Audit Trail**
   - Track when data was deleted
   - Know who deleted the data
   - Maintain compliance records

3. **Referential Integrity**
   - Prevent orphaned foreign keys
   - Maintain data consistency
   - Easier to handle cascading deletes

4. **Performance**
   - No need to physically delete records
   - Faster delete operations
   - Easier to implement undo functionality

5. **Compliance**
   - Meet data retention requirements
   - Support regulatory audits
   - Maintain historical records

### Disadvantages

1. **Database Size**
   - Database grows over time
   - Requires archival strategy
   - Storage costs increase

2. **Query Complexity**
   - All queries must filter deleted records
   - Requires query filters
   - Potential for bugs if filter forgotten

3. **Performance**
   - Additional WHERE clause on all queries
   - May impact query performance
   - Requires proper indexing

## Implementation Details

### Automatic Query Filtering

```csharp
// Automatically excludes soft-deleted entities
var projects = await _context.Projects.ToListAsync();
// WHERE IsDeleted = 0 is automatically applied

// To include soft-deleted entities
var allProjects = await _context.Projects
    .IgnoreQueryFilters()
    .ToListAsync();
```

### Restoration

```csharp
public async Task RestoreAsync(Guid id)
{
    var entity = await _context.Set<T>()
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(e => e.Id == id);
    
    if (entity != null)
    {
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
```

### Hard Delete (After Retention Period)

```csharp
public async Task PermanentlyDeleteOldRecordsAsync(int retentionDays = 90)
{
    var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
    
    var oldRecords = await _context.Set<T>()
        .IgnoreQueryFilters()
        .Where(e => e.IsDeleted && e.DeletedAt < cutoffDate)
        .ToListAsync();
    
    _context.Set<T>().RemoveRange(oldRecords);
    await _context.SaveChangesAsync();
}
```

### Indexing Strategy

```csharp
// Add index for soft delete filter
modelBuilder.Entity<Project>()
    .HasIndex(p => p.IsDeleted)
    .HasName("IX_Projects_IsDeleted");

// Composite index for common queries
modelBuilder.Entity<Project>()
    .HasIndex(p => new { p.IsDeleted, p.CreatedAt })
    .HasName("IX_Projects_IsDeleted_CreatedAt");
```

## Consequences

### Positive

- ✅ Data recovery capability
- ✅ Audit trail maintained
- ✅ Compliance support
- ✅ Referential integrity
- ✅ Undo functionality

### Negative

- ⚠️ Database size grows
- ⚠️ Query complexity increases
- ⚠️ Potential performance impact
- ⚠️ Requires discipline to use correctly

## Alternatives Considered

### 1. Hard Delete
- **Pros**: Simple, smaller database
- **Cons**: No recovery, no audit trail
- **Decision**: Doesn't meet compliance requirements

### 2. Archive Tables
- **Pros**: Separate storage, cleaner queries
- **Cons**: Complex, manual management
- **Decision**: Soft delete simpler to implement

### 3. Event Sourcing
- **Pros**: Complete audit trail, time travel
- **Cons**: Complex, overkill for requirements
- **Decision**: Soft delete sufficient

## Maintenance Strategy

1. **Monitoring**
   - Track soft-deleted record count
   - Monitor database size growth
   - Alert on unusual deletion patterns

2. **Archival**
   - Archive old soft-deleted records
   - Move to separate storage
   - Implement retention policies

3. **Cleanup**
   - Permanently delete after retention period
   - Scheduled job for cleanup
   - Configurable retention days

## Related Decisions

- [ADR-001: Modular Monolith Architecture](001-modular-monolith-architecture.md)
- [ADR-002: Separate DbContext Per Module](002-separate-dbcontext-per-module.md)

## References

- [Soft Delete Pattern](https://en.wikipedia.org/wiki/Soft_delete)
- [EF Core Query Filters](https://docs.microsoft.com/en-us/ef/core/querying/filters)
- [Data Retention Policies](https://en.wikipedia.org/wiki/Data_retention)

