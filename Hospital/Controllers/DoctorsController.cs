using Hospital.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class DoctorsController : Controller
{
    private readonly DoctorRepository repository;

    public DoctorsController(DoctorRepository repository)
    {
        this.repository = repository;
    }
    public IActionResult Doctors()
    {
        return View(repository.GetAllAccountInfo());
    }

    public IActionResult DoctorDetails()
    {
        return View(repository.GetAllAccountInfo());
    }
}