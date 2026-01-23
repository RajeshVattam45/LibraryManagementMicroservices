using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Interfaces;
using BookCatalogService.Infrastructure.Data;
using BookCatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder ( args );

// --------------------
// Configuration (important for Azure)
// --------------------
builder.Configuration
    .AddJsonFile ( "appsettings.json", optional: false )
    .AddJsonFile ( $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true )
    .AddEnvironmentVariables ();

// --------------------
// Add DbContext
// --------------------
builder.Services.AddDbContext<BookCatalogDbContext> ( options =>
{
    options.UseSqlServer (
        builder.Configuration.GetConnectionString ( "DefaultConnection" ),
        sql =>
        {
            sql.EnableRetryOnFailure (
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds ( 30 ),
                errorNumbersToAdd: null
            );
        } );
} );


// --------------------
// Register Repositories & Services
// --------------------
builder.Services.AddScoped<IBookRepository, BookRepository> ();
builder.Services.AddScoped<IBookAppService, BookAppService> ();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository> ();
builder.Services.AddScoped<IAuthorAppService, AuthorAppService> ();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository> ();
builder.Services.AddScoped<ICategoryAppService, CategoryAppService> ();

builder.Services.AddScoped<IPublisherRepository, PublisherRepository> ();
builder.Services.AddScoped<IPublisherAppService, PublisherAppService> ();

// --------------------
// API + Swagger
// --------------------
builder.Services.AddControllers ();
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();

var app = builder.Build ();

// --------------------
// Middleware Pipeline
// --------------------
app.UseSwagger ();
app.UseSwaggerUI ( c =>
{
    c.SwaggerEndpoint ( "/swagger/v1/swagger.json", "Book Service API v1" );
} );

app.MapControllers ();

// Health check
app.MapGet ( "/health", ( ) => Results.Ok ( "Healthy" ) );

// Redirect root to Swagger
app.MapGet ( "/", context =>
{
    context.Response.Redirect ( "/swagger" );
    return Task.CompletedTask;
} );

app.UseHttpsRedirection ();
app.UseAuthorization ();
app.MapControllers ();

app.Run ();

// Needed for EF tools
public partial class Program { }
