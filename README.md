# 📝 TodoListApp - .NET 8 Web API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)
![C#](https://img.shields.io/badge/C%23-11-239120?logo=c-sharp)
![License](https://img.shields.io/badge/license-MIT-blue)
![Build Status](https://img.shields.io/github/actions/workflow/status/YOUR_USERNAME/TodoListApp/ci.yml?branch=main)

A simple, beginner-friendly **To-Do List REST API** built with **.NET 8** and **C#**. This project demonstrates modern web API development practices including CRUD operations, unit testing, and continuous integration.

---

## ✨ Features

- ✅ Complete **CRUD operations** (Create, Read, Update, Delete)
- ✅ **RESTful API** design with proper HTTP methods
- ✅ **In-memory storage** (no database required)
- ✅ **Comprehensive unit tests** with xUnit and FluentAssertions
- ✅ **Swagger/OpenAPI** documentation
- ✅ **Continuous Integration** with GitHub Actions
- ✅ **Clean architecture** with layered design
- ✅ **Dependency injection** pattern

---

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)
- Code editor (VS Code, Visual Studio, or Rider)

### Installation

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/TodoListApp.git

# Navigate to project directory
cd TodoListApp

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project src/TodoListApp.Api

# Run tests
dotnet test
```

### Access the API

Once running, access the API at:
- **Swagger UI**: https://localhost:7001/swagger
- **API Base URL**: https://localhost:7001/api/todos

---

## 📚 API Endpoints

| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/api/todos` | Get all todos | 200 OK |
| GET | `/api/todos/{id}` | Get todo by ID | 200 OK, 404 Not Found |
| POST | `/api/todos` | Create new todo | 201 Created, 400 Bad Request |
| PUT | `/api/todos/{id}` | Update todo | 200 OK, 400 Bad Request, 404 Not Found |
| DELETE | `/api/todos/{id}` | Delete todo | 204 No Content, 404 Not Found |

### Example Requests

**Create a Todo:**
```bash
curl -X POST https://localhost:7001/api/todos \
  -H "Content-Type: application/json" \
  -d '{"title":"Buy groceries","description":"Milk, eggs, bread","isCompleted":false}'
```

**Get All Todos:**
```bash
curl https://localhost:7001/api/todos
```

**Update a Todo:**
```bash
curl -X PUT https://localhost:7001/api/todos/1 \
  -H "Content-Type: application/json" \
  -d '{"title":"Buy groceries","description":"Milk, eggs, bread, cheese","isCompleted":true}'
```

**Delete a Todo:**
```bash
curl -X DELETE https://localhost:7001/api/todos/1
```

---

## 🏗️ Project Structure

```
TodoListApp/
│
├── src/
│   └── TodoListApp.Api/          # Main API project
│       ├── Controllers/          # API endpoints
│       ├── Services/             # Business logic
│       ├── Models/               # Data models
│       └── Program.cs            # Application entry point
│
├── tests/
│   └── TodoListApp.Tests/        # Unit tests
│       ├── TodoServiceTests.cs   # Service tests
│       └── TodosControllerTests.cs # Controller tests
│
├── .github/
│   └── workflows/
│       └── ci.yml                # CI pipeline
│
└── docs/
    ├── RUNNING_GUIDE.md          # Detailed setup instructions
    ├── PROJECT_WORKING.md        # Architecture explanation
    └── POSSIBLE_ERRORS.md        # Troubleshooting guide
```

---

## 🧪 Testing

The project includes **18 comprehensive unit tests** covering:
- ✅ Service layer CRUD operations
- ✅ Controller HTTP responses
- ✅ Input validation
- ✅ Error handling

**Run tests:**
```bash
dotnet test
```

**Run tests with detailed output:**
```bash
dotnet test --verbosity detailed
```

**Test frameworks used:**
- **xUnit** - Testing framework
- **FluentAssertions** - Readable assertions

---

## 🔄 Continuous Integration

This project uses **GitHub Actions** for automated CI pipeline.

**Pipeline runs on:**
- Push to `main`, `master`, or `develop` branches
- Pull requests to these branches

**Pipeline steps:**
1. ✅ Checkout code
2. ✅ Setup .NET 8 SDK
3. ✅ Restore dependencies
4. ✅ Build solution
5. ✅ Run all tests

**CI Status**: Check the badge at the top of this README.

---

## 📖 Documentation

Detailed documentation is available in the `docs/` folder:

- **[RUNNING_GUIDE.md](docs/RUNNING_GUIDE.md)** - Complete setup and running instructions
- **[PROJECT_WORKING.md](docs/PROJECT_WORKING.md)** - Architecture and design explanation
- **[POSSIBLE_ERRORS.md](docs/POSSIBLE_ERRORS.md)** - Common errors and solutions

---

## 🛠️ Built With

- **[.NET 8](https://dotnet.microsoft.com/)** - Framework
- **[ASP.NET Core](https://docs.microsoft.com/aspnet/core)** - Web framework
- **[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Swagger/OpenAPI documentation
- **[xUnit](https://xunit.net/)** - Testing framework
- **[FluentAssertions](https://fluentassertions.com/)** - Assertion library

---

## 🎯 Learning Objectives

This project is designed to teach:
- ✅ Building RESTful APIs with ASP.NET Core
- ✅ Implementing CRUD operations
- ✅ Dependency injection pattern
- ✅ Unit testing best practices
- ✅ CI/CD with GitHub Actions
- ✅ Clean architecture principles

---

## 🚀 Future Enhancements

Potential improvements for learning:
- [ ] Add Entity Framework Core with SQL Server
- [ ] Implement JWT authentication
- [ ] Add input validation with FluentValidation
- [ ] Implement logging with Serilog
- [ ] Add Docker support
- [ ] Deploy to Azure App Service (CD)
- [ ] Add integration tests
- [ ] Implement CQRS pattern

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 👤 Author

**Your Name**
- GitHub: [@YOUR_USERNAME](https://github.com/YOUR_USERNAME)

---

## 🙏 Acknowledgments

- Built as a learning project for .NET 8 and clean architecture
- Inspired by modern REST API best practices
- Thanks to the .NET community for excellent documentation

---

## 📧 Support

If you have questions or run into issues:
1. Check the [Troubleshooting Guide](docs/POSSIBLE_ERRORS.md)
2. Open an [Issue](https://github.com/YOUR_USERNAME/TodoListApp/issues)
3. Review the [Documentation](docs/)

---

**Happy Coding! 🎉**
