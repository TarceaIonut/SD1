using AccountDiffService;
using DoctorCheckupsService.Models;
using Microsoft.EntityFrameworkCore;
using DoctorCheckup = DoctorCheckupsService.Models.DoctorCheckup;

namespace DoctorCheckupsService.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<DoctorCheckup> DoctorCheckups { get; set; }

    public int AddDoctorCheckup(DoctorCheckup doctorCheckup)
    {
        var newDc = DoctorCheckups.Add(doctorCheckup).Entity;
        SaveChanges();
        return newDc.Id;
    }

    public bool deleteById(int id) {
        int rowsDeleted = DoctorCheckups.Where(p => p.Id == id).ExecuteDelete();
        SaveChanges();
        return rowsDeleted > 0;
    }
    public void UpdateDoctorCheckup(DoctorCheckup doctorCheckup){
        DoctorCheckups.Update(doctorCheckup);
        SaveChanges();
    }
    public DoctorCheckup? GetById(int id) => DoctorCheckups.FirstOrDefault(p => p.Id == id);
    public List<DoctorCheckup> GetDoctorCheckups() => DoctorCheckups.ToList();
}