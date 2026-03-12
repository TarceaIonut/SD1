using Hospital.Models;
using Hospital.Repositories;

namespace Hospital.Service;

public class DoctorsService
{
    private readonly DoctorRepository _context;
    public DoctorsService(DoctorRepository context) {
        _context = context;
    }

    public List<Pair<Doctor,Account>> GetAllAccountInfo() => _context.GetAllAccountInfo();
}