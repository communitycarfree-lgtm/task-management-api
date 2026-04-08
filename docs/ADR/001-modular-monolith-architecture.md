# ADR-001: Modular Monolith Architecture

**Date**: April 8, 2026  
**Status**: Accepted  
**Context**: Choosing architecture pattern for Task Management API

## Problem

We need to choose an architecture pattern that:
- Allows independent module development
- Maintains clear boundaries between features
- Enables future migration to microservices
- Provides shared infrastructure
- Supports team collaboration

## Decision

We will use a **Modular Monolith** architecture with **Vertical Slice** pattern.

### Architecture Overview

```
Task Management API (Single Deployable)
├── Projects Module (Independent)
├── Tasks Module (Independent)
├── Users Module (Independent)
├── Notifications Module (Independent)
└── Shared Layer (Cross-cutting)
```

### Vertical Slice Pattern

Each module follows:
```
Presentation → Application → Domain → Infrastructure
```

## Rationale

### Advantages

1. **Clear Module Boundaries**
   - Each module has its own DbContext
   - Independent data models
   - Minimal cross-module dependencies

2. **Team Collaboration**
   - Teams can work on modules independently
   - Clear ownership and responsibility
   - Reduced merge conflicts

3. **Scalability**
   - Easy to add new modules
   - Modules can be extracted to microservices later
   - Shared infrastructure can be extracted to libraries

4. **Maintainability**
   - Clear separation of concerns
   - Easy to understand module structure
   - Consistent patterns across modules

5. **Testing**
   - Modules can be tested independently
   - Clear test boundaries
   - Easy to mock dependencies

### Disadvantages

1. **Complexity**
   - More files and folders
   - Requires discipline to maintain boundaries
   - Learning curve for new developers

2. **Code Duplication**
   - Some code may be duplicated across modules
   - Shared utilities must be carefully managed

3. **Performance**
   - Multiple DbContexts may impact performance
   - Requires careful query optimization

## Implementation

### Module Structure

```
Modules/
├── Projects/
│   ├── Presentation/
│   │   └── Controllers/
│   ├── Application/
│   │   ├── DTOs/
│   │   └── Services/
│   ├── Domain/
│   │   ├── Entities/
│   │   └── Enums/
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   └── Services/
│   └── Configuration/
│       └── ProjectsModuleExtensions.cs
```

### Module Registration

```csharp
// Program.cs
builder.Services.AddProjectsModule(configuration);
builder.Services.AddTasksModule(configuration);
builder.Services.AddUsersModule(configuration);
builder.Services.AddNotificationsModule(configuration);
```

### Module Extension Pattern

```csharp
public static class ProjectsModuleExtensions
{
    public static IServiceCollection AddProjectsModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ProjectsDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Register repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        
        // Register services
        services.AddScoped<ProjectService>();
        
        return services;
    }
}
```

## Consequences

### Positive

- ✅ Clear module boundaries
- ✅ Independent development
- ✅ Easy to test
- ✅ Scalable structure
- ✅ Future microservices migration path

### Negative

- ⚠️ More complex structure
- ⚠️ Requires discipline
- ⚠️ Potential code duplication
- ⚠️ Multiple DbContexts

## Alternatives Considered

### 1. Microservices
- **Pros**: Complete independence, independent scaling
- **Cons**: Complexity, distributed transactions, operational overhead
- **Decision**: Too complex for initial development

### 2. Layered Monolith
- **Pros**: Simple structure, easy to understand
- **Cons**: Tight coupling, difficult to scale teams
- **Decision**: Lacks module boundaries

### 3. CQRS + Event Sourcing
- **Pros**: Scalable, audit trail
- **Cons**: Complex, overkill for current requirements
- **Decision**: Not needed for current scope

## Related Decisions

- [ADR-002: Separate DbContext Per Module](002-separate-dbcontext-per-module.md)
- [ADR-003: Soft Delete Implementation](003-soft-delete-implementation.md)
- [ADR-004: SignalR Real-time Updates](004-signalr-real-time-updates.md)

## References

- [Modular Monolith](https://www.kamilgrzybek.com/design/modular-monolith-primer/)
- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

