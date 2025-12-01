using BookCatalogService.Application.Services;
using BookCatalogService.Domain.Interfaces;
using BookCatalogService.Infrastructure.Data;
using BookCatalogService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder ( args );

// --------------------
// Add DbContext
// --------------------
builder.Services.AddDbContext<BookCatalogDbContext> ( options =>
{
    options.UseSqlServer ( builder.Configuration.GetConnectionString ( "DefaultConnection" ) );
} );

// Register Repositories & Services
builder.Services.AddScoped<IBookRepository, BookRepository> ();
builder.Services.AddScoped<IBookAppService, BookAppService> ();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository> ();
builder.Services.AddScoped<IAuthorAppService, AuthorAppService> ();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository> ();
builder.Services.AddScoped<ICategoryAppService, CategoryAppService> ();

builder.Services.AddScoped<IPublisherRepository, PublisherRepository> ();
builder.Services.AddScoped<IPublisherAppService, PublisherAppService> ();


// API + Swagger
builder.Services.AddControllers ();
builder.Services.AddEndpointsApiExplorer ();
builder.Services.AddSwaggerGen ();

var app = builder.Build ();

// ---------------------------------
// ⭐ Apply EF Core Migrations here
// ---------------------------------
using (var scope = app.Services.CreateScope ())
{
    var db = scope.ServiceProvider.GetRequiredService<BookCatalogDbContext> ();
    try
    {
        db.Database.Migrate (); // runs only pending migrations
    }
    catch (Exception ex)
    {
        Console.WriteLine ( "❌ Migration error: " + ex.Message );
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment ())
{
    app.UseSwagger ();
    app.UseSwaggerUI ();
}

app.UseHttpsRedirection ();
app.UseAuthorization ();
app.MapControllers ();
app.Run ();
