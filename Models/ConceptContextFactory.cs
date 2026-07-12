using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ConceptAPI.Models;

// Used only by `dotnet ef` design-time commands (migrations add/remove, database update).
// Program.cs's real startup (Jwt:Key / Google:ClientId checks) is intentionally bypassed
// here - EF tooling just needs to know the provider, not run the whole app.
public class ConceptContextFactory : IDesignTimeDbContextFactory<ConceptContext>
{
    public ConceptContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ConceptContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ConceptContext(optionsBuilder.Options);
    }
}
