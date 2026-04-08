# ADR-004: SignalR Real-time Updates

**Date**: April 8, 2026  
**Status**: Accepted  
**Context**: Real-time collaboration requirements

## Problem

We need to:
- Provide real-time updates to connected clients
- Support multiple concurrent connections
- Handle group-based messaging
- Maintain connection state

## Decision

Use **SignalR** for real-time communication with group-based broadcasting.

### Implementation

```csharp
// SignalR Hub
public class TaskUpdatesHub : Hub
{
    public async Task JoinProjectGroup(string projectId)
    {
        await Groups.AddToGroupAsync(Connection.ConnectionId, $"project-{projectId}");
    }
    
    public async Task LeaveProjectGroup(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Connection.ConnectionId, $"project-{projectId}");
    }
    
    public async Task SendTaskUpdate(string projectId, TaskUpdateDto update)
    {
        await Clients.Group($"project-{projectId}")
            .SendAsync("TaskUpdated", update);
    }
}

// Service Integration
public class TaskService
{
    private readonly IHubContext<TaskUpdatesHub> _hubContext;
    
    public async Task UpdateTaskStatusAsync(Guid taskId, TaskStatus status)
    {
        var task = await _repository.GetByIdAsync(taskId);
        task.Status = status;
        await _repository.UpdateAsync(task);
        
        // Broadcast update
        await _hubContext.Clients.Group($"project-{task.ProjectId}")
            .SendAsync("TaskUpdated", new TaskUpdateDto { Id = task.Id, Status = status });
    }
}

// Client Connection
var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5000/hubs/task-updates")
    .WithAutomaticReconnect()
    .Build();

connection.On<TaskUpdateDto>("TaskUpdated", (update) =>
{
    Console.WriteLine($"Task {update.Id} updated to {update.Status}");
});

await connection.StartAsync();
```

## Rationale

### Advantages

1. **Real-time Updates**
   - Instant notification to clients
   - No polling required
   - Reduced latency

2. **Scalability**
   - Supports 1000+ concurrent connections
   - Built-in group management
   - Automatic reconnection

3. **Flexibility**
   - Works with HTTP and WebSocket
   - Fallback mechanisms
   - Cross-platform support

4. **Integration**
   - Native .NET integration
   - Works with ASP.NET Core
   - Easy to implement

5. **User Experience**
   - Collaborative real-time experience
   - Instant feedback
   - Reduced refresh cycles

### Disadvantages

1. **Complexity**
   - Requires client-side implementation
   - Connection management overhead
   - Debugging can be challenging

2. **Resource Usage**
   - Persistent connections consume resources
   - Memory usage per connection
   - Network bandwidth

3. **Scalability Challenges**
   - Horizontal scaling requires backplane
   - State management across servers
   - Connection affinity needed

## Implementation Details

### Hub Configuration

```csharp
// Program.cs
builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<TaskUpdatesHub>("/hubs/task-updates");
```

### Group Management

```csharp
// Join project group
await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");

// Join user group
await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

// Leave group
await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project-{projectId}");
```

### Broadcasting Patterns

```csharp
// Broadcast to specific group
await Clients.Group($"project-{projectId}")
    .SendAsync("TaskUpdated", update);

// Broadcast to specific user
await Clients.Group($"user-{userId}")
    .SendAsync("NotificationReceived", notification);

// Broadcast to all connected clients
await Clients.All.SendAsync("SystemMessage", message);

// Broadcast to all except sender
await Clients.AllExcept(Context.ConnectionId)
    .SendAsync("UserJoined", userName);
```

### Authorization

```csharp
public class TaskUpdatesHub : Hub
{
    private readonly IUserService _userService;
    
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            Context.Abort();
            return;
        }
        
        await base.OnConnectedAsync();
    }
    
    public async Task JoinProjectGroup(string projectId)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        var hasAccess = await _userService.HasProjectAccessAsync(userId, projectId);
        
        if (!hasAccess)
            throw new HubException("Access denied");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
    }
}
```

### Error Handling

```csharp
public override async Task OnDisconnectedAsync(Exception? exception)
{
    if (exception != null)
    {
        _logger.LogError(exception, "SignalR connection error");
    }
    
    await base.OnDisconnectedAsync(exception);
}
```

## Consequences

### Positive

- ✅ Real-time collaboration
- ✅ Instant updates
- ✅ Scalable architecture
- ✅ Good user experience
- ✅ Native .NET support

### Negative

- ⚠️ Increased complexity
- ⚠️ Resource consumption
- ⚠️ Horizontal scaling challenges
- ⚠️ Client-side implementation needed

## Scaling Strategy

### Single Server

```csharp
// Works out of the box
builder.Services.AddSignalR();
```

### Multiple Servers (Backplane)

```csharp
// Using Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis(options =>
    {
        options.ConnectionFactory = async writer =>
        {
            var connection = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
            return connection.GetDatabase();
        };
    });
```

### Kubernetes

```yaml
# Deployment with sticky sessions
apiVersion: apps/v1
kind: Deployment
metadata:
  name: task-management-api
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: api
        image: task-management-api:latest
        ports:
        - containerPort: 80
```

## Monitoring

```csharp
// Track connections
var connectionCount = _hubContext.Clients.All.SendAsync("GetConnectionCount");

// Log connection events
public override async Task OnConnectedAsync()
{
    _logger.LogInformation("Client {ConnectionId} connected", Context.ConnectionId);
    await base.OnConnectedAsync();
}

public override async Task OnDisconnectedAsync(Exception? exception)
{
    _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);
    await base.OnDisconnectedAsync(exception);
}
```

## Alternatives Considered

### 1. WebSockets
- **Pros**: Lower level, more control
- **Cons**: More complex, less abstraction
- **Decision**: SignalR provides better abstraction

### 2. Server-Sent Events (SSE)
- **Pros**: Simpler, HTTP-based
- **Cons**: One-way communication, no fallback
- **Decision**: SignalR supports bidirectional

### 3. Polling
- **Pros**: Simple, no persistent connections
- **Cons**: High latency, resource intensive
- **Decision**: Not suitable for real-time

## Related Decisions

- [ADR-001: Modular Monolith Architecture](001-modular-monolith-architecture.md)

## References

- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr/)
- [SignalR Hubs](https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs)
- [SignalR Groups](https://docs.microsoft.com/en-us/aspnet/core/signalr/groups)
- [Real-time Web Applications](https://en.wikipedia.org/wiki/Real-time_web)

