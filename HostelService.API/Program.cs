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
// Load configuration in correct order
// --------------------
builder.Configuration
    .AddJsonFile ( "appsettings.json", optional: false, reloadOnChange: true )
    .AddJsonFile ( $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true )
    .AddEnvironmentVariables ();

// --------------------
// Database Connection
// --------------------
var connectionString = builder.Configuration.GetConnectionString ( "DefaultConnection" );

builder.Services.AddDbContext<HostelDbContext> ( options =>
    options.UseSqlServer (
        connectionString,
        sql =>
        {
            sql.MigrationsAssembly ( "HostelService.Infrastructure" );
            sql.EnableRetryOnFailure ();
        } )
);


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

//builder.Services.AddHttpClient<IStudentValidationService, StudentValidationService> ();
builder.Services.AddHttpClient<IStudentValidationService, StudentValidationService> ( client =>
{
    client.BaseAddress = new Uri ( "https://host.docker.internal:7230/" );
} );


// --------------------
// MVC + Swagger
// --------------------
builder.Services.AddControllers ();
builder.Services.AddMemoryCache ();
builder.Services.AddEndpointsApiExplorer ();

builder.Services.AddSwaggerGen ( c =>
{
    c.SwaggerDoc ( "v1", new OpenApiInfo { Title = "Hostel Service API", Version = "v1" } );
} );

var app = builder.Build ();

// --------------------
// Middleware Pipeline
// --------------------
if (app.Environment.IsDevelopment ())
{
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

app.UseHttpsRedirection ();
app.MapControllers ();

var applyMigrationsEnv = builder.Configuration["APPLY_MIGRATIONS"];
var applyMigrations = false;

// If env var is explicitly set to "true", honor it.
// Otherwise allow auto-run only in Development.
if (!string.IsNullOrEmpty ( applyMigrationsEnv ))
{
    bool.TryParse ( applyMigrationsEnv, out applyMigrations );
}
else
{
    applyMigrations = app.Environment.IsDevelopment ();
}

var isEfDesignTime = AppDomain.CurrentDomain
    .GetAssemblies ()
    .Any ( a => a.FullName!.StartsWith ( "Microsoft.EntityFrameworkCore.Design" ) );

if (applyMigrations && !isEfDesignTime)
{
    using var scope = app.Services.CreateScope ();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>> ();
    var db = services.GetRequiredService<HostelDbContext> ();

    db.Database.Migrate ();
}


app.Run ();
public partial class Program { }
