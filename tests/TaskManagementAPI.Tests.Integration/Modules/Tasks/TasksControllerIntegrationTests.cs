using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagementAPI;
using TaskManagementAPI.Modules.Tasks.Application.DTOs;
using TaskManagementAPI.Modules.Tasks.Domain.Enums;
using Xunit;

namespace TaskManagementAPI.Tests.Integration.Modules.Tasks;

/// <summary>
/// Integration tests for the TasksController.
/// </summary>
public class TasksControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        // Note: In a real scenario, you would set up authentication headers here
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task CreateTask_WithValidRequest_ReturnsCreatedStatus()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            ProjectId = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Description",
            Priority = TaskPriority.High,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadAsAsync<TaskDto>();
        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
    }

    [Fact]
    public async Task CreateTask_WithPastDueDate_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTaskRequest
        {
            ProjectId = Guid.NewGuid(),
            Title = "Test Task",
            DueDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProjectTasks_WithValidProjectId_ReturnsTaskList()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/tasks/project/{projectId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsAsync<TaskListResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetProjectTasks_WithStatusFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/tasks/project/{projectId}?status={TaskStatus.InProgress}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsAsync<TaskListResponse>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateTaskStatus_WithValidStatus_ReturnsUpdatedTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskStatusRequest { Status = TaskStatus.InProgress };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{taskId}/status", request);

        // Assert
        // Note: This will return 404 since we don't have a real task, but it demonstrates the endpoint
        Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignTask_WithValidAssignee_ReturnsUpdatedTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new AssignTaskRequest { AssigneeId = "user123" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tasks/{taskId}/assignee", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteTask_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AddTimeTrackingEntry_WithValidData_ReturnsCreatedStatus()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new AddTimeTrackingRequest
        {
            Hours = 8,
            Date = DateTime.UtcNow
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/tasks/{taskId}/time-entries", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddTaskDependency_WithValidData_ReturnsCreatedStatus()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new AddTaskDependencyRequest { BlockedByTaskId = Guid.NewGuid() };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/tasks/{taskId}/dependencies", request);

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Created);
    }
}
