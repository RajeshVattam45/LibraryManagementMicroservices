using BookCatalogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder ( args );

// --------------------
// Add DbContext
// --------------------
builder.Services.AddDbContext<BookCatalogDbContext> ( options =>
{
    options.UseSqlServer (
        builder.Configuration.GetConnectionString ( "DefaultConnection" ) );
} );

// Add services to the container.
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
    db.Database.Migrate ();
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
