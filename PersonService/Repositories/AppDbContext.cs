using Microsoft.EntityFrameworkCore;

namespace Account.Serivice.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Person> Persons { get; set; }

    public Person? GetPersonById(int id) {
        return Persons.FirstOrDefault(p => p.Id == id);
    }
    public bool PersonExistsByEmail(string email) {
        return Persons.Any(p => p.Email == email);
    }

    public int NewPerson(Person p) {
        var new_p = Persons.Add(p).Entity;
        SaveChanges();
        return new_p.Id;
    }

    public bool RemoveId(int id) {
        int rowsDeleted = Persons
            .Where(p => p.Id == id)
            .ExecuteDelete();
        SaveChanges();
        return rowsDeleted > 0;
    }
}