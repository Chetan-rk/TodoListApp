# 📝 TodoListApp - Full Stack .NET 8 Application

[![.NET CI](https://github.com/YOUR_USERNAME/TodoListApp/actions/workflows/ci.yml/badge.svg)](https://github.com/YOUR_USERNAME/TodoListApp/actions/workflows/ci.yml)

A production-ready Todo List application with ASP.NET Core Web API backend and modern frontend, featuring complete CRUD operations, automated testing, and CI/CD pipeline.

## 🚀 Features

### Backend (ASP.NET Core Web API)
- **RESTful API** with CRUD operations (Create, Read, Update, Delete)
- **Entity Framework Core** with SQL Server database
- **Repository Pattern** with Service layer abstraction
- **Input Validation** with automatic error handling
- **CORS Configuration** for frontend integration
- **Swagger/OpenAPI** documentation

### Frontend (Vanilla JavaScript)
- **Modern UI** with responsive design
- **Real-time updates** without page refresh
- **Form validation** and user feedback
- **Notification system** for user actions
- **Clean, maintainable code** with ES6+ features

### Testing & Quality
- **Unit Tests** for business logic (xUnit, FluentAssertions)
- **Integration Tests** for API endpoints
- **In-Memory Database** for isolated testing
- **GitHub Actions CI** pipeline
- **Code Coverage** reporting

## 🏗️ Architecture
TodoListApp/
├── 📁 src/
│ └── TodoListApp.Api/ # ASP.NET Core Web API
│ ├── Controllers/ # API endpoints
│ ├── Models/ # Data models
│ ├── Services/ # Business logic
│ ├── Data/ # Database context
│ └── Program.cs # Startup configuration
│
├── 📁 tests/
│ └── TodoListApp.Tests/ # Test projects
│ ├── Controllers/ # API tests
│ ├── Services/ # Service tests
│ ├── Integration/ # Integration tests
│ └── Data/ # DB context tests
│
├── 📁 frontend/ # Frontend files
│ ├── index.html # Main HTML
│ └── app.js # Frontend logic
│
├── 📄 TodoListApp.sln # Solution file
├── 📄 .github/workflows/ci.yml # CI pipeline
└── 📄 PROJECT_DOCUMENTATION.md # Detailed documentation

## 🛠️ Tech Stack

### Backend
- **.NET 8.0** - Modern framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 8** - ORM for database
- **SQL Server** - Production database
- **SQLite/InMemory** - Testing databases
- **Swagger** - API documentation

### Frontend
- **HTML5** - Semantic markup
- **CSS3** - Modern styling
- **Vanilla JavaScript** - No frameworks needed
- **Fetch API** - HTTP requests

### Testing
- **xUnit** - Testing framework
- **FluentAssertions** - Readable assertions
- **Moq** - Mocking framework
- **Entity Framework Core InMemory** - Test database
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing

## 📦 Dependencies

### Runtime Dependencies
- Microsoft.EntityFrameworkCore.SqlServer (8.0.0)
- Microsoft.EntityFrameworkCore.Tools (8.0.0)
- Swashbuckle.AspNetCore (6.5.0) - Swagger/OpenAPI

### Development Dependencies
- Microsoft.EntityFrameworkCore.InMemory (8.0.11) - Testing
- Microsoft.AspNetCore.Mvc.Testing (8.0.11) - Integration tests
- xUnit (2.9.3) - Testing framework
- FluentAssertions (6.12.1) - Assertion library
- Moq (4.20.72) - Mocking library

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) or Docker
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR_USERNAME/TodoListApp.git
   cd TodoListApp
