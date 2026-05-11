using AccountDiffService;
using Hospital.Controllers.Command;
using Hospital.Repositories;
using Hospital.Service;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class DoctorsController : Controller
{
    private readonly Greeter.GreeterClient _greeterClient;
    private readonly DoctorsService _service;

    public DoctorsController(DoctorsService service, Greeter.GreeterClient greeterClient)
    {
        this._service = service;
        this._greeterClient = greeterClient;
    }
    public async Task<IActionResult> Doctors()
    {
        var c = new DoctorsCommand(_greeterClient);
        var v = c.ExecuteAsync().Result;
        
        return View(v);
    }
}