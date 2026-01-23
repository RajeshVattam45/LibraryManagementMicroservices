using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Interfaces;
using BookCatalogService.Infrastructure.Data;
using BookCatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder ( args );

// --------------------
// Configuration
// --------------------
builder.Configuration
    .AddJsonFile ( "appsettings.json", optional: false )
    .AddJsonFile ( $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true )
    .AddEnvironmentVariables ();

// --------------------
// Database
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
// Dependency Injection
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
// MVC + Swagger
// --------------------
builder.Services.AddControllers ();
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();

var app = builder.Build ();

// --------------------
// Middleware Pipeline (ORDER MATTERS)
// --------------------
app.UseHttpsRedirection ();

app.UseSwagger ();
app.UseSwaggerUI ( c =>
{
    c.SwaggerEndpoint ( "/swagger/v1/swagger.json", "Book Service API v1" );
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

app.Run ();

public partial class Program { }
