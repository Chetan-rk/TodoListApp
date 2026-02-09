using Microsoft.AspNetCore.Mvc;
using TodoListApp.Api.Models;
using TodoListApp.Api.Services;

namespace TodoListApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    /// <summary>
    /// Get all todos
    /// </summary>
    /// <returns>List of all todo items</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TodoItem>> GetAll()
    {
        var todos = _todoService.GetAll();
        return Ok(todos);
    }

    /// <summary>
    /// Get a specific todo by ID
    /// </summary>
    /// <param name="id">Todo ID</param>
    /// <returns>Todo item if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoItem> GetById(int id)
    {
        var todo = _todoService.GetById(id);
        if (todo == null)
            return NotFound(new { message = $"Todo with ID {id} not found" });

        return Ok(todo);
    }

    /// <summary>
    /// Create a new todo
    /// </summary>
    /// <param name="item">Todo item to create</param>
    /// <returns>Created todo item</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TodoItem> Create([FromBody] TodoItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Title))
            return BadRequest(new { message = "Title is required" });

        var createdTodo = _todoService.Create(item);
        return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
    }

    /// <summary>
    /// Update an existing todo
    /// </summary>
    /// <param name="id">Todo ID</param>
    /// <param name="item">Updated todo data</param>
    /// <returns>Updated todo item</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoItem> Update(int id, [FromBody] TodoItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Title))
            return BadRequest(new { message = "Title is required" });

        var updatedTodo = _todoService.Update(id, item);
        if (updatedTodo == null)
            return NotFound(new { message = $"Todo with ID {id} not found" });

        return Ok(updatedTodo);
    }

    /// <summary>
    /// Delete a todo
    /// </summary>
    /// <param name="id">Todo ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var result = _todoService.Delete(id);
        if (!result)
            return NotFound(new { message = $"Todo with ID {id} not found" });

        return NoContent();
    }
}
