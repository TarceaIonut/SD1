using Microsoft.EntityFrameworkCore;

namespace Account.Serivice.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Accounts> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Accounts>()
            .Ignore(p => p.person);
    }
}