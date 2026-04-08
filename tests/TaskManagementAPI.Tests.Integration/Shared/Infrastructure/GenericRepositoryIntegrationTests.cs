using FluentAssertions;
using TaskManagementAPI.Shared.Infrastructure.Repositories;
using TaskManagementAPI.Tests.Integration.Fixtures;

namespace TaskManagementAPI.Tests.Integration.Shared.Infrastructure;

public class GenericRepositoryIntegrationTests : IAsyncLifetime
{
    private DatabaseFixture _fixture = null!;
    private GenericRepository<DatabaseFixture.TestEntity> _repository = null!;

    public async Task InitializeAsync()
    {
        _fixture = new DatabaseFixture();
        await _fixture.InitializeAsync();
        _repository = new GenericRepository<DatabaseFixture.TestEntity>(_fixture.Context);
    }

    public async Task DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_PersistsToDatabase()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

        // Act
        await _repository.AddAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(entity.Id);
        retrieved.Should().NotBeNull();
        retrieved?.Name.Should().Be("Test Entity");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsEntity()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
        await _repository.AddAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(entity.Id);
        result?.Name.Should().Be("Test Entity");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllNonDeletedEntities()
    {
        // Arrange
        var entity1 = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" };
        var entity2 = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" };
        var entity3 = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 3", IsDeleted = true, DeletedAt = DateTime.UtcNow };

        await _repository.AddAsync(entity1);
        await _repository.AddAsync(entity2);
        await _repository.AddAsync(entity3);
        await _fixture.Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(e => e.Id == entity1.Id);
        result.Should().Contain(e => e.Id == entity2.Id);
        result.Should().NotContain(e => e.Id == entity3.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithValidEntity_PersistsChanges()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Original Name" };
        await _repository.AddAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Act
        entity.Name = "Updated Name";
        await _repository.UpdateAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(entity.Id);
        retrieved?.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SoftDeletesEntity()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity to Delete" };
        await _repository.AddAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity.Id);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByIdAsync(entity.Id);
        retrieved.Should().BeNull();

        // Verify it's actually soft-deleted by checking the raw context
        // For in-memory database, we need to access the entity directly
        var allEntities = _fixture.Context.TestEntities.ToList();
        var deletedEntity = allEntities.FirstOrDefault(e => e.Id == entity.Id);
        deletedEntity.Should().NotBeNull();
        deletedEntity?.IsDeleted.Should().BeTrue();
        deletedEntity?.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Act & Assert
        await _repository.DeleteAsync(Guid.NewGuid());
        await _fixture.Context.SaveChangesAsync();
    }

    [Fact]
    public async Task MultipleOperations_MaintainDataIntegrity()
    {
        // Arrange
        var entity1 = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" };
        var entity2 = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" };

        // Act
        await _repository.AddAsync(entity1);
        await _repository.AddAsync(entity2);
        await _fixture.Context.SaveChangesAsync();

        entity1.Name = "Updated Entity 1";
        await _repository.UpdateAsync(entity1);
        await _fixture.Context.SaveChangesAsync();

        await _repository.DeleteAsync(entity2.Id);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        var all = await _repository.GetAllAsync();
        all.Should().HaveCount(1);
        all.First().Name.Should().Be("Updated Entity 1");
    }

    [Fact]
    public async Task GetByIdAsync_ExcludesSoftDeletedEntity()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        await _repository.AddAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity.Id);
        await _fixture.Context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUpdatedAtTimestamp()
    {
        // Arrange
        var entity = new DatabaseFixture.TestEntity { Id = Guid.NewGuid(), Name = "Entity" };
        await _repository.AddAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        var originalUpdatedAt = entity.UpdatedAt;

        // Act
        entity.Name = "Updated";
        await Task.Delay(10);
        await _repository.UpdateAsync(entity);
        await _fixture.Context.SaveChangesAsync();

        // Assert
        entity.UpdatedAt.Should().NotBeNull();
        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt ?? DateTime.MinValue);
    }
}
