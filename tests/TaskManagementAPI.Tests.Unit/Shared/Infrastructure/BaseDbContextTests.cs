using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Shared.Infrastructure;

namespace TaskManagementAPI.Tests.Unit.Shared.Infrastructure;

public class BaseDbContextTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    private class TestDbContext : BaseDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }

    private DbContextOptions<TestDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task OnModelCreating_AppliesSoftDeleteQueryFilter()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity1 = new TestEntity { Id = Guid.NewGuid(), Name = "Active Entity" };
        var entity2 = new TestEntity { Id = Guid.NewGuid(), Name = "Deleted Entity", IsDeleted = true, DeletedAt = DateTime.UtcNow };

        context.TestEntities.Add(entity1);
        context.TestEntities.Add(entity2);
        await context.SaveChangesAsync();

        // Act
        var result = await context.TestEntities.ToListAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(e => e.Id == entity1.Id);
        result.Should().NotContain(e => e.Id == entity2.Id);
    }

    [Fact]
    public async Task SaveChangesAsync_UpdatesUpdatedAtTimestamp()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.Name = "Updated Name";
        await Task.Delay(10); // Ensure time difference
        await context.SaveChangesAsync();

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }

    [Fact]
    public async Task SaveChangesAsync_SetsSoftDeleteTimestamp()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        // Act
        entity.IsDeleted = true;
        await context.SaveChangesAsync();

        // Assert
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task SaveChanges_UpdatesUpdatedAtTimestamp()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        context.TestEntities.Add(entity);
        context.SaveChanges();

        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.Name = "Updated Name";
        await Task.Delay(10); // Ensure time difference
        context.SaveChanges();

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }

    [Fact]
    public async Task SaveChanges_SetsSoftDeleteTimestamp()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        context.TestEntities.Add(entity);
        context.SaveChanges();

        // Act
        entity.IsDeleted = true;
        context.SaveChanges();

        // Assert
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task QueryFilter_ExcludesSoftDeletedEntitiesFromAllQueries()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var activeEntity = new TestEntity { Id = Guid.NewGuid(), Name = "Active" };
        var deletedEntity = new TestEntity { Id = Guid.NewGuid(), Name = "Deleted", IsDeleted = true, DeletedAt = DateTime.UtcNow };

        context.TestEntities.Add(activeEntity);
        context.TestEntities.Add(deletedEntity);
        await context.SaveChangesAsync();

        // Act
        var allEntities = await context.TestEntities.ToListAsync();
        var byId = await context.TestEntities.FirstOrDefaultAsync(e => e.Id == deletedEntity.Id);

        // Assert
        allEntities.Should().HaveCount(1);
        allEntities.Should().ContainSingle(e => e.Id == activeEntity.Id);
        byId.Should().BeNull();
    }

    [Fact]
    public async Task MultipleEntities_AllApplySoftDeleteFilter()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity1 = new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" };
        var entity2 = new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2", IsDeleted = true, DeletedAt = DateTime.UtcNow };
        var entity3 = new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3" };

        context.TestEntities.AddRange(entity1, entity2, entity3);
        await context.SaveChangesAsync();

        // Act
        var result = await context.TestEntities.ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.Id == entity1.Id);
        result.Should().Contain(e => e.Id == entity3.Id);
        result.Should().NotContain(e => e.Id == entity2.Id);
    }

    [Fact]
    public async Task SaveChangesAsync_DoesNotUpdateDeletedAtIfAlreadySet()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TestDbContext(options);

        var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        var originalDeletedAt = DateTime.UtcNow.AddDays(-1);
        entity.IsDeleted = true;
        entity.DeletedAt = originalDeletedAt;
        await context.SaveChangesAsync();

        var firstDeletedAt = entity.DeletedAt;

        // Act
        entity.Name = "Updated Name";
        await Task.Delay(10);
        await context.SaveChangesAsync();

        // Assert
        entity.DeletedAt.Should().Be(firstDeletedAt);
    }
}
