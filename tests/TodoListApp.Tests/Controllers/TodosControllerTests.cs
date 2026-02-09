using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Api.Controllers;
using TodoListApp.Api.Data;
using TodoListApp.Api.Models;
using TodoListApp.Api.Services;
using Xunit;

namespace TodoListApp.Tests.Controllers;

/// <summary>
/// Integration tests for TodosController using InMemory database
/// These tests verify the full flow from controller through service to database
/// </summary>
public class TodosControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TodosController _controller;
    private readonly ITodoService _service;

    public TodosControllerTests()
    {
        // Setup InMemory database with unique name for each test instance
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new TodoService(_context);
        _controller = new TodosController(_service);
    }

    #region GetAll Tests

    [Fact]
    public void GetAll_WithNoTodos_ReturnsEmptyList()
    {
        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var todos = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItem>>().Subject;
        todos.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_WithMultipleTodos_ReturnsAllTodos()
    {
        // Arrange
        var todo1 = new TodoItem { Title = "Todo 1", Description = "Description 1" };
        var todo2 = new TodoItem { Title = "Todo 2", Description = "Description 2" };
        _service.Create(todo1);
        _service.Create(todo2);

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var todos = okResult.Value.Should().BeAssignableTo<IEnumerable<TodoItem>>().Subject.ToList();
        todos.Should().HaveCount(2);
        todos.Should().Contain(t => t.Title == "Todo 1");
        todos.Should().Contain(t => t.Title == "Todo 2");
    }

    [Fact]
    public void GetAll_ReturnsOkResultWith200StatusCode()
    {
        // Act
        var result = _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.StatusCode.Should().Be(200);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public void GetById_WithValidId_ReturnsTodo()
    {
        // Arrange
        var todo = new TodoItem { Title = "Test Todo", Description = "Test Description" };
        var created = _service.Create(todo);

        // Act
        var result = _controller.GetById(created.Id);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTodo = okResult.Value.Should().BeOfType<TodoItem>().Subject;
        returnedTodo.Id.Should().Be(created.Id);
        returnedTodo.Title.Should().Be("Test Todo");
        returnedTodo.Description.Should().Be("Test Description");
    }

    [Fact]
    public void GetById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = _controller.GetById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public void GetById_WithNegativeId_ReturnsNotFound()
    {
        // Act
        var result = _controller.GetById(-1);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Create Tests

    [Fact]
    public void Create_WithValidTodo_ReturnsCreatedResult()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "New Todo", 
            Description = "New Description",
            IsCompleted = false
        };

        // Act
        var result = _controller.Create(todo);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        var returnedTodo = createdResult.Value.Should().BeOfType<TodoItem>().Subject;
        returnedTodo.Id.Should().BeGreaterThan(0);
        returnedTodo.Title.Should().Be("New Todo");
        returnedTodo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var todo = new TodoItem { Title = "", Description = "Description" };

        // Act
        var result = _controller.Create(todo);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public void Create_WithWhitespaceTitle_ReturnsBadRequest()
    {
        // Arrange
        var todo = new TodoItem { Title = "   ", Description = "Description" };

        // Act
        var result = _controller.Create(todo);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_WithNullTitle_ReturnsBadRequest()
    {
        // Arrange
        var todo = new TodoItem { Title = null!, Description = "Description" };

        // Act
        var result = _controller.Create(todo);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Create_WithoutDescription_ReturnsCreatedResult()
    {
        // Arrange
        var todo = new TodoItem { Title = "Title Only" };

        // Act
        var result = _controller.Create(todo);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var returnedTodo = createdResult!.Value as TodoItem;
        returnedTodo!.Description.Should().BeEmpty();
    }

    [Fact]
    public void Create_ReturnsLocationHeader()
    {
        // Arrange
        var todo = new TodoItem { Title = "Test Todo" };

        // Act
        var result = _controller.Create(todo);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(TodosController.GetById));
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_WithValidData_ReturnsUpdatedTodo()
    {
        // Arrange
        var original = new TodoItem { Title = "Original", Description = "Original Desc" };
        var created = _service.Create(original);
        
        var updated = new TodoItem 
        { 
            Title = "Updated", 
            Description = "Updated Desc",
            IsCompleted = true 
        };

        // Act
        var result = _controller.Update(created.Id, updated);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTodo = okResult.Value.Should().BeOfType<TodoItem>().Subject;
        returnedTodo.Title.Should().Be("Updated");
        returnedTodo.Description.Should().Be("Updated Desc");
        returnedTodo.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var todo = new TodoItem { Title = "Test" };

        // Act
        var result = _controller.Update(999, todo);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Update_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var original = new TodoItem { Title = "Original" };
        var created = _service.Create(original);
        var updated = new TodoItem { Title = "" };

        // Act
        var result = _controller.Update(created.Id, updated);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Update_MarkingAsCompleted_SetsCompletedAt()
    {
        // Arrange
        var original = new TodoItem { Title = "Original", IsCompleted = false };
        var created = _service.Create(original);
        var updated = new TodoItem { Title = "Original", IsCompleted = true };

        // Act
        var result = _controller.Update(created.Id, updated);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTodo = okResult.Value.Should().BeOfType<TodoItem>().Subject;
        returnedTodo.CompletedAt.Should().NotBeNull();
        returnedTodo.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Update_UnmarkingAsCompleted_ClearsCompletedAt()
    {
        // Arrange
        var original = new TodoItem { Title = "Original", IsCompleted = true };
        var created = _service.Create(original);
        created.CompletedAt = DateTime.UtcNow;
        _context.SaveChanges();

        var updated = new TodoItem { Title = "Original", IsCompleted = false };

        // Act
        var result = _controller.Update(created.Id, updated);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedTodo = okResult.Value.Should().BeOfType<TodoItem>().Subject;
        returnedTodo.CompletedAt.Should().BeNull();
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var todo = new TodoItem { Title = "To Delete" };
        var created = _service.Create(todo);

        // Act
        var result = _controller.Delete(created.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var noContentResult = result as NoContentResult;
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public void Delete_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Delete_RemovesTodo_FromDatabase()
    {
        // Arrange
        var todo = new TodoItem { Title = "To Delete" };
        var created = _service.Create(todo);
        var todoId = created.Id;

        // Act
        _controller.Delete(todoId);

        // Assert
        var deletedTodo = _service.GetById(todoId);
        deletedTodo.Should().BeNull();
    }

    [Fact]
    public void Delete_WithNegativeId_ReturnsNotFound()
    {
        // Act
        var result = _controller.Delete(-1);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void FullCrudWorkflow_CreatesUpdatesAndDeletesTodo()
    {
        // Create
        var newTodo = new TodoItem { Title = "Workflow Test", Description = "Test Description" };
        var createResult = _controller.Create(newTodo);
        var createdTodo = ((createResult.Result as CreatedAtActionResult)!.Value as TodoItem)!;

        // Read
        var getResult = _controller.GetById(createdTodo.Id);
        var retrievedTodo = ((getResult.Result as OkObjectResult)!.Value as TodoItem)!;
        retrievedTodo.Title.Should().Be("Workflow Test");

        // Update
        var updateData = new TodoItem { Title = "Updated Workflow", Description = "Updated", IsCompleted = true };
        var updateResult = _controller.Update(createdTodo.Id, updateData);
        var updatedTodo = ((updateResult.Result as OkObjectResult)!.Value as TodoItem)!;
        updatedTodo.Title.Should().Be("Updated Workflow");
        updatedTodo.IsCompleted.Should().BeTrue();

        // Delete
        var deleteResult = _controller.Delete(createdTodo.Id);
        deleteResult.Should().BeOfType<NoContentResult>();

        // Verify deletion
        var getDeletedResult = _controller.GetById(createdTodo.Id);
        getDeletedResult.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}