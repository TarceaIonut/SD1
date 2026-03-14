using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories
{
    public class AppDbContext : DbContext
    {   
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Person> Persons { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<DoctorCheckups>  DoctorCheckups { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorCheckups>().HasKey(dc => dc.Id);
            modelBuilder.Entity<DoctorCheckups>().HasOne(a => a.Doctor)
                .WithMany(d => d.DoctorCheckups)
                .HasForeignKey(a => a.DoctorId);
            modelBuilder.Entity<DoctorCheckups>().HasOne(a => a.Patient)
                .WithMany(p => p.DoctorCheckups)
                .HasForeignKey(a => a.PatientId);
            
            modelBuilder.Entity<Person>().HasKey(d => d.Id);
            modelBuilder.Entity<Person>().Property(p => p.Role).HasColumnName("UserRole");
            modelBuilder.Entity<Account>().HasKey(a => a.Id);
        
            modelBuilder.Entity<Account>().HasOne(a => a.person).WithOne(p => p.Account)
                .HasForeignKey<Account>(a => a.personId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Person>()
                .HasDiscriminator<Person.UserRole>("UserRole")
                .HasValue<Admin>(Person.UserRole.Admin)
                .HasValue<Doctor>(Person.UserRole.Doctor)
                .HasValue<Patient>(Person.UserRole.Patient);
        }
        public static String ListToString<T>(List<T> set) where T : class {
            String buffer = typeof(T).Name + "\n";
            foreach (var var in set) {
                buffer += var + "\n";
            }
            return buffer;
        }
        
    }
}

