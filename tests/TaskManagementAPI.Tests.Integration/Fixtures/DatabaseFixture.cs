using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Shared.Domain;
using TaskManagementAPI.Shared.Infrastructure;

namespace TaskManagementAPI.Tests.Integration.Fixtures;

/// <summary>
/// Test fixture providing an in-memory database for integration tests.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private TestDbContext? _context;

    public TestDbContext Context => _context ?? throw new InvalidOperationException("Context not initialized");

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        _context = new TestDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (_context != null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }

    /// <summary>
    /// Test DbContext for integration tests.
    /// </summary>
    public class TestDbContext : BaseDbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.HasQueryFilter(e => !e.IsDeleted);
            });
        }
    }

    /// <summary>
    /// Test entity for integration tests.
    /// </summary>
    public class TestEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}
