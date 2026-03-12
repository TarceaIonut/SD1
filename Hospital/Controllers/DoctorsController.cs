using Hospital.Repositories;
using Hospital.Service;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class DoctorsController : Controller
{
    private readonly DoctorsService _service;

    public DoctorsController(DoctorsService service)
    {
        this._service = service;
    }
    public IActionResult Doctors() {
        return View(_service.GetAllAccountInfo());
    }
}