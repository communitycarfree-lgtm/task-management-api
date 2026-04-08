using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Persistence;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Services;
using Xunit;
using TaskStatus = TaskManagementAPI.Modules.Tasks.Domain.Enums.TaskStatus;

namespace TaskManagementAPI.Tests.Unit.Modules.Tasks;

/// <summary>
/// Unit tests for the TaskRepository class.
/// </summary>
public class TaskRepositoryTests : IAsyncLifetime
{
    private readonly DbContextOptions<TasksDbContext> _options;
    private TasksDbContext _context = null!;
    private TaskRepository _repository = null!;

    public TaskRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<TasksDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    public async Task InitializeAsync()
    {
        _context = new TasksDbContext(_options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new TaskRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task GetProjectTasksPagedAsync_WithValidProjectId_ReturnsPaginatedTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var tasks = new List<WorkTask>
        {
            new WorkTask { ProjectId = projectId, Title = "Task 1", Status = TaskStatus.New },
            new WorkTask { ProjectId = projectId, Title = "Task 2", Status = TaskStatus.InProgress },
            new WorkTask { ProjectId = projectId, Title = "Task 3", Status = TaskStatus.Completed }
        };
        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        // Act
        var (result, totalCount) = await _repository.GetProjectTasksPagedAsync(projectId, 1, 10);

        // Assert
        Assert.Equal(3, totalCount);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetProjectTasksWithFiltersAsync_WithStatusFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var tasks = new List<WorkTask>
        {
            new WorkTask { ProjectId = projectId, Title = "Task 1", Status = TaskStatus.New },
            new WorkTask { ProjectId = projectId, Title = "Task 2", Status = TaskStatus.InProgress },
            new WorkTask { ProjectId = projectId, Title = "Task 3", Status = TaskStatus.Completed }
        };
        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        // Act
        var (result, totalCount) = await _repository.GetProjectTasksWithFiltersAsync(
            projectId, status: TaskStatus.InProgress);

        // Assert
        Assert.Equal(1, totalCount);
        Assert.Single(result);
        Assert.Equal("Task 2", result.First().Title);
    }

    [Fact]
    public async Task GetProjectTasksWithFiltersAsync_WithPriorityFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var tasks = new List<WorkTask>
        {
            new WorkTask { ProjectId = projectId, Title = "Task 1", Priority = TaskPriority.Low },
            new WorkTask { ProjectId = projectId, Title = "Task 2", Priority = TaskPriority.High },
            new WorkTask { ProjectId = projectId, Title = "Task 3", Priority = TaskPriority.Critical }
        };
        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        // Act
        var (result, totalCount) = await _repository.GetProjectTasksWithFiltersAsync(
            projectId, priority: TaskPriority.High);

        // Assert
        Assert.Equal(1, totalCount);
        Assert.Single(result);
        Assert.Equal("Task 2", result.First().Title);
    }

    [Fact]
    public async Task GetUserTasksAsync_WithValidAssignee_ReturnsUserTasks()
    {
        // Arrange
        var assigneeId = "user123";
        var tasks = new List<WorkTask>
        {
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Task 1", AssigneeId = assigneeId },
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Task 2", AssigneeId = assigneeId },
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Task 3", AssigneeId = "other-user" }
        };
        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        // Act
        var (result, totalCount) = await _repository.GetUserTasksAsync(assigneeId);

        // Assert
        Assert.Equal(2, totalCount);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetOverdueTasksAsync_ReturnsOnlyOverdueTasks()
    {
        // Arrange
        var tasks = new List<WorkTask>
        {
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Overdue", DueDate = DateTime.UtcNow.AddDays(-1), Status = TaskStatus.New },
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Future", DueDate = DateTime.UtcNow.AddDays(1), Status = TaskStatus.New },
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Completed", DueDate = DateTime.UtcNow.AddDays(-1), Status = TaskStatus.Completed }
        };
        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetOverdueTasksAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Overdue", result.First().Title);
    }

    [Fact]
    public async Task GetTasksDueSoonAsync_ReturnsTasksDueWithinSpecifiedDays()
    {
        // Arrange
        var tasks = new List<WorkTask>
        {
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Due Soon", DueDate = DateTime.UtcNow.AddHours(12), Status = TaskStatus.New },
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Due Later", DueDate = DateTime.UtcNow.AddDays(5), Status = TaskStatus.New },
            new WorkTask { ProjectId = Guid.NewGuid(), Title = "Overdue", DueDate = DateTime.UtcNow.AddDays(-1), Status = TaskStatus.New }
        };
        await _context.Tasks.AddRangeAsync(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetTasksDueSoonAsync(days: 1);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task HasIncompleteBlockingTasksAsync_WithIncompleteBlockers_ReturnsTrue()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var blockingTaskId = Guid.NewGuid();

        var blockingTask = new WorkTask { Id = blockingTaskId, ProjectId = Guid.NewGuid(), Title = "Blocking", Status = TaskStatus.New };
        var task = new WorkTask { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Blocked" };
        var dependency = new TaskDependency { TaskId = taskId, BlockedByTaskId = blockingTaskId };

        await _context.Tasks.AddAsync(blockingTask);
        await _context.Tasks.AddAsync(task);
        await _context.TaskDependencies.AddAsync(dependency);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasIncompleteBlockingTasksAsync(taskId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasIncompleteBlockingTasksAsync_WithCompleteBlockers_ReturnsFalse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var blockingTaskId = Guid.NewGuid();

        var blockingTask = new WorkTask { Id = blockingTaskId, ProjectId = Guid.NewGuid(), Title = "Blocking", Status = TaskStatus.Completed };
        var task = new WorkTask { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Blocked" };
        var dependency = new TaskDependency { TaskId = taskId, BlockedByTaskId = blockingTaskId };

        await _context.Tasks.AddAsync(blockingTask);
        await _context.Tasks.AddAsync(task);
        await _context.TaskDependencies.AddAsync(dependency);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasIncompleteBlockingTasksAsync(taskId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetBlockingTasksAsync_ReturnsAllBlockingTasks()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var blockingTask1Id = Guid.NewGuid();
        var blockingTask2Id = Guid.NewGuid();

        var blockingTask1 = new WorkTask { Id = blockingTask1Id, ProjectId = Guid.NewGuid(), Title = "Blocking 1" };
        var blockingTask2 = new WorkTask { Id = blockingTask2Id, ProjectId = Guid.NewGuid(), Title = "Blocking 2" };
        var task = new WorkTask { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Blocked" };

        await _context.Tasks.AddRangeAsync(blockingTask1, blockingTask2, task);
        await _context.TaskDependencies.AddRangeAsync(
            new TaskDependency { TaskId = taskId, BlockedByTaskId = blockingTask1Id },
            new TaskDependency { TaskId = taskId, BlockedByTaskId = blockingTask2Id }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBlockingTasksAsync(taskId);

        // Assert
        Assert.Equal(2, result.Count());
    }
}

