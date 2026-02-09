using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Api.Data;
using TodoListApp.Api.Models;
using TodoListApp.Api.Services;
using Xunit;

namespace TodoListApp.Tests.Services;

/// <summary>
/// Unit tests for TodoService using InMemory database
/// These tests verify business logic in the service layer
/// </summary>
public class TodoServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ITodoService _service;

    public TodoServiceTests()
    {
        // Setup InMemory database with unique name for each test instance
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new TodoService(_context);
    }

    #region Create Tests

    [Fact]
    public void Create_WithValidTodo_ShouldAddTodoToDatabase()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "Test Todo", 
            Description = "Test Description" 
        };

        // Act
        var result = _service.Create(todo);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Title.Should().Be("Test Todo");
        result.Description.Should().Be("Test Description");
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToCurrentTime()
    {
        // Arrange
        var todo = new TodoItem { Title = "Test" };
        var beforeCreate = DateTime.UtcNow;

        // Act
        var result = _service.Create(todo);
        var afterCreate = DateTime.UtcNow;

        // Assert
        result.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        result.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Arrange & Act
        var todo1 = _service.Create(new TodoItem { Title = "Todo 1" });
        var todo2 = _service.Create(new TodoItem { Title = "Todo 2" });
        var todo3 = _service.Create(new TodoItem { Title = "Todo 3" });

        // Assert
        todo1.Id.Should().NotBe(todo2.Id);
        todo2.Id.Should().NotBe(todo3.Id);
        todo1.Id.Should().NotBe(todo3.Id);
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldSucceed()
    {
        // Arrange
        var todo = new TodoItem { Title = "Title Only", Description = "" };

        // Act
        var result = _service.Create(todo);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().BeEmpty();
    }

    [Fact]
    public void Create_DefaultIsCompletedShouldBeFalse()
    {
        // Arrange
        var todo = new TodoItem { Title = "Test" };

        // Act
        var result = _service.Create(todo);

        // Assert
        result.IsCompleted.Should().BeFalse();
        result.CompletedAt.Should().BeNull();
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public void GetAll_WithNoTodos_ShouldReturnEmptyList()
    {
        // Act
        var result = _service.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_WithMultipleTodos_ShouldReturnAllTodos()
    {
        // Arrange
        _service.Create(new TodoItem { Title = "Todo 1" });
        _service.Create(new TodoItem { Title = "Todo 2" });
        _service.Create(new TodoItem { Title = "Todo 3" });

        // Act
        var result = _service.GetAll().ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Title == "Todo 1");
        result.Should().Contain(t => t.Title == "Todo 2");
        result.Should().Contain(t => t.Title == "Todo 3");
    }

    [Fact]
    public void GetAll_ShouldReturnTodosOrderedByCreatedAtDescending()
    {
        // Arrange
        var first = _service.Create(new TodoItem { Title = "First" });
        Thread.Sleep(10); // Ensure different timestamps
        var second = _service.Create(new TodoItem { Title = "Second" });
        Thread.Sleep(10);
        var third = _service.Create(new TodoItem { Title = "Third" });

        // Act
        var result = _service.GetAll().ToList();

        // Assert
        result.Should().HaveCount(3);
        result[0].Title.Should().Be("Third");  // Most recent first
        result[1].Title.Should().Be("Second");
        result[2].Title.Should().Be("First");
    }

    [Fact]
    public void GetAll_ShouldIncludeAllTodoProperties()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "Complete Todo",
            Description = "Full Description",
            IsCompleted = true
        };
        _service.Create(todo);

        // Act
        var result = _service.GetAll().First();

        // Assert
        result.Title.Should().Be("Complete Todo");
        result.Description.Should().Be("Full Description");
        result.IsCompleted.Should().BeTrue();
        result.CreatedAt.Should().NotBe(default);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public void GetById_WithValidId_ShouldReturnTodo()
    {
        // Arrange
        var created = _service.Create(new TodoItem { Title = "Test Todo" });

        // Act
        var result = _service.GetById(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Title.Should().Be("Test Todo");
    }

    [Fact]
    public void GetById_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = _service.GetById(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetById_WithNegativeId_ShouldReturnNull()
    {
        // Act
        var result = _service.GetById(-1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetById_WithZeroId_ShouldReturnNull()
    {
        // Act
        var result = _service.GetById(0);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region Update Tests

    [Fact]
    public void Update_WithValidData_ShouldUpdateTodo()
    {
        // Arrange
        var original = _service.Create(new TodoItem 
        { 
            Title = "Original Title",
            Description = "Original Description",
            IsCompleted = false
        });

        var updateData = new TodoItem
        {
            Title = "Updated Title",
            Description = "Updated Description",
            IsCompleted = false
        };

        // Act
        var result = _service.Update(original.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(original.Id);
        result.Title.Should().Be("Updated Title");
        result.Description.Should().Be("Updated Description");
    }

    [Fact]
    public void Update_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var updateData = new TodoItem { Title = "Updated" };

        // Act
        var result = _service.Update(999, updateData);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Update_MarkingAsCompleted_ShouldSetCompletedAt()
    {
        // Arrange
        var original = _service.Create(new TodoItem 
        { 
            Title = "Todo",
            IsCompleted = false
        });

        var updateData = new TodoItem
        {
            Title = "Todo",
            IsCompleted = true
        };

        var beforeUpdate = DateTime.UtcNow;

        // Act
        var result = _service.Update(original.Id, updateData);
        var afterUpdate = DateTime.UtcNow;

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeTrue();
        result.CompletedAt.Should().NotBeNull();
        result.CompletedAt.Should().BeOnOrAfter(beforeUpdate);
        result.CompletedAt.Should().BeOnOrBefore(afterUpdate);
    }

    [Fact]
    public void Update_UnmarkingAsCompleted_ShouldClearCompletedAt()
    {
        // Arrange
        var original = _service.Create(new TodoItem 
        { 
            Title = "Todo",
            IsCompleted = true
        });
        
        // Manually set CompletedAt
        original.CompletedAt = DateTime.UtcNow;
        _context.SaveChanges();

        var updateData = new TodoItem
        {
            Title = "Todo",
            IsCompleted = false
        };

        // Act
        var result = _service.Update(original.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.IsCompleted.Should().BeFalse();
        result.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Update_KeepingCompleted_ShouldNotChangeCompletedAt()
    {
        // Arrange
        var original = _service.Create(new TodoItem 
        { 
            Title = "Todo",
            IsCompleted = true
        });
        
        var originalCompletedAt = DateTime.UtcNow.AddHours(-1);
        original.CompletedAt = originalCompletedAt;
        _context.SaveChanges();

        var updateData = new TodoItem
        {
            Title = "Updated Todo",
            IsCompleted = true
        };

        // Act
        var result = _service.Update(original.Id, updateData);

        // Assert
        result.Should().NotBeNull();
        result!.CompletedAt.Should().Be(originalCompletedAt);
    }

    [Fact]
    public void Update_ShouldPersistChangesToDatabase()
    {
        // Arrange
        var original = _service.Create(new TodoItem { Title = "Original" });
        var updateData = new TodoItem { Title = "Updated", Description = "New Desc" };

        // Act
        _service.Update(original.Id, updateData);
        var retrieved = _service.GetById(original.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Title.Should().Be("Updated");
        retrieved.Description.Should().Be("New Desc");
    }

    [Fact]
    public void Update_ShouldNotChangeCreatedAt()
    {
        // Arrange
        var original = _service.Create(new TodoItem { Title = "Original" });
        var originalCreatedAt = original.CreatedAt;
        Thread.Sleep(50); // Ensure time passes

        var updateData = new TodoItem { Title = "Updated" };

        // Act
        var result = _service.Update(original.Id, updateData);

        // Assert
        result!.CreatedAt.Should().Be(originalCreatedAt);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public void Delete_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var todo = _service.Create(new TodoItem { Title = "To Delete" });

        // Act
        var result = _service.Delete(todo.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Delete_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = _service.Delete(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Delete_ShouldRemoveTodoFromDatabase()
    {
        // Arrange
        var todo = _service.Create(new TodoItem { Title = "To Delete" });
        var todoId = todo.Id;

        // Act
        var deleteResult = _service.Delete(todoId);
        var getResult = _service.GetById(todoId);

        // Assert
        deleteResult.Should().BeTrue();
        getResult.Should().BeNull();
    }

    [Fact]
    public void Delete_ShouldNotAffectOtherTodos()
    {
        // Arrange
        var todo1 = _service.Create(new TodoItem { Title = "Todo 1" });
        var todo2 = _service.Create(new TodoItem { Title = "Todo 2" });
        var todo3 = _service.Create(new TodoItem { Title = "Todo 3" });

        // Act
        _service.Delete(todo2.Id);

        // Assert
        _service.GetById(todo1.Id).Should().NotBeNull();
        _service.GetById(todo2.Id).Should().BeNull();
        _service.GetById(todo3.Id).Should().NotBeNull();
        _service.GetAll().Should().HaveCount(2);
    }

    [Fact]
    public void Delete_WithNegativeId_ShouldReturnFalse()
    {
        // Act
        var result = _service.Delete(-1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Delete_SameIdTwice_ShouldReturnFalseOnSecondCall()
    {
        // Arrange
        var todo = _service.Create(new TodoItem { Title = "To Delete" });

        // Act
        var firstDelete = _service.Delete(todo.Id);
        var secondDelete = _service.Delete(todo.Id);

        // Assert
        firstDelete.Should().BeTrue();
        secondDelete.Should().BeFalse();
    }

    #endregion

    #region Edge Cases and Validation

    [Fact]
    public void Create_WithMaxLengthTitle_ShouldSucceed()
    {
        // Arrange
        var longTitle = new string('A', 200); // Max length is 200
        var todo = new TodoItem { Title = longTitle };

        // Act
        var result = _service.Create(todo);

        // Assert
        result.Title.Should().HaveLength(200);
    }

    [Fact]
    public void Create_WithMaxLengthDescription_ShouldSucceed()
    {
        // Arrange
        var longDescription = new string('B', 1000); // Max length is 1000
        var todo = new TodoItem 
        { 
            Title = "Test",
            Description = longDescription 
        };

        // Act
        var result = _service.Create(todo);

        // Assert
        result.Description.Should().HaveLength(1000);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}