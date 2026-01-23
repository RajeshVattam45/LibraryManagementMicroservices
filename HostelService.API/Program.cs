using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HostelService.Infrastructure.Data;
using HostelService.Application.Services;
using HostelService.Domain.Interfaces;
using HostelService.Infrastructure.Repositories;
using HostelService.Application.Adapter;
using HostelService.Application.Mediators;

var builder = WebApplication.CreateBuilder ( args );

// --------------------
// Load configuration
// --------------------
builder.Configuration
    .AddJsonFile ( "appsettings.json", optional: false, reloadOnChange: true )
    .AddJsonFile ( $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true )
    .AddEnvironmentVariables ();

// --------------------
// Database
// --------------------
var connectionString = builder.Configuration.GetConnectionString ( "DefaultConnection" );

builder.Services.AddDbContext<HostelDbContext> ( options =>
    options.UseSqlServer (
        connectionString,
        sql =>
        {
            sql.MigrationsAssembly ( "HostelService.Infrastructure" );
            sql.EnableRetryOnFailure (
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds ( 30 ),
                errorNumbersToAdd: null
            );
        } ) );

// --------------------
// Dependency Injection
// --------------------
builder.Services.AddScoped<IHostelAppService, HostelAppService> ();
builder.Services.AddScoped<IHostelRepository, HostelRepository> ();

builder.Services.AddScoped<IRoomAppService, RoomAppService> ();
builder.Services.AddScoped<IRoomRepository, RoomRepository> ();

builder.Services.AddScoped<IHostelStudentRepository, HostelStudentRepository> ();
builder.Services.AddScoped<IHostelStudentAppService, HostelStudentAppService> ();

builder.Services.AddScoped<IHostelAdapter, HostelAdapter> ();
builder.Services.AddScoped<IStudentAssignmentMediator, StudentAssignmentMediator> ();

builder.Services.AddScoped ( typeof ( IGenericRepository<> ), typeof ( GenericRepository<> ) );

// HTTP client
var studentApiBaseUrl = builder.Configuration["StudentService:BaseUrl"];
builder.Services.AddHttpClient<IStudentValidationService, StudentValidationService> ( client =>
{
    client.BaseAddress = new Uri ( studentApiBaseUrl );
} );

// --------------------
// MVC + Swagger
// --------------------
builder.Services.AddControllers ();
builder.Services.AddMemoryCache ();
builder.Services.AddEndpointsApiExplorer ();

builder.Services.AddSwaggerGen ( c =>
{
    c.SwaggerDoc ( "v1", new OpenApiInfo
    {
        Title = "Hostel Service API",
        Version = "v1"
    } );
} );

var app = builder.Build ();

// --------------------
// Middleware Pipeline (ORDER MATTERS)
// --------------------
app.UseHttpsRedirection ();

app.UseSwagger ();
app.UseSwaggerUI ( c =>
{
    c.SwaggerEndpoint ( "/swagger/v1/swagger.json", "Hostel Service API v1" );
    c.RoutePrefix = "swagger";
} );

// Health check
app.MapGet ( "/health", ( ) => Results.Ok ( "Healthy" ) );

// Redirect root to Swagger
app.MapGet ( "/", context =>
{
    context.Response.Redirect ( "/swagger" );
    return Task.CompletedTask;
} );

// Controllers (ONLY ONCE)
app.MapControllers ();

// --------------------
// Optional EF Migrations
// --------------------
var applyMigrationsEnv = builder.Configuration["APPLY_MIGRATIONS"];
var applyMigrations = !string.IsNullOrEmpty ( applyMigrationsEnv )
    ? bool.TryParse ( applyMigrationsEnv, out var result ) && result
    : app.Environment.IsDevelopment ();

var isEfDesignTime = AppDomain.CurrentDomain
    .GetAssemblies ()
    .Any ( a => a.FullName!.StartsWith ( "Microsoft.EntityFrameworkCore.Design" ) );

if (applyMigrations && !isEfDesignTime)
{
    using var scope = app.Services.CreateScope ();
    var db = scope.ServiceProvider.GetRequiredService<HostelDbContext> ();
    db.Database.Migrate ();
}

app.Run ();

public partial class Program { }
