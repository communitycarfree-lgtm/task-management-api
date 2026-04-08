using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Shared.Infrastructure.Repositories;

namespace TaskManagementAPI.Tests.Unit.Shared.Infrastructure;

/// <summary>
/// Unit tests for GenericRepository CRUD operations.
/// Tests with in-memory database to verify repository behavior.
/// </summary>
public class GenericRepositoryTests : IAsyncLifetime
{
    public class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> TestEntities { get; set; } = null!;

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
        }
    }

    private readonly DbContextOptions<TestDbContext> _options;
    private TestDbContext _context = null!;

    private TestDbContext CreateContext()
    {
        return new TestDbContext(_options);
    }

    public GenericRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    public async Task InitializeAsync()
    {
        _context = CreateContext();
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
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
        // Arrange & Act
        var repository = new GenericRepository<TestEntity>(_context);

        // Assert
        repository.Should().NotBeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddToContext()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);
        var entity = new TestEntity { Name = "Test" };

        // Act
        var result = await repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        result.Should().Be(entity);
        result.Name.Should().Be("Test");
        var retrieved = await repository.GetByIdAsync(entity.Id);
        retrieved.Should().NotBeNull();
        retrieved?.Name.Should().Be("Test");
    }

    [Fact]
    public async Task AddAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);

        // Act & Assert
        var action = () => repository.AddAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);
        var entity = new TestEntity { Name = "Test" };
        await repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(entity.Id);

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);
        var entities = new List<TestEntity>
        {
            new TestEntity { Name = "Entity1" },
            new TestEntity { Name = "Entity2" },
            new TestEntity { Name = "Entity3" }
        };

        foreach (var entity in entities)
        {
            await repository.AddAsync(entity);
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateAsync_WithValidEntity_ShouldUpdateContext()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);
        var entity = new TestEntity { Name = "Original" };
        await repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        entity.Name = "Updated";

        // Act
        await repository.UpdateAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await repository.GetByIdAsync(entity.Id);
        updated?.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateAsync_WithNullEntity_ShouldThrowArgumentNullException()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);

        // Act & Assert
        var action = () => repository.UpdateAsync(null!);
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSoftDeleteEntity()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);
        var entity = new TestEntity { Name = "ToDelete" };
        await repository.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(entity.Id);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await repository.GetByIdAsync(entity.Id);
        deleted?.IsDeleted.Should().BeTrue();
        deleted?.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
    {
        // Arrange
        var repository = new GenericRepository<TestEntity>(_context);

        // Act & Assert
        var action = () => repository.DeleteAsync(Guid.NewGuid());
        await action.Should().NotThrowAsync();
    }
}
