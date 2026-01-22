using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HostelService.Infrastructure.Data
{
    public class HostelDbContextFactory
        : IDesignTimeDbContextFactory<HostelDbContext>
    {
        public HostelDbContext CreateDbContext ( string[] args )
        {
            var basePath = Directory.GetCurrentDirectory ();

            var configuration = new ConfigurationBuilder ()
                .SetBasePath ( basePath )
                .AddJsonFile ( "appsettings.json", optional: false )
                .AddEnvironmentVariables ()
                .Build ();

            var connectionString =
                configuration.GetConnectionString ( "DefaultConnection" );

            var optionsBuilder = new DbContextOptionsBuilder<HostelDbContext> ();
            optionsBuilder.UseSqlServer (
                connectionString,
                sql =>
                {
                    sql.MigrationsAssembly ( "HostelService.Infrastructure" );
                    sql.EnableRetryOnFailure ();
                } );

            return new HostelDbContext ( optionsBuilder.Options );
        }
    }
}
