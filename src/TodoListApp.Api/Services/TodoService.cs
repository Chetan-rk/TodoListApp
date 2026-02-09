using Microsoft.EntityFrameworkCore;
using TodoListApp.Api.Data;
using TodoListApp.Api.Models;

namespace TodoListApp.Api.Services;

public interface ITodoService
{
    IEnumerable<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Create(TodoItem item);
    TodoItem? Update(int id, TodoItem item);
    bool Delete(int id);
}

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;

    public TodoService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TodoItem> GetAll()
    {
        Console.WriteLine($"📋 TodoService.GetAll called");
        var todos = _context.TodoItems
            .OrderByDescending(t => t.CreatedAt)
            .ToList();
        Console.WriteLine($"   Found {todos.Count} todos");
        return todos;
    }

    public TodoItem? GetById(int id)
    {
        Console.WriteLine($"🔍 TodoService.GetById called for ID: {id}");
        return _context.TodoItems.Find(id);
    }

    public TodoItem Create(TodoItem item)
    {
        Console.WriteLine($"📝 TodoService.Create called with Title: {item?.Title}");
    
        try
        {
            item.CreatedAt = DateTime.UtcNow;
            Console.WriteLine($"   Setting CreatedAt to: {item.CreatedAt}");
        
            _context.TodoItems.Add(item);
            Console.WriteLine($"   Added to context, ID should be: {item.Id} (before save)");
        
            var changes = _context.SaveChanges();
            Console.WriteLine($"   SaveChanges returned: {changes} changes");
            Console.WriteLine($"   After save, ID is: {item.Id}");
        
            return item;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ ERROR in Create: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"   🔍 Inner error: {ex.InnerException.Message}");
            throw;
        }
    }

    public TodoItem? Update(int id, TodoItem item)
    {
        Console.WriteLine($"✏️ TodoService.Update called for ID: {id}");
        
        var existingTodo = _context.TodoItems.Find(id);
        if (existingTodo == null)
        {
            Console.WriteLine($"   Todo with ID {id} not found");
            return null;
        }

        if (item.IsCompleted && !existingTodo.IsCompleted)
        {
            existingTodo.CompletedAt = DateTime.UtcNow;
            Console.WriteLine($"   Marked todo {id} as completed");
        }
        else if (!item.IsCompleted)
        {
            existingTodo.CompletedAt = null;
        }

        existingTodo.Title = item.Title;
        existingTodo.Description = item.Description;
        existingTodo.IsCompleted = item.IsCompleted;

        _context.SaveChanges();
        Console.WriteLine($"   Todo {id} updated successfully");
        
        return existingTodo;
    }

    public bool Delete(int id)
    {
        Console.WriteLine($"🗑️ TodoService.Delete called for ID: {id}");
        
        var todo = _context.TodoItems.Find(id);
        if (todo == null)
        {
            Console.WriteLine($"   Todo with ID {id} not found");
            return false;
        }

        _context.TodoItems.Remove(todo);
        _context.SaveChanges();
        Console.WriteLine($"   Todo {id} deleted successfully");
        
        return true;
    }
}