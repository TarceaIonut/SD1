using Microsoft.EntityFrameworkCore;

namespace Account.Serivice.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Person> Persons { get; set; }

    public Person? GetPersonById(int id) {
        return Persons.FirstOrDefault(p => p.Id == id);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Person>().HasDiscriminator(p => p.Role)
            .HasValue<Admin>(Person.UserRole.Admin)
            .HasValue<Patient>(Person.UserRole.Patient)
            .HasValue<Doctor>(Person.UserRole.Doctor);
    }
}