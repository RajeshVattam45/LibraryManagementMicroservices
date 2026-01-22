using HostelService.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost ( IWebHostBuilder builder )
    {
        builder.UseEnvironment ( "Test" );

        builder.ConfigureServices ( services =>
        {
            // Remove SQL Server DbContext
            var descriptor = services.SingleOrDefault (
                d => d.ServiceType == typeof ( DbContextOptions<HostelDbContext> )
            );

            if (descriptor != null)
                services.Remove ( descriptor );

            // Add InMemory DB
            services.AddDbContext<HostelDbContext> ( options =>
            {
                options.UseInMemoryDatabase ( "HostelService_TestDb" );
            } );
        } );
    }
}
