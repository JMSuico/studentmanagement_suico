using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using StudentManagement.Components;
using StudentManagement.Features.Data;
using StudentManagement.Features.Helpers;
using StudentManagement.Features.Repositories.Implementations;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Implementations;
using StudentManagement.Features.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Data Protection to avoid KeyRing errors
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")));

// MudBlazor Configuration
builder.Services.AddMudServices();

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Hardcoding ServerVersion avoids connection attempts during DI resolution
    var serverVersion = ServerVersion.Parse("8.0.30-mysql");
    options.UseMySql(connectionString, serverVersion)
           // .LogTo(Console.WriteLine, LogLevel.Information)
           // .EnableSensitiveDataLogging()
           .EnableDetailedErrors();
});

// Authentication Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
}).AddCookie("Cookies", options =>
{
    options.LoginPath = "/login";
});
builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IClassSectionRepository, ClassSectionRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IAttendanceSessionRepository, AttendanceSessionRepository>();

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IClassSectionService, ClassSectionService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IAttendanceSessionService, AttendanceSessionService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IAcademicProgressService, AcademicProgressService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Seed Default Admin User
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        var userService = services.GetRequiredService<IUserService>();
        if (!context.Users.Any())
        {
            var adminUser = new StudentManagement.Features.Data.Models.User
            {
                Username = "admin",
                Email = "admin@system.com",
                FirstName = "System",
                LastName = "Administrator",
                Role = StudentManagement.Features.Data.Enums.UserRole.Admin,
                IsActive = true,
                PasswordHash = ""
            };
            userService.RegisterAsync(adminUser, "admin123").Wait();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

// API auth endpoints — MUST be mapped BEFORE MapRazorComponents
// so the routing middleware matches them before Blazor's catch-all route handler.
app.MapPost("/api/auth/login", async (Microsoft.AspNetCore.Http.HttpContext context, [Microsoft.AspNetCore.Mvc.FromForm] string username, [Microsoft.AspNetCore.Mvc.FromForm] string password, IUserService userService) =>
{
    var user = await userService.AuthenticateAsync(username, password);
    if (user != null)
    {
        var claims = new System.Collections.Generic.List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim("FullName", $"{user.FirstName} {user.LastName}")
        };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "Cookies");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            IsPersistent = true
        };
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(context, "Cookies", principal, authProperties);
        return Microsoft.AspNetCore.Http.Results.Redirect("/dashboard");
    }
    return Microsoft.AspNetCore.Http.Results.Redirect("/login?error=InvalidCredentials");
}).DisableAntiforgery();

app.MapGet("/api/auth/logout", async (Microsoft.AspNetCore.Http.HttpContext context) =>
{
    await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(context, "Cookies");
    return Microsoft.AspNetCore.Http.Results.Redirect("/login");
}).DisableAntiforgery();

// Blazor component routing — AFTER API endpoints so they don't get swallowed
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
