using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Tests.Integration.Fixtures;

namespace TaskManagementAPI.Tests.Integration.Shared.Infrastructure;

public class BaseDbContextIntegrationTests : IAsyncLifetime
{
    private DatabaseFixture _fixture = null!;

    public async Task InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_PersistsNewEntity()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

        // Act
        _fixture.Context.TestEntities.Add(entity);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        var retrieved = await _fixture.Context.TestEntities.FirstOrDefaultAsync(e => e.Id == entity.Id);
        retrieved.Should().NotBeNull();
        retrieved?.Name.Should().Be("Test Entity");
    }

    [Fact]
    public async Task SaveChangesAsync_UpdatesUpdatedAtOnModification()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Original" };
        _fixture.Context.TestEntities.Add(entity);
        await _fixture.Context.SaveChangesAsync();

        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.Name = "Updated";
        await Task.Delay(10);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }

    [Fact]
    public async Task SaveChangesAsync_SetsDeletedAtOnSoftDelete()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        _fixture.Context.TestEntities.Add(entity);
        await _fixture.Context.SaveChangesAsync();

        // Act
        entity.IsDeleted = true;
        await _fixture.Context.SaveChangesAsync();

        // Assert
        entity.DeletedAt.Should().NotBeNull();
        entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task QueryFilter_ExcludesSoftDeletedEntities()
    {
        // Arrange
        var activeEntity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Active" };
        var deletedEntity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Deleted" };

        _fixture.Context.TestEntities.Add(activeEntity);
        _fixture.Context.TestEntities.Add(deletedEntity);
        await _fixture.Context.SaveChangesAsync();

        deletedEntity.IsDeleted = true;
        await _fixture.Context.SaveChangesAsync();

        // Act
        var result = await _fixture.Context.TestEntities.ToListAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(e => e.Id == activeEntity.Id);
    }

    [Fact]
    public async Task IgnoreQueryFilters_IncludesSoftDeletedEntities()
    {
        // Arrange
        var activeEntity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Active" };
        var deletedEntity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Deleted" };

        _fixture.Context.TestEntities.Add(activeEntity);
        _fixture.Context.TestEntities.Add(deletedEntity);
        await _fixture.Context.SaveChangesAsync();

        deletedEntity.IsDeleted = true;
        await _fixture.Context.SaveChangesAsync();

        // Act
        var result = await _fixture.Context.TestEntities.IgnoreQueryFilters().ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.Id == activeEntity.Id);
        result.Should().Contain(e => e.Id == deletedEntity.Id);
    }

    [Fact]
    public async Task SaveChanges_SynchronousMethod_UpdatesTimestamps()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        _fixture.Context.TestEntities.Add(entity);
        _fixture.Context.SaveChanges();

        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.Name = "Updated";
        await Task.Delay(10);
        _fixture.Context.SaveChanges();

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }

    [Fact]
    public async Task MultipleEntities_AllApplyQueryFilter()
    {
        // Arrange
        var entities = new[]
        {
            new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
            new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
            new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 3" },
            new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 4" }
        };

        _fixture.Context.TestEntities.AddRange(entities);
        await _fixture.Context.SaveChangesAsync();

        // Act - Delete two entities
        entities[1].IsDeleted = true;
        entities[3].IsDeleted = true;
        await _fixture.Context.SaveChangesAsync();

        var result = await _fixture.Context.TestEntities.ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.Id == entities[0].Id);
        result.Should().Contain(e => e.Id == entities[2].Id);
    }

    [Fact]
    public async Task CreatedAt_IsSetAutomatically()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        var beforeCreation = DateTime.UtcNow;

        // Act
        _fixture.Context.TestEntities.Add(entity);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        entity.CreatedAt.Should().BeOnOrAfter(beforeCreation);
        entity.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task SoftDeletedEntity_CanBeRestored()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        _fixture.Context.TestEntities.Add(entity);
        await _fixture.Context.SaveChangesAsync();

        entity.IsDeleted = true;
        await _fixture.Context.SaveChangesAsync();

        // Act
        entity.IsDeleted = false;
        entity.DeletedAt = null;
        await _fixture.Context.SaveChangesAsync();

        var result = await _fixture.Context.TestEntities.FirstOrDefaultAsync(e => e.Id == entity.Id);

        // Assert
        result.Should().NotBeNull();
        result?.IsDeleted.Should().BeFalse();
        result?.DeletedAt.Should().BeNull();
    }

    [Fact]
    public async Task DeletedAtNotUpdatedOnSubsequentModifications()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        _fixture.Context.TestEntities.Add(entity);
        await _fixture.Context.SaveChangesAsync();

        entity.IsDeleted = true;
        await _fixture.Context.SaveChangesAsync();
        var firstDeletedAt = entity.DeletedAt;

        // Act
        entity.Name = "Modified After Delete";
        await Task.Delay(10);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        entity.DeletedAt.Should().Be(firstDeletedAt);
    }
}
