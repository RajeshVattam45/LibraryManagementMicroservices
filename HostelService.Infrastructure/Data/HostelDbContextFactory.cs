using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelService.Infrastructure.Data
{
    public class HostelDbContextFactory : IDesignTimeDbContextFactory<HostelDbContext>
    {
        public HostelDbContext CreateDbContext ( string[] args )
        {
            // Check if Update-Database or Add-Migration provided a connection string
            var customArg = args.FirstOrDefault ( a => a.StartsWith ( "--connection=" ) );
            string connectionString;

            if (!string.IsNullOrWhiteSpace ( customArg ))
            {
                connectionString = customArg.Replace ( "--connection=", "" ).Trim ( '"' );
            }
            else
            {
                // fallback if no args given
                connectionString = "Server=localhost,1433;Database=HostelManagement;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;";
            }

            var builder = new DbContextOptionsBuilder<HostelDbContext> ();
            builder.UseSqlServer ( connectionString );

            return new HostelDbContext ( builder.Options );
        }
    }
}
