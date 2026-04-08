using Moq;
using TaskManagementAPI.Modules.Tasks.Application.Services;
using TaskManagementAPI.Modules.Tasks.Domain.Entities;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;
using TaskManagementAPI.Modules.Tasks.Infrastructure.Services;
using TaskManagementAPI.Shared.Domain.Interfaces;
using Xunit;

namespace TaskManagementAPI.Tests.Unit.Modules.Tasks;

/// <summary>
/// Unit tests for the TaskService class.
/// </summary>
public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly TaskService _taskService;

    public TaskServiceTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _taskService = new TaskService(_mockTaskRepository.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_WithValidData_CreatesTaskSuccessfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var title = "Test Task";
        var description = "Test Description";
        var priority = TaskPriority.High;
        var dueDate = DateTime.UtcNow.AddDays(5);

        _mockTaskRepository.Setup(r => r.AddAsync(It.IsAny<Task>()))
            .ReturnsAsync((Task t) => t);

        // Act
        var result = await _taskService.CreateTaskAsync(projectId, title, description, priority, dueDate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(title, result.Title);
        Assert.Equal(description, result.Description);
        Assert.Equal(priority, result.Priority);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(TaskStatus.New, result.Status);
        _mockTaskRepository.Verify(r => r.AddAsync(It.IsAny<Task>()), Times.Once);
        _mockNotificationService.Verify(n => n.BroadcastAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateTaskAsync_WithPastDueDate_ThrowsArgumentException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _taskService.CreateTaskAsync(projectId, "Test", null, TaskPriority.Medium, pastDate));
    }

    [Fact]
    public async Task UpdateTaskAsync_WithValidData_UpdatesTaskSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new Task
        {
            Id = taskId,
            ProjectId = Guid.NewGuid(),
            Title = "Old Title",
            Description = "Old Description",
            Priority = TaskPriority.Low,
            Status = TaskStatus.New
        };

        var newTitle = "New Title";
        var newDescription = "New Description";
        var newPriority = TaskPriority.High;

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, newTitle, newDescription, newPriority);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newTitle, result.Title);
        Assert.Equal(newDescription, result.Description);
        Assert.Equal(newPriority, result.Priority);
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_WithNonExistentTask_ReturnsNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((Task?)null);

        // Act
        var result = await _taskService.UpdateTaskAsync(taskId, "Title");

        // Assert
        Assert.Null(result);
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTaskAsync_WithValidId_DeletesTaskSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test" };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.DeleteAsync(taskId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.DeleteTaskAsync(taskId);

        // Assert
        Assert.True(result);
        _mockTaskRepository.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ToCompleted_WithNoBlockingTasks_Succeeds()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test", Status = TaskStatus.InProgress };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.HasIncompleteBlockingTasksAsync(taskId))
            .ReturnsAsync(false);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.UpdateTaskStatusAsync(taskId, TaskStatus.Completed);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TaskStatus.Completed, result.Status);
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskStatusAsync_ToCompleted_WithBlockingTasks_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test", Status = TaskStatus.InProgress };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.HasIncompleteBlockingTasksAsync(taskId))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _taskService.UpdateTaskStatusAsync(taskId, TaskStatus.Completed));
    }

    [Fact]
    public async Task AssignTaskAsync_WithValidAssignee_AssignsTaskSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var assigneeId = "user123";
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test", Priority = TaskPriority.Medium };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.AssignTaskAsync(taskId, assigneeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assigneeId, result.AssigneeId);
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<Task>()), Times.Once);
        _mockNotificationService.Verify(n => n.BroadcastAsync(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task AssignTaskAsync_WithCriticalPriority_NotifiesProjectManager()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var assigneeId = "user123";
        var task = new Task { Id = taskId, ProjectId = projectId, Title = "Test", Priority = TaskPriority.Critical };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.AssignTaskAsync(taskId, assigneeId);

        // Assert
        Assert.NotNull(result);
        _mockNotificationService.Verify(
            n => n.BroadcastAsync($"project-{projectId}", It.Is<string>(s => s.Contains("CRITICAL"))),
            Times.Once);
    }

    [Fact]
    public async Task AddTimeTrackingEntryAsync_WithValidData_CreatesEntrySuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = "user123";
        var hours = 8m;
        var date = DateTime.UtcNow;
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test", Status = TaskStatus.InProgress };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.AddTimeTrackingEntryAsync(taskId, userId, hours, date);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.TaskId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(hours, result.Hours);
        Assert.Equal(date, result.Date);
    }

    [Fact]
    public async Task AddTimeTrackingEntryAsync_WithNonInProgressTask_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test", Status = TaskStatus.New };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _taskService.AddTimeTrackingEntryAsync(taskId, "user123", 8, DateTime.UtcNow));
    }

    [Fact]
    public async Task AddTaskDependencyAsync_WithValidTasks_CreatesDependencySuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var blockedByTaskId = Guid.NewGuid();
        var task = new Task { Id = taskId, ProjectId = Guid.NewGuid(), Title = "Test" };
        var blockingTask = new Task { Id = blockedByTaskId, ProjectId = Guid.NewGuid(), Title = "Blocking" };

        _mockTaskRepository.Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);
        _mockTaskRepository.Setup(r => r.GetByIdAsync(blockedByTaskId))
            .ReturnsAsync(blockingTask);
        _mockTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<Task>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskService.AddTaskDependencyAsync(taskId, blockedByTaskId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.TaskId);
        Assert.Equal(blockedByTaskId, result.BlockedByTaskId);
    }

    [Fact]
    public async Task AddTaskDependencyAsync_WithSameTaskId_ThrowsException()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _taskService.AddTaskDependencyAsync(taskId, taskId));
    }
}
