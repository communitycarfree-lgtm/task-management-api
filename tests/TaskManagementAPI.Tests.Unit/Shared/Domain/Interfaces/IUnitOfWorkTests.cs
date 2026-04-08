using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Shared.Domain.Interfaces;

namespace TaskManagementAPI.Tests.Unit.Shared.Domain.Interfaces;

/// <summary>
/// Unit tests for IUnitOfWork interface contract.
/// Tests that the interface defines the expected transaction management operations.
/// </summary>
public class IUnitOfWorkTests
{
    // Concrete implementations for testing interface
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
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }
    }

    private class TestUnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();
        private int _saveChangesCount = 0;

        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new TestRepository();
            }
            return (IRepository<T>)_repositories[type];
        }

        public Task<int> SaveChangesAsync()
        {
            _saveChangesCount++;
            return Task.FromResult(_saveChangesCount);
        }

        public void Dispose()
        {
            _repositories.Clear();
        }
    }

    [Fact]
    public void Repository_ShouldReturnRepositoryForEntityType()
    {
        // Arrange
        var unitOfWork = new TestUnitOfWork();

        // Act
        var repository = unitOfWork.Repository<TestEntity>();

        // Assert
        Assert.NotNull(repository);
        Assert.IsAssignableFrom<IRepository<TestEntity>>(repository);
    }

    [Fact]
    public void Repository_ShouldReturnSameRepositoryInstanceForSameType()
    {
        // Arrange
        var unitOfWork = new TestUnitOfWork();

        // Act
        var repository1 = unitOfWork.Repository<TestEntity>();
        var repository2 = unitOfWork.Repository<TestEntity>();

        // Assert
        Assert.Same(repository1, repository2);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldReturnNumberOfChanges()
    {
        // Arrange
        var unitOfWork = new TestUnitOfWork();

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldIncrementOnMultipleCalls()
    {
        // Arrange
        var unitOfWork = new TestUnitOfWork();

        // Act
        var result1 = await unitOfWork.SaveChangesAsync();
        var result2 = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result1);
        Assert.Equal(2, result2);
    }

    [Fact]
    public void Dispose_ShouldImplementIDisposable()
    {
        // Arrange
        var unitOfWork = new TestUnitOfWork();

        // Act & Assert
        unitOfWork.Dispose();
        // Should not throw
    }

    [Fact]
    public void UnitOfWork_ShouldImplementIDisposable()
    {
        // Arrange & Act
        var unitOfWork = new TestUnitOfWork();

        // Assert
        Assert.IsAssignableFrom<IDisposable>(unitOfWork);
    }
}
