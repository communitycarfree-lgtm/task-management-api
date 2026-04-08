using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Tests.Unit.Shared.Domain.Interfaces;

/// <summary>
/// Unit tests for IRepository interface contract.
/// Tests that the interface defines the expected CRUD operations.
/// </summary>
public class IRepositoryTests
{
    // Concrete implementation for testing interface
    private class TestEntity : BaseEntity { }

    private class TestRepository : IRepository<TestEntity>
    {
        private readonly List<TestEntity> _entities = new();

        public Task<TestEntity?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_entities.FirstOrDefault(e => e.Id == id));
        }

        public Task<IEnumerable<TestEntity>> GetAllAsync()
        {
            return Task.FromResult(_entities.AsEnumerable());
        }

        public Task<TestEntity> AddAsync(TestEntity entity)
        {
            _entities.Add(entity);
            return Task.FromResult(entity);
        }

        public Task UpdateAsync(TestEntity entity)
        {
            var existing = _entities.FirstOrDefault(e => e.Id == entity.Id);
            if (existing != null)
            {
                existing.UpdatedAt = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            var entity = _entities.FirstOrDefault(e => e.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntityWhenExists()
    {
        // Arrange
        var repository = new TestRepository();
        var entity = new TestEntity();
        await repository.AddAsync(entity);

        // Act
        var result = await repository.GetByIdAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullWhenNotExists()
    {
        // Arrange
        var repository = new TestRepository();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var repository = new TestRepository();
        var entity1 = new TestEntity();
        var entity2 = new TestEntity();
        await repository.AddAsync(entity1);
        await repository.AddAsync(entity2);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyWhenNoEntities()
    {
        // Arrange
        var repository = new TestRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntityAndReturnIt()
    {
        // Arrange
        var repository = new TestRepository();
        var entity = new TestEntity();

        // Act
        var result = await repository.AddAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        var retrieved = await repository.GetByIdAsync(entity.Id);
        Assert.NotNull(retrieved);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var repository = new TestRepository();
        var entity = new TestEntity();
        await repository.AddAsync(entity);
        var beforeUpdate = DateTime.UtcNow;

        // Act
        await repository.UpdateAsync(entity);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        var updated = await repository.GetByIdAsync(entity.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.UpdatedAt);
        Assert.True(updated.UpdatedAt >= beforeUpdate && updated.UpdatedAt <= afterUpdate);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteEntity()
    {
        // Arrange
        var repository = new TestRepository();
        var entity = new TestEntity();
        await repository.AddAsync(entity);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await repository.DeleteAsync(entity.Id);

        var afterDelete = DateTime.UtcNow;

        // Assert
        var deleted = await repository.GetByIdAsync(entity.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
        Assert.NotNull(deleted.DeletedAt);
        Assert.True(deleted.DeletedAt >= beforeDelete && deleted.DeletedAt <= afterDelete);
    }
}
