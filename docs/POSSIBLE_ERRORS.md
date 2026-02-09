# 🐛 Possible Errors and Solutions

This guide covers common errors you might encounter and how to fix them.

---

## 📋 Table of Contents

1. [.NET SDK Errors](#net-sdk-errors)
2. [Build Errors](#build-errors)
3. [Runtime Errors](#runtime-errors)
4. [Test Errors](#test-errors)
5. [GitHub Actions CI Errors](#github-actions-ci-errors)
6. [Port Errors](#port-errors)
7. [Project File Errors](#project-file-errors)

---

## 1. .NET SDK Errors

### Error: `The command 'dotnet' is not recognized`

**Full Error Message**:
```
'dotnet' is not recognized as an internal or external command,
operable program or batch file.
```

**Cause**: .NET SDK is not installed or not in PATH.

**Solution**:

1. **Install .NET 8 SDK**:
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   - Run the installer
   - Restart terminal/command prompt

2. **Verify Installation**:
   ```bash
   dotnet --version
   ```
   Should output: `8.0.x`

3. **If Still Not Working** (Windows):
   - Add to PATH manually:
     - Search "Environment Variables" in Windows
     - Edit System Variables → PATH
     - Add: `C:\Program Files\dotnet\`
   - Restart terminal

---

### Error: `The current .NET SDK does not support targeting .NET 8.0`

**Full Error Message**:
```
error NETSDK1045: The current .NET SDK does not support targeting .NET 8.0.
Either target .NET 7.0 or lower, or use a version of the .NET SDK that supports .NET 8.0.
```

**Cause**: Older .NET SDK version installed.

**Solution**:

1. **Check Current Version**:
   ```bash
   dotnet --version
   ```

2. **Uninstall Old SDKs** (Windows):
   - Go to Settings → Apps
   - Search "Microsoft .NET SDK"
   - Uninstall versions older than 8.0

3. **Install .NET 8 SDK**:
   - Download from: https://dotnet.microsoft.com/download/dotnet/8.0

4. **Verify**:
   ```bash
   dotnet --list-sdks
   ```
   Should show `8.0.x` in the list

---

## 2. Build Errors

### Error: `MSB1011: Specify which project or solution file to use`

**Full Error Message**:
```
MSB1011: Specify which project or solution file to use because this folder contains
more than one project or solution file.
```

**Cause**: Multiple `.sln` or `.csproj` files in directory, or running command from wrong folder.

**Solution**:

**Option 1 - Specify Solution File**:
```bash
dotnet build TodoListApp.sln
```

**Option 2 - Navigate to Root**:
```bash
cd C:\Projects\TodoListApp
dotnet build
```

**Option 3 - Specify Full Path**:
```bash
dotnet build "C:\Projects\TodoListApp\TodoListApp.sln"
```

---

### Error: `The project file could not be found`

**Full Error Message**:
```
MSBUILD : error MSB1009: Project file does not exist.
Switch: TodoListApp.Api.csproj
```

**Cause**: Wrong directory or incorrect path.

**Solution**:

1. **Check Current Directory**:
   ```bash
   pwd  # Linux/Mac
   cd   # Windows
   ```

2. **Navigate to Correct Location**:
   ```bash
   cd C:\Projects\TodoListApp
   ```

3. **Verify Structure**:
   ```bash
   dir  # Windows
   ls   # Linux/Mac
   ```
   Should show `TodoListApp.sln`

4. **Use Correct Relative Path**:
   ```bash
   dotnet build src/TodoListApp.Api/TodoListApp.Api.csproj
   ```

---

### Error: `The type or namespace name could not be found`

**Full Error Message**:
```
error CS0246: The type or namespace name 'TodoItem' could not be found
(are you missing a using directive or an assembly reference?)
```

**Cause**: Missing `using` statement or project reference.

**Solution**:

1. **Add Using Statement**:
   ```csharp
   using TodoListApp.Api.Models;
   using TodoListApp.Api.Services;
   ```

2. **Check Project References** in `.csproj`:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\..\src\TodoListApp.Api\TodoListApp.Api.csproj" />
   </ItemGroup>
   ```

3. **Restore Packages**:
   ```bash
   dotnet restore
   dotnet build
   ```

---

### Error: `Package restore failed`

**Full Error Message**:
```
error NU1101: Unable to find package Swashbuckle.AspNetCore.
No packages exist with this id in source(s): nuget.org
```

**Cause**: Network issues or NuGet configuration problems.

**Solution**:

1. **Check Internet Connection**

2. **Clear NuGet Cache**:
   ```bash
   dotnet nuget locals all --clear
   ```

3. **Restore Again**:
   ```bash
   dotnet restore --force
   ```

4. **Check NuGet Sources**:
   ```bash
   dotnet nuget list source
   ```
   Should include `https://api.nuget.org/v3/index.json`

5. **Add NuGet Source** (if missing):
   ```bash
   dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
   ```

---

## 3. Runtime Errors

### Error: `Unable to bind to https://localhost:7001`

**Full Error Message**:
```
Unhandled exception. System.IO.IOException: Failed to bind to address
https://localhost:7001: address already in use.
```

**Cause**: Port is already in use by another application.

**Solution**:

**Option 1 - Kill Process Using Port** (Windows):
```powershell
netstat -ano | findstr :7001
taskkill /PID <PID> /F
```

**Option 1 - Kill Process** (Linux/Mac):
```bash
lsof -i :7001
kill -9 <PID>
```

**Option 2 - Change Port**:

Edit `src/TodoListApp.Api/Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7002;http://localhost:5002"
}
```

Or use command line:
```bash
dotnet run --project src/TodoListApp.Api --urls "https://localhost:7002"
```

**Option 3 - Stop Other Instances**:
- Check Task Manager for other `dotnet.exe` processes
- Close Visual Studio if running the project

---

### Error: `Missing required parameter 'todoService'`

**Full Error Message**:
```
System.InvalidOperationException: Unable to resolve service for type
'TodoListApp.Api.Services.ITodoService' while attempting to activate
'TodoListApp.Api.Controllers.TodosController'.
```

**Cause**: Service not registered in `Program.cs`.

**Solution**:

Check `Program.cs` contains:
```csharp
builder.Services.AddSingleton<ITodoService, TodoService>();
```

If missing, add it before `var app = builder.Build();`

---

### Error: `No route matches the supplied values`

**Full Error Message**:
```
InvalidOperationException: No route matches the supplied values.
```

**Cause**: Incorrect routing configuration.

**Solution**:

1. **Verify Controller Route Attribute**:
   ```csharp
   [Route("api/[controller]")]
   ```

2. **Check Program.cs**:
   ```csharp
   app.MapControllers();
   ```

3. **Verify HTTP Method Attributes**:
   ```csharp
   [HttpGet]
   [HttpPost]
   [HttpPut("{id}")]
   [HttpDelete("{id}")]
   ```

---

## 4. Test Errors

### Error: `No tests found`

**Full Error Message**:
```
Test run for TodoListApp.Tests.dll (.NETCoreApp,Version=v8.0)
No test is available in TodoListApp.Tests.dll
```

**Cause**: Tests not properly marked with `[Fact]` attribute or test project not built.

**Solution**:

1. **Rebuild Test Project**:
   ```bash
   dotnet build tests/TodoListApp.Tests/TodoListApp.Tests.csproj
   ```

2. **Check Test Class**:
   ```csharp
   using Xunit;  // Must have this
   
   public class TodoServiceTests  // Must be public
   {
       [Fact]  // Must have this attribute
       public void TestMethod()
       {
           // Test code
       }
   }
   ```

3. **Verify xUnit Package**:
   Check `TodoListApp.Tests.csproj` contains:
   ```xml
   <PackageReference Include="xunit" Version="2.6.2" />
   ```

---

### Error: `Test failed: FluentAssertions not found`

**Full Error Message**:
```
error CS0246: The type or namespace name 'FluentAssertions' could not be found
```

**Cause**: FluentAssertions package not installed.

**Solution**:

1. **Verify Package Reference** in `TodoListApp.Tests.csproj`:
   ```xml
   <PackageReference Include="FluentAssertions" Version="6.12.0" />
   ```

2. **Restore Packages**:
   ```bash
   dotnet restore
   ```

3. **Add Using Statement** in test file:
   ```csharp
   using FluentAssertions;
   ```

---

### Error: `Test failed: Object reference not set to an instance`

**Full Error Message**:
```
System.NullReferenceException: Object reference not set to an instance of an object.
```

**Cause**: Service or controller not initialized in test constructor.

**Solution**:

Check test class constructor:
```csharp
public class TodoServiceTests
{
    private readonly ITodoService _todoService;
    
    public TodoServiceTests()
    {
        _todoService = new TodoService();  // Must initialize
    }
}
```

---

## 5. GitHub Actions CI Errors

### Error: `Solution file not found`

**Error in CI Log**:
```
Error: Could not find file 'TodoListApp.sln'
```

**Cause**: Incorrect path in workflow file or solution file not committed.

**Solution**:

1. **Check File Exists**:
   ```bash
   git ls-files | grep TodoListApp.sln
   ```

2. **If Missing, Add and Commit**:
   ```bash
   git add TodoListApp.sln
   git commit -m "Add solution file"
   git push
   ```

3. **Verify Workflow Path** in `.github/workflows/ci.yml`:
   ```yaml
   - name: Restore dependencies
     run: dotnet restore TodoListApp.sln
   ```

---

### Error: `Tests failed in CI`

**Error in CI Log**:
```
Error: One or more tests failed
```

**Cause**: Tests pass locally but fail in CI due to environment differences.

**Solution**:

1. **Check CI Logs** for specific failing test

2. **Common Issues**:
   - **Time-sensitive tests**: Use `TimeSpan.FromSeconds(5)` instead of `TimeSpan.FromMilliseconds(100)`
   - **File paths**: Use relative paths, not absolute
   - **Culture-specific**: Use `CultureInfo.InvariantCulture` for dates

3. **Run Tests Locally First**:
   ```bash
   dotnet test --configuration Release
   ```

4. **Check for Flaky Tests**: Run multiple times
   ```bash
   dotnet test --logger "console;verbosity=detailed"
   ```

---

### Error: `.NET SDK not found in CI`

**Error in CI Log**:
```
Error: The specified version of .NET SDK was not found
```

**Cause**: Incorrect SDK version in workflow.

**Solution**:

Check `.github/workflows/ci.yml`:
```yaml
- name: Setup .NET 8
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '8.0.x'  # Must match project target framework
```

---

### Error: `Build failed: CS0246 type not found`

**Error in CI Log**:
```
error CS0246: The type or namespace name 'TodoItem' could not be found
```

**Cause**: Project references broken or restore failed.

**Solution**:

1. **Verify Project References** in `.csproj` use **relative paths**:
   ```xml
   <ProjectReference Include="..\..\src\TodoListApp.Api\TodoListApp.Api.csproj" />
   ```

2. **Add Restore Step** before build in workflow:
   ```yaml
   - name: Restore dependencies
     run: dotnet restore TodoListApp.sln
     
   - name: Build
     run: dotnet build TodoListApp.sln --no-restore
   ```

---

## 6. Port Errors

### Error: `Connection refused`

**Full Error Message**:
```
System.Net.Http.HttpRequestException: Connection refused
```

**Cause**: Application not running or wrong port.

**Solution**:

1. **Start Application**:
   ```bash
   dotnet run --project src/TodoListApp.Api
   ```

2. **Check Correct Port**:
   Look for console output:
   ```
   Now listening on: https://localhost:7001
   ```

3. **Use Correct URL**:
   ```bash
   curl https://localhost:7001/api/todos
   ```

---

### Error: `SSL certificate problem`

**Full Error Message**:
```
curl: (60) SSL certificate problem: unable to get local issuer certificate
```

**Cause**: Development HTTPS certificate not trusted.

**Solution**:

1. **Trust Development Certificate**:
   ```bash
   dotnet dev-certs https --trust
   ```

2. **Use HTTP Instead** (for testing):
   ```bash
   curl http://localhost:5001/api/todos
   ```

3. **Skip SSL Verification** (curl, not recommended):
   ```bash
   curl -k https://localhost:7001/api/todos
   ```

---

## 7. Project File Errors

### Error: `Invalid project file format`

**Full Error Message**:
```
error MSB4025: The project file could not be loaded. Invalid character in the given encoding.
```

**Cause**: Corrupted or incorrectly formatted `.csproj` file.

**Solution**:

1. **Check File Encoding**: Must be UTF-8

2. **Validate XML Structure**:
   ```xml
   <Project Sdk="Microsoft.NET.Sdk.Web">
     <PropertyGroup>
       <TargetFramework>net8.0</TargetFramework>
     </PropertyGroup>
   </Project>
   ```

3. **Compare with Working Template**: Use the provided `.csproj` files from this project

---

### Error: `Duplicate project GUIDs`

**Full Error Message**:
```
error MSB4025: The project GUID '{...}' is duplicated
```

**Cause**: Multiple projects with same GUID in solution file.

**Solution**:

1. **Regenerate Solution File**:
   ```bash
   dotnet new sln -n TodoListApp --force
   dotnet sln add src/TodoListApp.Api/TodoListApp.Api.csproj
   dotnet sln add tests/TodoListApp.Tests/TodoListApp.Tests.csproj
   ```

2. **Or Use Provided Solution File**: Copy from this project

---

## 🔧 General Troubleshooting Steps

### When Nothing Works:

1. **Clean Everything**:
   ```bash
   dotnet clean
   dotnet nuget locals all --clear
   ```

2. **Delete bin and obj Folders**:
   ```bash
   # Windows PowerShell
   Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force
   
   # Linux/Mac
   find . -name "bin" -o -name "obj" | xargs rm -rf
   ```

3. **Restore and Rebuild**:
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Restart IDE**: Close and reopen Visual Studio/VS Code/Rider

5. **Restart Computer**: Sometimes necessary after SDK install

---

## 📞 Getting Help

If you're still stuck:

1. **Check Error Message Carefully**: Often contains the solution
2. **Search Error Code**: Google the exact error message
3. **Check .NET Documentation**: https://learn.microsoft.com/dotnet
4. **Stack Overflow**: Search for your specific error
5. **GitHub Issues**: Check this repository's Issues tab

---

## ✅ Quick Verification Checklist

Use this to verify everything is set up correctly:

```bash
# 1. Check .NET version
dotnet --version  # Should be 8.0.x

# 2. Check you're in correct directory
pwd  # Should end with TodoListApp

# 3. List files
ls  # Should show TodoListApp.sln

# 4. Restore packages
dotnet restore

# 5. Build solution
dotnet build

# 6. Run tests
dotnet test

# 7. Run application
dotnet run --project src/TodoListApp.Api
```

If all of these work without errors, your environment is correctly set up! ✅

---

## 🆘 Still Having Issues?

Create an issue on GitHub with:
- **Error message** (full text)
- **Steps to reproduce**
- **Your environment**:
  - OS: Windows/Mac/Linux
  - .NET version: `dotnet --version`
  - IDE: Visual Studio/VS Code/Rider

We'll help you resolve it! 🚀
