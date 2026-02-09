# 🏗️ How the Project Works - TodoListApp

A beginner-friendly explanation of the TodoListApp architecture, design patterns, and workflow.

---

## 📐 Architecture Overview

This project follows a **layered architecture** pattern commonly used in ASP.NET Core applications:

```
┌─────────────────────────────────────┐
│         HTTP Request (API)          │
│     (GET, POST, PUT, DELETE)        │
└─────────────────────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│          Controller Layer           │
│      (TodosController.cs)           │
│   - Handles HTTP requests           │
│   - Validates input                 │
│   - Returns HTTP responses          │
└─────────────────────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│          Service Layer              │
│        (TodoService.cs)             │
│   - Business logic                  │
│   - Data manipulation               │
│   - In-memory storage               │
└─────────────────────────────────────┘
                 ↓
┌─────────────────────────────────────┐
│           Data Layer                │
│      (List<TodoItem>)               │
│   - In-memory storage               │
│   - No database required            │
└─────────────────────────────────────┘
```

### Why This Architecture?

- **Separation of Concerns**: Each layer has a specific responsibility
- **Testability**: Layers can be tested independently
- **Maintainability**: Easy to modify or replace individual layers
- **Scalability**: Easy to add features or swap storage mechanisms

---

## 📁 Folder Structure Explained

```
TodoListApp/
│
├── TodoListApp.sln                    # Solution file - groups all projects
│
├── src/                               # Source code folder
│   └── TodoListApp.Api/               # Main API project
│       ├── Controllers/               # HTTP request handlers
│       │   └── TodosController.cs     # Todo CRUD endpoints
│       │
│       ├── Services/                  # Business logic
│       │   └── TodoService.cs         # Todo operations & storage
│       │
│       ├── Models/                    # Data structures
│       │   └── TodoItem.cs            # Todo entity definition
│       │
│       ├── Program.cs                 # App entry point & configuration
│       └── TodoListApp.Api.csproj     # Project configuration
│
├── tests/                             # Test projects folder
│   └── TodoListApp.Tests/             # Unit test project
│       ├── TodoServiceTests.cs        # Service layer tests
│       ├── TodosControllerTests.cs    # Controller layer tests
│       └── TodoListApp.Tests.csproj   # Test project configuration
│
├── .github/                           # GitHub-specific files
│   └── workflows/                     # CI/CD workflows
│       └── ci.yml                     # Continuous Integration pipeline
│
└── docs/                              # Documentation
    ├── RUNNING_GUIDE.md               # How to run the project
    ├── PROJECT_WORKING.md             # This file
    └── POSSIBLE_ERRORS.md             # Troubleshooting guide
```

### Purpose of Each Folder

- **`src/`**: Contains production code that runs in the application
- **`tests/`**: Contains test code that validates the production code
- **`.github/workflows/`**: Contains CI/CD automation scripts
- **`docs/`**: Contains project documentation

---

## 🔄 Request Flow: From HTTP to Response

Let's trace a request through the application:

### Example: Creating a New Todo

```
1. User sends POST request
   ↓
   POST https://localhost:7001/api/todos
   Body: { "title": "Buy milk", "description": "2% milk", "isCompleted": false }

2. ASP.NET Core routing directs to TodosController.Create()
   ↓
   
3. Controller validates the request
   ↓
   - Checks if title is not empty
   - If invalid → returns BadRequest (400)
   - If valid → continues

4. Controller calls TodoService.Create()
   ↓
   
5. Service layer:
   ↓
   - Assigns unique ID (auto-increment)
   - Sets CreatedAt timestamp
   - Adds to in-memory List<TodoItem>
   - Returns created TodoItem

6. Controller receives TodoItem from service
   ↓
   
7. Controller returns HTTP 201 Created
   ↓
   Response: 
   {
     "id": 1,
     "title": "Buy milk",
     "description": "2% milk",
     "isCompleted": false,
     "createdAt": "2026-02-06T10:30:00Z",
     "completedAt": null
   }

8. User receives response
```

---

## 🧩 Component Breakdown

### 1. **Model Layer** (`Models/TodoItem.cs`)

**Purpose**: Defines the data structure for a todo item.

```csharp
public class TodoItem
{
    public int Id { get; set; }              // Unique identifier
    public string Title { get; set; }        // Task title (required)
    public string Description { get; set; }  // Task details (optional)
    public bool IsCompleted { get; set; }    // Completion status
    public DateTime CreatedAt { get; set; }  // Creation timestamp
    public DateTime? CompletedAt { get; set; } // Completion timestamp (nullable)
}
```

**Key Points**:
- `Id` is auto-generated by the service
- `CreatedAt` is set automatically on creation
- `CompletedAt` is only set when `IsCompleted` changes to true
- All properties use C# auto-properties for simplicity

---

### 2. **Service Layer** (`Services/TodoService.cs`)

**Purpose**: Contains business logic and manages in-memory data storage.

#### Interface: `ITodoService`

Defines the contract (methods) that the service must implement:

```csharp
public interface ITodoService
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Create(TodoItem item);
    TodoItem? Update(int id, TodoItem item);
    bool Delete(int id);
}
```

**Why use an interface?**
- Enables dependency injection
- Makes testing easier (can mock the interface)
- Allows swapping implementations without changing controllers

#### Implementation: `TodoService`

```csharp
public class TodoService : ITodoService
{
    private readonly List<TodoItem> _todos = new();  // In-memory storage
    private int _nextId = 1;                          // Auto-increment ID
    
    // Implementation details...
}
```

**Key Features**:

1. **In-Memory Storage**: Uses `List<TodoItem>` to store data
   - Data persists while app is running
   - Data is lost when app restarts
   - Good for development/learning, not production

2. **Auto-Increment ID**:
   ```csharp
   item.Id = _nextId++;  // Assigns ID 1, 2, 3, 4...
   ```

3. **CRUD Operations**:
   - **Create**: Adds new item to list
   - **Read**: Retrieves items from list
   - **Update**: Modifies existing item in list
   - **Delete**: Removes item from list

4. **Timestamp Management**:
   - Sets `CreatedAt` when creating
   - Sets `CompletedAt` when marking as completed
   - Clears `CompletedAt` when marking as incomplete

---

### 3. **Controller Layer** (`Controllers/TodosController.cs`)

**Purpose**: Handles HTTP requests and returns HTTP responses.

```csharp
[ApiController]
[Route("api/[controller]")]  // Creates route: /api/todos
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    
    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;  // Dependency injection
    }
    
    // HTTP methods...
}
```

#### Attributes Explained

- `[ApiController]`: Enables automatic model validation and API-specific behaviors
- `[Route("api/[controller]")]`: Sets base URL to `/api/todos`
- `[HttpGet]`, `[HttpPost]`, etc.: Maps method to HTTP verb
- `[ProducesResponseType]`: Documents possible response status codes

#### HTTP Methods

| Method | Route | Action | Returns |
|--------|-------|--------|---------|
| GET | `/api/todos` | GetAll() | 200 OK with all todos |
| GET | `/api/todos/{id}` | GetById(id) | 200 OK or 404 Not Found |
| POST | `/api/todos` | Create(item) | 201 Created or 400 Bad Request |
| PUT | `/api/todos/{id}` | Update(id, item) | 200 OK, 400 Bad Request, or 404 Not Found |
| DELETE | `/api/todos/{id}` | Delete(id) | 204 No Content or 404 Not Found |

#### Validation Example

```csharp
if (string.IsNullOrWhiteSpace(item.Title))
    return BadRequest(new { message = "Title is required" });
```

- Controller validates input before calling service
- Returns appropriate HTTP status codes
- Provides meaningful error messages

---

### 4. **Program.cs** (Application Entry Point)

**Purpose**: Configures and starts the web application.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register services (Dependency Injection)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITodoService, TodoService>();  // Important!

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // API documentation
    app.UseSwaggerUI();    // Swagger UI
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();      // Map routes to controllers

app.Run();                 // Start the application
```

#### Key Concepts

1. **Dependency Injection**:
   ```csharp
   builder.Services.AddSingleton<ITodoService, TodoService>();
   ```
   - Registers `TodoService` as the implementation of `ITodoService`
   - `Singleton`: One instance shared across entire application lifetime
   - Controllers receive this service automatically via constructor

2. **Middleware Pipeline**:
   - Processes requests in order
   - Each middleware can modify request/response
   - Order matters!

3. **Swagger Integration**:
   - Auto-generates API documentation
   - Provides interactive testing UI
   - Only enabled in Development environment

---

## 🧪 Test Project Architecture

### Why Separate Test Project?

- **Isolation**: Tests don't ship with production code
- **Organization**: Clear separation of concerns
- **Tooling**: Can use different NuGet packages
- **Performance**: Production code doesn't include test frameworks

### Test Structure

```
TodoListApp.Tests/
├── TodoServiceTests.cs        # Tests for TodoService
├── TodosControllerTests.cs    # Tests for TodosController
└── TodoListApp.Tests.csproj   # Test project configuration
```

### Testing Frameworks Used

1. **xUnit**: Test framework
   - `[Fact]`: Marks a test method
   - `[Theory]`: Parameterized tests (not used here but available)

2. **FluentAssertions**: Readable assertions
   ```csharp
   result.Should().NotBeNull();
   result.Id.Should().BeGreaterThan(0);
   todos.Should().HaveCount(2);
   ```

### Test Anatomy

```csharp
[Fact]  // Marks this as a test
public void Create_ShouldAddNewTodo_AndReturnWithId()
{
    // Arrange - Set up test data
    var newTodo = new TodoItem { Title = "Test", Description = "Test" };
    
    // Act - Execute the method being tested
    var result = _todoService.Create(newTodo);
    
    // Assert - Verify the results
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

**AAA Pattern** (Arrange-Act-Assert):
- **Arrange**: Prepare test data and dependencies
- **Act**: Call the method under test
- **Assert**: Verify the outcome

### Test Coverage

**Service Tests** (9 tests):
- Create todo
- Get all todos
- Get by ID (exists and doesn't exist)
- Update todo (exists and doesn't exist)
- Delete todo (exists and doesn't exist)
- ID auto-increment

**Controller Tests** (9 tests):
- GET all (returns OK)
- GET by ID (OK and NotFound)
- POST create (Created and BadRequest)
- PUT update (OK, NotFound, BadRequest)
- DELETE (NoContent and NotFound)

---

## 🔁 In-Memory Storage How It Works

### Storage Mechanism

```csharp
private readonly List<TodoItem> _todos = new();
private int _nextId = 1;
```

### Lifecycle

1. **Application Starts**: Empty list created
2. **Requests Come In**: List is modified (add/update/delete)
3. **Application Runs**: Data persists in memory
4. **Application Stops**: Data is lost

### Visual Representation

```
Application Start:
_todos = []
_nextId = 1

After Create("Buy milk"):
_todos = [
  { Id: 1, Title: "Buy milk", ... }
]
_nextId = 2

After Create("Do laundry"):
_todos = [
  { Id: 1, Title: "Buy milk", ... },
  { Id: 2, Title: "Do laundry", ... }
]
_nextId = 3

After Delete(1):
_todos = [
  { Id: 2, Title: "Do laundry", ... }
]
_nextId = 3  (doesn't decrement)

Application Restart:
_todos = []  (everything lost!)
_nextId = 1
```

### Why Singleton Registration?

```csharp
builder.Services.AddSingleton<ITodoService, TodoService>();
```

- **Singleton**: One instance for entire application lifetime
- Ensures all requests use the same `List<TodoItem>`
- If we used **Scoped** or **Transient**, each request would get a new list (data would be lost immediately)

### Limitations

- ❌ Data doesn't persist across restarts
- ❌ No concurrent access handling
- ❌ No data validation at storage level
- ✅ Simple for learning and development
- ✅ No database setup required
- ✅ Fast for testing

---

## ⚙️ CI Pipeline Workflow

### What is CI (Continuous Integration)?

**Continuous Integration** is a practice where code changes are automatically:
1. Built
2. Tested
3. Validated

Every time you push code to GitHub, the CI pipeline runs automatically.

### CI Workflow Steps

```yaml
name: CI - Build and Test

on:
  push:                    # Trigger on code push
    branches: [ main ]
  pull_request:            # Trigger on pull requests
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest # Use Ubuntu server
    
    steps:
    1. Checkout code       # Download repository
    2. Setup .NET 8        # Install .NET SDK
    3. Restore packages    # Download NuGet packages
    4. Build solution      # Compile code
    5. Run tests           # Execute all tests
    6. Report results      # Show pass/fail
```

### When CI Runs

- ✅ Push to `main`, `master`, or `develop` branch
- ✅ Pull request to these branches
- ✅ Manual trigger (workflow_dispatch)

### What CI Checks

1. **Code Compiles**: No syntax errors
2. **Tests Pass**: All 18 tests must pass
3. **No Breaking Changes**: Ensures code quality

### If CI Fails

- ❌ Tests fail → Fix the code
- ❌ Build fails → Check for compilation errors
- ❌ Restore fails → Check .csproj files

**Pipeline will block merge** until all checks pass (if configured as required check).

---

## 🎯 Design Patterns Used

### 1. **Dependency Injection**

**What**: Objects receive dependencies from outside rather than creating them.

**Example**:
```csharp
// Bad (tight coupling)
public class TodosController
{
    private readonly TodoService _service = new TodoService();
}

// Good (dependency injection)
public class TodosController
{
    private readonly ITodoService _service;
    
    public TodosController(ITodoService service)
    {
        _service = service;  // Injected by framework
    }
}
```

**Benefits**:
- Easy to test (can inject mocks)
- Easy to swap implementations
- Loose coupling

---

### 2. **Repository Pattern** (Simplified)

Our `TodoService` acts as a simplified repository:
- Abstracts data access
- Provides CRUD operations
- Can be swapped with database implementation later

---

### 3. **RESTful API Design**

Follows REST principles:
- **Resource-based URLs**: `/api/todos`
- **HTTP verbs**: GET, POST, PUT, DELETE
- **Stateless**: Each request is independent
- **Standard status codes**: 200, 201, 404, etc.

---

## 🔧 How Everything Connects

```
┌─────────────────────────────────────────────┐
│           User/Client Application           │
│         (Browser, Postman, curl)            │
└─────────────────────────────────────────────┘
                     ↓ HTTP Request
┌─────────────────────────────────────────────┐
│          ASP.NET Core Pipeline              │
│  (Routing, Model Binding, Validation)       │
└─────────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────┐
│           TodosController                   │
│  - Receives HTTP request                    │
│  - Validates input                          │
│  - Calls service layer                      │
└─────────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────┐
│            ITodoService                     │
│    (Dependency Injection Container)         │
│          ↓                                  │
│       TodoService                           │
│  - Business logic                           │
│  - Data manipulation                        │
└─────────────────────────────────────────────┘
                     ↓
┌─────────────────────────────────────────────┐
│         In-Memory Storage                   │
│       List<TodoItem> _todos                 │
└─────────────────────────────────────────────┘
                     ↓
         Response flows back up
                     ↓
┌─────────────────────────────────────────────┐
│              HTTP Response                  │
│        (JSON with status code)              │
└─────────────────────────────────────────────┘
```

---

## 🚀 Future Enhancements (Not Implemented)

This is a learning project. In a production application, you might add:

1. **Real Database**: Entity Framework Core + SQL Server/PostgreSQL
2. **Authentication**: JWT tokens, OAuth
3. **Authorization**: Role-based access control
4. **Logging**: Structured logging (Serilog)
5. **Error Handling**: Global exception middleware
6. **Validation**: FluentValidation library
7. **Caching**: Redis or in-memory caching
8. **API Versioning**: Support multiple API versions
9. **Rate Limiting**: Prevent abuse
10. **Docker**: Containerization
11. **CD**: Deploy to Azure/AWS

---

## 📝 Summary

**What You've Built**:
- ✅ RESTful Web API with CRUD operations
- ✅ Layered architecture (Controller → Service → Storage)
- ✅ Dependency injection
- ✅ Comprehensive unit tests
- ✅ Automated CI pipeline
- ✅ Swagger documentation

**Key Takeaways**:
- Controllers handle HTTP, services handle logic
- Interfaces enable testability and flexibility
- In-memory storage is simple but temporary
- Tests ensure code reliability
- CI automates quality checks

**Next Steps**:
- Explore Entity Framework Core for database integration
- Learn about authentication and authorization
- Study advanced design patterns
- Build a frontend (React, Angular, Vue)

---

Congratulations! You now understand how a modern .NET Web API works! 🎉
