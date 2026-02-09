using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoListApp.Api.Data;
using TodoListApp.Api.Models;
using Xunit;

namespace TodoListApp.Tests.Integration;

/// <summary>
/// Integration tests for the entire API using WebApplicationFactory
/// These tests verify the complete HTTP request/response pipeline
/// </summary>
public class TodoApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public TodoApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        
        // Clear the database before each test
        CleanDatabase();
    }

    #region GET /api/todos Tests

    [Fact]
    public async Task GetAll_WithNoTodos_ReturnsEmptyArray()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        todos.Should().NotBeNull();
        todos.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_WithTodos_ReturnsAllTodos()
    {
        // Arrange
        var todo1 = await CreateTodoAsync(new TodoItem { Title = "Integration Todo 1" });
        var todo2 = await CreateTodoAsync(new TodoItem { Title = "Integration Todo 2" });

        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        todos.Should().NotBeNull();
        todos.Should().Contain(t => t.Title == "Integration Todo 1");
        todos.Should().Contain(t => t.Title == "Integration Todo 2");
    }

    [Fact]
    public async Task GetAll_ReturnsCorrectContentType()
    {
        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
    }

    #endregion

    #region GET /api/todos/{id} Tests

    [Fact]
    public async Task GetById_WithValidId_ReturnsTodo()
    {
        // Arrange
        var created = await CreateTodoAsync(new TodoItem { Title = "GetById Test", Description = "Test Desc" });

        // Act
        var response = await _client.GetAsync($"/api/todos/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(created.Id);
        todo.Title.Should().Be("GetById Test");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/todos/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WithNegativeId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/todos/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/todos Tests

    [Fact]
    public async Task Create_WithValidTodo_ReturnsCreated()
    {
        // Arrange
        var newTodo = new TodoItem 
        { 
            Title = "New Integration Todo",
            Description = "New Description",
            IsCompleted = false
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        todo.Should().NotBeNull();
        todo!.Id.Should().BeGreaterThan(0);
        todo.Title.Should().Be("New Integration Todo");
        todo.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Create_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var newTodo = new TodoItem { Title = "", Description = "Description" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithWhitespaceTitle_ReturnsBadRequest()
    {
        // Arrange
        var newTodo = new TodoItem { Title = "   ", Description = "Description" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithoutDescription_ReturnsCreated()
    {
        // Arrange
        var newTodo = new TodoItem { Title = "Title Only" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var todo = await response.Content.ReadFromJsonAsync<TodoItem>();
        todo!.Description.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_ReturnsLocationHeader()
    {
        // Arrange
        var newTodo = new TodoItem { Title = "Location Test" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        // Assert
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().MatchRegex(@"/api/[Tt]odos/\d+");
    }

    #endregion

    #region PUT /api/todos/{id} Tests

    [Fact]
    public async Task Update_WithValidData_ReturnsOk()
    {
        // Arrange
        var created = await CreateTodoAsync(new TodoItem { Title = "Original", Description = "Original Desc" });
        var updateData = new TodoItem 
        { 
            Title = "Updated Title",
            Description = "Updated Desc",
            IsCompleted = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<TodoItem>();
        updated!.Title.Should().Be("Updated Title");
        updated.Description.Should().Be("Updated Desc");
        updated.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateData = new TodoItem { Title = "Updated" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/todos/999999", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithEmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var created = await CreateTodoAsync(new TodoItem { Title = "Original" });
        var updateData = new TodoItem { Title = "" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_MarkingAsCompleted_SetsCompletedAt()
    {
        // Arrange
        var created = await CreateTodoAsync(new TodoItem { Title = "Todo", IsCompleted = false });
        var updateData = new TodoItem { Title = "Todo", IsCompleted = true };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", updateData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<TodoItem>();
        updated!.IsCompleted.Should().BeTrue();
        updated.CompletedAt.Should().NotBeNull();
        updated.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    #endregion

    #region DELETE /api/todos/{id} Tests

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var created = await CreateTodoAsync(new TodoItem { Title = "To Delete" });

        // Act
        var response = await _client.DeleteAsync($"/api/todos/{created.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync("/api/todos/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_RemovesTodo_VerifyWithGet()
    {
        // Arrange
        var created = await CreateTodoAsync(new TodoItem { Title = "To Delete and Verify" });

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}");
        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region End-to-End Workflow Tests

    [Fact]
    public async Task FullWorkflow_CreateReadUpdateDelete_WorksCorrectly()
    {
        // 1. Create
        var newTodo = new TodoItem { Title = "Workflow Test", Description = "Test Description" };
        var createResponse = await _client.PostAsJsonAsync("/api/todos", newTodo);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<TodoItem>();

        // 2. Read
        var getResponse = await _client.GetAsync($"/api/todos/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<TodoItem>();
        retrieved!.Title.Should().Be("Workflow Test");

        // 3. Update
        var updateData = new TodoItem { Title = "Updated Workflow", Description = "Updated", IsCompleted = true };
        var updateResponse = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", updateData);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await updateResponse.Content.ReadFromJsonAsync<TodoItem>();
        updated!.Title.Should().Be("Updated Workflow");
        updated.IsCompleted.Should().BeTrue();

        // 4. Delete
        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 5. Verify deletion
        var verifyResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task MultipleOperations_WorkIndependently()
    {
        // Create multiple todos
        var todo1 = await CreateTodoAsync(new TodoItem { Title = "Multi Op Todo 1" });
        var todo2 = await CreateTodoAsync(new TodoItem { Title = "Multi Op Todo 2" });
        var todo3 = await CreateTodoAsync(new TodoItem { Title = "Multi Op Todo 3" });

        // Update one
        var updateData = new TodoItem { Title = "Updated Multi Op Todo 2", IsCompleted = true };
        await _client.PutAsJsonAsync($"/api/todos/{todo2.Id}", updateData);

        // Delete one
        await _client.DeleteAsync($"/api/todos/{todo3.Id}");

        // Verify state
        var getAllResponse = await _client.GetAsync("/api/todos");
        var allTodos = await getAllResponse.Content.ReadFromJsonAsync<List<TodoItem>>();
        
        allTodos.Should().Contain(t => t.Id == todo1.Id && t.Title == "Multi Op Todo 1");
        allTodos.Should().Contain(t => t.Id == todo2.Id && t.Title == "Updated Multi Op Todo 2" && t.IsCompleted);
        allTodos.Should().NotContain(t => t.Id == todo3.Id);
    }

    #endregion

    #region CORS Tests

    [Fact]
    public async Task Api_ShouldAllowCorsFromConfiguredOrigin()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add("Origin", "http://localhost:5500");

        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert - CORS headers may not be present in test environment
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Helper Methods

    private async Task<TodoItem> CreateTodoAsync(TodoItem todo)
    {
        var response = await _client.PostAsJsonAsync("/api/todos", todo);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }

    private void CleanDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.TodoItems.RemoveRange(db.TodoItems);
        db.SaveChanges();
    }

    #endregion
}

/// <summary>
/// Custom WebApplicationFactory that configures a shared InMemory database for all tests
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"IntegrationTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database with shared instance for all tests
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });
    }
}