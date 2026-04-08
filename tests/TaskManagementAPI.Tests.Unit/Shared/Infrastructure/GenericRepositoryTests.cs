using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Shared.Infrastructure.Repositories;

namespace TaskManagementAPI.Tests.Unit.Shared.Infrastructure;

/// <summary>
/// Unit tests for GenericRepository CRUD operations.
/// Tests with mocked DbContext to verify repository behavior.
/// </summary>
public class GenericRepositoryTests
{
    private class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    private Mock<DbContext> CreateMockDbContext()
    {
        var mockContext = new Mock<DbContext>();
        var mockDbSet = new Mock<DbSet<TestEntity>>();

        mockContext
            .Setup(c => c.Set<TestEntity>())
            .Returns(mockDbSet.Object);

        return mockContext;
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        var action = () => new GenericRepository<TestEntity>(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithValidContext_ShouldSucceed()
    {
        // Arrange
        var mockContext = CreateMockDbContext();

        // Act
        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddToContext()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);
        var entity = new TestEntity { Name = "Test" };

        // Act
        var result = await repository.AddAsync(entity);

        // Assert
        result.Should().Be(entity);
        mockDbSet.Verify(s => s.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act & Assert
        var action = () => repository.AddAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        var entity = new TestEntity { Name = "Test" };
        var id = entity.Id;

        mockDbSet
            .Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act
        var result = await repository.GetByIdAsync(id);

        // Assert
        result.Should().Be(entity);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();

        mockDbSet
            .Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestEntity?)null);

        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        var entities = new List<TestEntity>
        {
            new TestEntity { Name = "Entity1" },
            new TestEntity { Name = "Entity2" },
            new TestEntity { Name = "Entity3" }
        };

        mockDbSet
            .Setup(s => s.ToListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);

        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainInOrder(entities);
    }

    [Fact]
    public async Task UpdateAsync_WithValidEntity_ShouldUpdateContext()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);
        var entity = new TestEntity { Name = "Updated" };

        // Act
        await repository.UpdateAsync(entity);

        // Assert
        mockDbSet.Verify(s => s.Update(entity), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act & Assert
        var action = () => repository.UpdateAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSoftDeleteEntity()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();
        var entity = new TestEntity { Name = "ToDelete" };
        var id = entity.Id;

        mockDbSet
            .Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act
        await repository.DeleteAsync(id);

        // Assert
        entity.IsDeleted.Should().BeTrue();
        entity.DeletedAt.Should().NotBeNull();
        mockDbSet.Verify(s => s.Update(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var mockContext = CreateMockDbContext();
        var mockDbSet = new Mock<DbSet<TestEntity>>();

        mockDbSet
            .Setup(s => s.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestEntity?)null);

        mockContext.Setup(c => c.Set<TestEntity>()).Returns(mockDbSet.Object);

        var repository = new GenericRepository<TestEntity>(mockContext.Object);

        // Act & Assert
        var action = () => repository.DeleteAsync(Guid.NewGuid());
        await action.Should().NotThrowAsync();
    }
}
