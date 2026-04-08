# 🎯 Task Management API

A modular monolith API for efficient project and task management, built with .NET 9 and high-quality architectural standards.

## 🚀 Overview

The Task Management API is a production-ready system designed for scalability, maintainability, and real-time collaboration. It follows a vertical slice architecture combined with modular monolith principles to ensure clear separation of concerns.

### Key Features
- **Project Management**: Create, update, and manage project lifecycles.
- **Task Tracking**: Detailed task management with dependencies and time tracking.
- **User Management**: Identity-based authentication and role-based access control.
- **Real-time Updates**: SignalR-powered notifications for collaborative workflows.
- **Soft Deletion**: Transparent data persistence with logic-based removals.
- **Modular Monolith**: Each feature is isolated to prevent code entanglement.

## 🛠️ Tech Stack
- **Backend**: ASP.NET Core 9.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity (JWT)
- **Real-time**: SignalR
- **Documentation**: Swagger/OpenAPI

## 📚 Documentation

Detailed technical documentation is available in the repository:

- 🏗️ **[ARCHITECTURE.md](ARCHITECTURE.md)**: System design, diagrams, and data flows.
- 💻 **[DEVELOPMENT.md](DEVELOPMENT.md)**: Coding standards, AI context, and setup guide.
- 📊 **[API Specification](http://localhost:5120/swagger/index.html)**: Interactive Swagger UI (Requires app running).

## 🚦 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server

### Installation
1. Clone the repository.
2. Update connection strings in `TaskManagementAPI/appsettings.json`.
3. Run migrations and start the application:
   ```bash
   dotnet run --project TaskManagementAPI
   ```

## 📜 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
