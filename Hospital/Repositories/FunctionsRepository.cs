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
    public Doctor? DoctorFind(string email, string username)
    {
        var d = Doctors();
        var p = Patients();
        return _context.Doctors.Include(d => d.Account).
            FirstOrDefault(d => d.Email == email && d.Account.Username == username);
    }
    public Patient? PatientFind(string email, string username) {
        var d = Doctors();
        var p = Patients();
        return _context.Patients.Include(p => p.Account).
            FirstOrDefault(p => p.Email == email && p.Account.Username == username);
    }

    public DoctorCheckups? NewCheckup(DoctorCheckups doctorCheckups) {
        var v = _context.DoctorCheckups.Add(doctorCheckups).Entity;
        if (_context.SaveChanges() > 0) return v;
        else return null;
    }
    public List<DoctorCheckups> DoctorCheckupsList() => _context.DoctorCheckups
        .Include(d => d.Doctor).ThenInclude(d => d.Account)
        .Include(d => d.Patient).ThenInclude(p => p.Account)
        .ToList();

    public List<DoctorCheckups> DoctorCheckupsListDoctor(Doctor doctor) =>
        _context.DoctorCheckups
            .Include(d => d.Doctor).ThenInclude(d => d.Account)
            .Include(d => d.Patient).ThenInclude(p => p.Account)
            .Where(dc => dc.Doctor.Email == doctor.Email && dc.Doctor.Account.Username == doctor.Account.Username).ToList();
    public List<DoctorCheckups> DoctorCheckupsListPatient(Patient patient) =>
        _context.DoctorCheckups
            .Include(d => d.Doctor).ThenInclude(d => d.Account)
            .Include(d => d.Patient).ThenInclude(p => p.Account)
            .Where(dc => dc.Patient.Email == patient.Email && dc.Patient.Account.Username == patient.Account.Username).ToList();
    
}