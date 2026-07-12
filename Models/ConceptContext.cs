using Microsoft.EntityFrameworkCore;

namespace ConceptAPI.Models;

public class ConceptContext : DbContext
{
    public ConceptContext(DbContextOptions<ConceptContext> options)
        : base(options)
    {
    }

    public DbSet<Concept> Concepts { get; set; }
    public DbSet<ConceptOwnership> ConceptOwnerships { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.GoogleId)
            .IsUnique();
    }
}