# 🚀 Running Guide - TodoListApp

This guide will walk you through running the TodoListApp locally on your machine.

---

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

### 1. **.NET 8 SDK**
- Download from: https://dotnet.microsoft.com/download/dotnet/8.0
- Verify installation:
  ```bash
  dotnet --version
  ```
  Should output: `8.0.x` or higher

### 2. **Git**
- Download from: https://git-scm.com/downloads
- Verify installation:
  ```bash
  git --version
  ```

### 3. **Code Editor** (Choose one)
- **Visual Studio 2022** (Community/Professional/Enterprise)
  - Download: https://visualstudio.microsoft.com/
- **Visual Studio Code**
  - Download: https://code.visualstudio.com/
  - Install C# extension: `ms-dotnettools.csharp`
- **JetBrains Rider** (Paid)

### 4. **Terminal/Command Line**
- Windows: Command Prompt, PowerShell, or Windows Terminal
- macOS/Linux: Terminal

---

## 📥 Step 1: Clone the Repository

Open your terminal and run:

```bash
git clone https://github.com/YOUR_USERNAME/TodoListApp.git
cd TodoListApp
```

**Note:** Replace `YOUR_USERNAME` with your actual GitHub username.

---

## 📂 Step 2: Navigate to Project Folder

Ensure you're in the root directory:

```bash
cd C:\Projects\TodoListApp
```

Or on macOS/Linux:

```bash
cd ~/Projects/TodoListApp
```

You should see the following structure:

```
TodoListApp/
├── src/
├── tests/
├── .github/
├── docs/
└── TodoListApp.sln
```

---

## 🔧 Step 3: Restore Dependencies

Restore all NuGet packages for the solution:

```bash
dotnet restore
```

**Expected Output:**
```
Determining projects to restore...
Restored TodoListApp.Api
Restored TodoListApp.Tests
```

---

## 🔨 Step 4: Build the Solution

Build the entire solution in Release mode:

```bash
dotnet build --configuration Release
```

**Expected Output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## ▶️ Step 5: Run the API

### Option A: Run from Solution Root

```bash
dotnet run --project src/TodoListApp.Api
```

### Option B: Navigate to API Project

```bash
cd src/TodoListApp.Api
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

## 🌐 Step 6: Access the API

### Swagger UI (Recommended for Testing)
Open your browser and navigate to:

```
https://localhost:7001/swagger
```

or

```
http://localhost:5001/swagger
```

You'll see an interactive API documentation page where you can:
- View all endpoints
- Test API calls directly
- See request/response examples

### Direct API Endpoints

Base URL: `https://localhost:7001/api/todos`

- **GET** `/api/todos` - Get all todos
- **GET** `/api/todos/{id}` - Get single todo
- **POST** `/api/todos` - Create todo
- **PUT** `/api/todos/{id}` - Update todo
- **DELETE** `/api/todos/{id}` - Delete todo

---

## 🧪 Step 7: Run Tests

### Run All Tests

From the solution root:

```bash
dotnet test
```

### Run Tests with Detailed Output

```bash
dotnet test --verbosity detailed
```

### Run Tests in Specific Project

```bash
dotnet test tests/TodoListApp.Tests/TodoListApp.Tests.csproj
```

**Expected Output:**
```
Passed!  - Failed:     0, Passed:    18, Skipped:     0, Total:    18
```

---

## 🛠️ Step 8: Testing with cURL

### Create a Todo

```bash
curl -X POST https://localhost:7001/api/todos \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Buy groceries\",\"description\":\"Milk, eggs, bread\",\"isCompleted\":false}"
```

### Get All Todos

```bash
curl https://localhost:7001/api/todos
```

### Get Single Todo

```bash
curl https://localhost:7001/api/todos/1
```

### Update Todo

```bash
curl -X PUT https://localhost:7001/api/todos/1 \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Buy groceries\",\"description\":\"Milk, eggs, bread, cheese\",\"isCompleted\":true}"
```

### Delete Todo

```bash
curl -X DELETE https://localhost:7001/api/todos/1
```

---

## 📊 API Request/Response Examples

### POST /api/todos (Create)

**Request:**
```json
{
  "title": "Complete project documentation",
  "description": "Write README and guides",
  "isCompleted": false
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "title": "Complete project documentation",
  "description": "Write README and guides",
  "isCompleted": false,
  "createdAt": "2026-02-06T10:30:00Z",
  "completedAt": null
}
```

### GET /api/todos (Get All)

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "title": "Complete project documentation",
    "description": "Write README and guides",
    "isCompleted": false,
    "createdAt": "2026-02-06T10:30:00Z",
    "completedAt": null
  },
  {
    "id": 2,
    "title": "Review code",
    "description": "Check for bugs",
    "isCompleted": true,
    "createdAt": "2026-02-06T11:00:00Z",
    "completedAt": "2026-02-06T12:00:00Z"
  }
]
```

### GET /api/todos/1 (Get Single)

**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Complete project documentation",
  "description": "Write README and guides",
  "isCompleted": false,
  "createdAt": "2026-02-06T10:30:00Z",
  "completedAt": null
}
```

**Response (404 Not Found):**
```json
{
  "message": "Todo with ID 999 not found"
}
```

### PUT /api/todos/1 (Update)

**Request:**
```json
{
  "title": "Complete project documentation",
  "description": "Write README, guides, and examples",
  "isCompleted": true
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "title": "Complete project documentation",
  "description": "Write README, guides, and examples",
  "isCompleted": true,
  "createdAt": "2026-02-06T10:30:00Z",
  "completedAt": "2026-02-06T13:45:00Z"
}
```

### DELETE /api/todos/1 (Delete)

**Response (204 No Content):**
No body returned

**Response (404 Not Found):**
```json
{
  "message": "Todo with ID 999 not found"
}
```

---

## 🛑 Stopping the Application

Press `Ctrl + C` in the terminal where the app is running.

---

## 🔄 Quick Command Reference

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run API
dotnet run --project src/TodoListApp.Api

# Run tests
dotnet test

# Clean build artifacts
dotnet clean

# Build and run in one command
dotnet build && dotnet run --project src/TodoListApp.Api
```

---

## 📱 IDE-Specific Instructions

### Visual Studio 2022

1. Open `TodoListApp.sln`
2. Set `TodoListApp.Api` as startup project (right-click → Set as Startup Project)
3. Press `F5` to run with debugging or `Ctrl+F5` to run without debugging
4. Run tests: Test Explorer → Run All

### Visual Studio Code

1. Open folder: `File → Open Folder → TodoListApp`
2. Open terminal: `Terminal → New Terminal`
3. Run: `dotnet run --project src/TodoListApp.Api`
4. Run tests: `dotnet test`

### JetBrains Rider

1. Open `TodoListApp.sln`
2. Right-click `TodoListApp.Api` → Run
3. Run tests: Right-click test project → Run Unit Tests

---

## ✅ Verification Checklist

- [ ] .NET 8 SDK installed
- [ ] Repository cloned
- [ ] Dependencies restored (`dotnet restore`)
- [ ] Solution builds successfully (`dotnet build`)
- [ ] API runs without errors (`dotnet run`)
- [ ] Swagger UI accessible in browser
- [ ] All tests pass (`dotnet test`)
- [ ] API endpoints respond correctly

---

## 🆘 Need Help?

If you encounter issues:
1. Check `POSSIBLE_ERRORS.md` for common problems and solutions
2. Verify all prerequisites are installed correctly
3. Ensure you're in the correct directory
4. Check that ports 5001 and 7001 are not in use

---

## 🎉 Success!

If everything works, you should see:
- ✅ Solution builds without errors
- ✅ All 18 tests pass
- ✅ API running on https://localhost:7001
- ✅ Swagger UI accessible

Happy coding! 🚀
