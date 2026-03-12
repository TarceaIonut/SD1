using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories;

public class FunctionsRepository {
    private readonly AppDbContext _context;
    public FunctionsRepository(AppDbContext context) {
        _context = context;
    }
    public List<Doctor> Doctors() => _context.Doctors.Include(d => d.Account).ToList();
    public List<Patient> Patients() => _context.Patients.Include(p => p.Account).ToList();
    public Doctor? DoctorFind(string email, string username) {
        return _context.Doctors.Include(d => d.Account).
            FirstOrDefault(d => d.Email == email && d.Account.Username == username);
    }
    public Patient? PatientFind(string email, string username) {
        return _context.Patients.Include(p => p.Account).
            FirstOrDefault(p => p.Email == email && p.Account.Username == username);
    }
    public DoctorCheckups NewCheckup(DoctorCheckups doctorCheckups) => _context.DoctorCheckups.Add(doctorCheckups).Entity;
}