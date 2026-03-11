

using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories
{
    public class DoctorRepository
    {
        private readonly AppDbContext _context;

        public DoctorRepository (AppDbContext context) {
            _context = context;
        }
        public List<Doctor> GetAll() {
            return _context.Doctors.ToList();  
        }

        public List<Pair<Doctor,Account>> GetAllAccountInfo() {
            return _context.Doctors.Join(_context.Accounts, doctor => doctor.Id, account => account.personId, 
                (doctor, account) => new Pair<Doctor, Account>(doctor, account)).ToList();
        }
        public Doctor? GetById(int id) => _context.Doctors.Find(id);
        public bool Insert(Doctor doctor) {
            _context.Doctors.Add(doctor);
            return _context.SaveChanges() > 0;
        }
        public bool UpdateById(Doctor doctor) {
            _context.Doctors.Update(doctor);
            return _context.SaveChanges() > 0;
        }
        public bool  Delete(Doctor doctor) {
            _context.Doctors.Remove(doctor);
            return _context.SaveChanges() > 0;
        }
        public bool DeleteById(int id) {
            var doctor = GetById(id);
            if (doctor == null) return false;
            _context.Doctors.Remove(doctor);
            return _context.SaveChanges() > 0;
        }
    }
}

