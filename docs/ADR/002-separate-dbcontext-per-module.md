# ADR-002: Separate DbContext Per Module

**Date**: April 8, 2026  
**Status**: Accepted  
**Context**: Database context strategy for modular monolith

## Problem

In a modular monolith, we need to decide:
- Should each module have its own DbContext?
- How do we handle cross-module queries?
- How do we maintain data consistency?

## Decision

Each module will have its own **DbContext** inheriting from **BaseDbContext**.

### Implementation

```csharp
// Projects Module
public class ProjectsDbContext : BaseDbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
}

// Tasks Module
public class TasksDbContext : BaseDbContext
{
    public DbSet<WorkTask> Tasks { get; set; }
    public DbSet<TaskDependency> TaskDependencies { get; set; }
}

// Users Module
public class UsersDbContext : IdentityDbContext<ApplicationUser>
{
    // Identity tables
}

// Notifications Module
public class NotificationsDbContext : BaseDbContext
{
    public DbSet<Notification> Notifications { get; set; }
}
```

## Rationale

### Advantages

1. **Module Independence**
   - Each module owns its data
   - Clear data boundaries
   - Easy to extract to separate database later

2. **Scalability**
   - Modules can have different database strategies
   - Easy to shard data per module
   - Supports future microservices migration

3. **Performance**
   - Smaller DbContext = faster queries
   - Reduced memory footprint
   - Better query optimization per module

4. **Testing**
   - Easy to mock DbContext per module
   - Independent database setup for tests
   - Clear test boundaries

5. **Maintenance**
   - Migrations per module
   - Clear ownership of data model
   - Easier to understand data flow

### Disadvantages

1. **Cross-Module Queries**
   - Cannot use EF Core joins across contexts
   - Must use application-level joins
   - Potential N+1 query problems

2. **Transactions**
   - Cannot use EF Core transactions across contexts
   - Must implement distributed transaction pattern
   - Complexity in maintaining consistency

3. **Code Duplication**
   - Common configurations duplicated
   - Shared entities may be duplicated
   - Requires careful management

## Implementation Details

### BaseDbContext

```csharp
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply soft delete query filters
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
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            
            if (entry.State == EntityState.Modified && entry.Entity.IsDeleted)
                entry.Entity.DeletedAt = DateTime.UtcNow;
        }
    }
}
```

### Module DbContext

```csharp
public class ProjectsDbContext : BaseDbContext
{
    public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options) 
        : base(options) { }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
    }
}
```

### Cross-Module Queries

```csharp
// Instead of EF Core join
var projectsWithMembers = await _projectsContext.Projects
    .Include(p => p.Members)
    .ToListAsync();

// For cross-module queries, use application-level join
var projects = await _projectsContext.Projects.ToListAsync();
var users = await _usersContext.Users.ToListAsync();

var projectsWithUserNames = projects
    .Join(users, p => p.OwnerId, u => u.Id, 
        (p, u) => new { Project = p, Owner = u })
    .ToList();
```

## Consequences

### Positive

- ✅ Clear module boundaries
- ✅ Independent data ownership
- ✅ Better performance per module
- ✅ Easy to extract to microservices
- ✅ Simpler migrations per module

### Negative

- ⚠️ Cross-module queries more complex
- ⚠️ Distributed transactions needed
- ⚠️ Potential code duplication
- ⚠️ Requires careful consistency management

## Alternatives Considered

### 1. Single Shared DbContext
- **Pros**: Simple, easy cross-module queries
- **Cons**: Tight coupling, difficult to scale
- **Decision**: Violates module independence

### 2. Shared DbContext + Module-Specific Contexts
- **Pros**: Flexibility, shared + independent data
- **Cons**: Complex, confusing ownership
- **Decision**: Too complex

### 3. Event-Driven Consistency
- **Pros**: Loose coupling, scalable
- **Cons**: Complex, eventual consistency
- **Decision**: Overkill for current requirements

## Migration Strategy

If cross-module queries become problematic:

1. **Phase 1**: Implement caching for frequently joined data
2. **Phase 2**: Implement event-driven synchronization
3. **Phase 3**: Extract modules to microservices with separate databases

## Related Decisions

- [ADR-001: Modular Monolith Architecture](001-modular-monolith-architecture.md)
- [ADR-003: Soft Delete Implementation](003-soft-delete-implementation.md)

## References

- [Entity Framework Core DbContext](https://docs.microsoft.com/en-us/ef/core/dbcontexts/)
- [Modular Monolith Database Strategy](https://www.kamilgrzybek.com/design/modular-monolith-primer/)

