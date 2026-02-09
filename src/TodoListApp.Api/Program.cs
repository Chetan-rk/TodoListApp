using Microsoft.EntityFrameworkCore;
using TodoListApp.Api.Data;
using TodoListApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS for frontend - THIS IS CRITICAL!
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Configure Database - HARDCODED for testing
var connectionString = "Server=localhost\\SQLEXPRESS01;Database=TodoListDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";
Console.WriteLine($"🔗 Connection string: {connectionString}");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register TodoService
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Configure the HTTP request pipeline

// IMPORTANT: UseCors must come before UseAuthorization and MapControllers
app.UseCors("AllowFrontend");

// Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// Test database connection ONCE at startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Console.WriteLine("🔄 Testing database connection...");
        
        // Test connection
        var canConnect = db.Database.CanConnect();
        Console.WriteLine($"✅ Database can connect: {canConnect}");
        
        if (canConnect)
        {
            // Get database info
            var dbName = db.Database.GetDbConnection().Database;
            var dataSource = db.Database.GetDbConnection().DataSource;
            Console.WriteLine($"📁 Connected to: {dataSource}");
            Console.WriteLine($"🗃️ Database: {dbName}");
            
            // Count existing todos
            var count = db.TodoItems.Count();
            Console.WriteLine($"📊 Found {count} todos in database");
            
            // List all todos
            var todos = db.TodoItems.ToList();
            foreach (var todo in todos)
            {
                Console.WriteLine($"   - ID: {todo.Id}, Title: {todo.Title}, Created: {todo.CreatedAt}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ ERROR: {ex.Message}");
        if (ex.InnerException != null)
            Console.WriteLine($"🔍 Inner: {ex.InnerException.Message}");
    }
}

Console.WriteLine("🚀 Starting API on http://localhost:5000");
Console.WriteLine("📚 Swagger available at: http://localhost:5000/swagger");
Console.WriteLine("📡 API endpoints available at: http://localhost:5000/api/todos");
Console.WriteLine("🌐 CORS configured for frontend: http://localhost:5500");

app.Run("http://localhost:5000");

public partial class Program { }