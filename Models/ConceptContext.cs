using Microsoft.EntityFrameworkCore;

namespace ConceptAPI.Models;

public class ConceptContext : DbContext
{
    public ConceptContext(DbContextOptions<ConceptContext> options)
        : base(options)
    {
    }

    public DbSet<Concept> Concepts { get; set; }
}