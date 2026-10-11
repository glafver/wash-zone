using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WashZone.Data
{
    /// <summary>
    /// Design-time factory so `dotnet ef` commands can build the DbContext without
    /// running Program.cs (which would try to connect and migrate the database).
    /// The connection string is only used when a command needs a live database
    /// (e.g. `database update`); `migrations add` does not connect.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Must match the runtime configuration in Program.cs so generated
            // migrations use the same column types (`timestamp without time zone`).
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var connectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? "Host=localhost;Port=5432;Database=washzone;Username=postgres;Password=postgres";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
