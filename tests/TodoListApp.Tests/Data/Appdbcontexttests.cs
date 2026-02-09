using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Api.Data;
using TodoListApp.Api.Models;
using Xunit;

namespace TodoListApp.Tests.Data;

/// <summary>
/// Tests for AppDbContext configuration and entity mappings
/// </summary>
public class AppDbContextTests : IDisposable
{
    private readonly AppDbContext _context;

    public AppDbContextTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    #region Context Configuration Tests

    [Fact]
    public void Context_ShouldHaveTodoItemsDbSet()
    {
        // Assert
        _context.TodoItems.Should().NotBeNull();
    }

    [Fact]
    public void Context_ShouldBeAbleToAddAndSaveTodoItem()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "Test Todo",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        _context.TodoItems.Add(todo);
        var changes = _context.SaveChanges();

        // Assert
        changes.Should().Be(1);
        todo.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Context_ShouldTrackEntityChanges()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "Original",
            CreatedAt = DateTime.UtcNow
        };
        _context.TodoItems.Add(todo);
        _context.SaveChanges();

        // Act
        todo.Title = "Modified";
        var state = _context.Entry(todo).State;

        // Assert
        state.Should().Be(EntityState.Modified);
    }

    #endregion

    #region Entity Validation Tests

    [Fact]
    public void TodoItem_ShouldHaveIdAsKey()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TodoItem));

        // Act
        var primaryKey = entityType!.FindPrimaryKey();

        // Assert
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().ContainSingle();
        primaryKey.Properties.First().Name.Should().Be("Id");
    }

    [Fact]
    public void TodoItem_TitleShouldBeRequired()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TodoItem));
        var titleProperty = entityType!.FindProperty("Title");

        // Assert
        titleProperty.Should().NotBeNull();
        titleProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void TodoItem_TitleShouldHaveMaxLength200()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TodoItem));
        var titleProperty = entityType!.FindProperty("Title");

        // Assert
        titleProperty!.GetMaxLength().Should().Be(200);
    }

    [Fact]
    public void TodoItem_DescriptionShouldHaveMaxLength1000()
    {
        // Arrange
        var entityType = _context.Model.FindEntityType(typeof(TodoItem));
        var descriptionProperty = entityType!.FindProperty("Description");

        // Assert
        descriptionProperty!.GetMaxLength().Should().Be(1000);
    }

    [Fact]
    public void TodoItem_DescriptionCanBeEmpty()
    {
        // Arrange & Act
        var todo = new TodoItem 
        { 
            Title = "Test",
            Description = "",
            CreatedAt = DateTime.UtcNow
        };
        _context.TodoItems.Add(todo);
        var changes = _context.SaveChanges();

        // Assert
        changes.Should().Be(1);
        todo.Description.Should().BeEmpty();
    }

    #endregion

    #region CRUD Operation Tests

    [Fact]
    public void Context_ShouldSupportMultipleInserts()
    {
        // Arrange & Act
        for (int i = 1; i <= 5; i++)
        {
            _context.TodoItems.Add(new TodoItem 
            { 
                Title = $"Todo {i}",
                CreatedAt = DateTime.UtcNow
            });
        }
        var changes = _context.SaveChanges();

        // Assert
        changes.Should().Be(5);
        _context.TodoItems.Count().Should().Be(5);
    }

    [Fact]
    public void Context_ShouldSupportUpdate()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "Original",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow
        };
        _context.TodoItems.Add(todo);
        _context.SaveChanges();

        // Act
        todo.Title = "Updated";
        todo.Description = "Updated Description";
        var changes = _context.SaveChanges();

        // Assert
        changes.Should().Be(1);
        var updated = _context.TodoItems.Find(todo.Id);
        updated!.Title.Should().Be("Updated");
        updated.Description.Should().Be("Updated Description");
    }

    [Fact]
    public void Context_ShouldSupportDelete()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "To Delete",
            CreatedAt = DateTime.UtcNow
        };
        _context.TodoItems.Add(todo);
        _context.SaveChanges();
        var todoId = todo.Id;

        // Act
        _context.TodoItems.Remove(todo);
        var changes = _context.SaveChanges();

        // Assert
        changes.Should().Be(1);
        _context.TodoItems.Find(todoId).Should().BeNull();
    }

    [Fact]
    public void Context_ShouldSupportFind()
    {
        // Arrange
        var todo = new TodoItem 
        { 
            Title = "Findable Todo",
            CreatedAt = DateTime.UtcNow
        };
        _context.TodoItems.Add(todo);
        _context.SaveChanges();

        // Act
        var found = _context.TodoItems.Find(todo.Id);

        // Assert
        found.Should().NotBeNull();
        found!.Title.Should().Be("Findable Todo");
    }

    [Fact]
    public void Context_ShouldSupportLinqQueries()
    {
        // Arrange
        _context.TodoItems.Add(new TodoItem { Title = "Todo 1", IsCompleted = false, CreatedAt = DateTime.UtcNow });
        _context.TodoItems.Add(new TodoItem { Title = "Todo 2", IsCompleted = true, CreatedAt = DateTime.UtcNow });
        _context.TodoItems.Add(new TodoItem { Title = "Todo 3", IsCompleted = false, CreatedAt = DateTime.UtcNow });
        _context.SaveChanges();

        // Act
        var completedTodos = _context.TodoItems
            .Where(t => t.IsCompleted)
            .ToList();

        // Assert
        completedTodos.Should().HaveCount(1);
        completedTodos.First().Title.Should().Be("Todo 2");
    }

    [Fact]
    public void Context_ShouldSupportOrderBy()
    {
        // Arrange
        var now = DateTime.UtcNow;
        _context.TodoItems.Add(new TodoItem { Title = "Third", CreatedAt = now.AddHours(2) });
        _context.TodoItems.Add(new TodoItem { Title = "First", CreatedAt = now });
        _context.TodoItems.Add(new TodoItem { Title = "Second", CreatedAt = now.AddHours(1) });
        _context.SaveChanges();

        // Act
        var orderedTodos = _context.TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        // Assert
        orderedTodos[0].Title.Should().Be("Third");
        orderedTodos[1].Title.Should().Be("Second");
        orderedTodos[2].Title.Should().Be("First");
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}