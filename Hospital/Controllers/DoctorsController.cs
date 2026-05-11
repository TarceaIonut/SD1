using AccountDiffService;
using Hospital.Controllers.Command;
using Hospital.Repositories;
using Hospital.Service;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class DoctorsController : Controller
{
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;
    private readonly DoctorsService _service;

    public DoctorsController(DoctorsService service, AccountServiceRead.AccountServiceReadClient accountRead)
    {
        _service = service;
        _accountRead = accountRead;
    }
    public async Task<IActionResult> Doctors()
    {
        var c = new DoctorsCommand(_accountRead);
        var v = c.ExecuteAsync().Result;
        
        return View(v);
    }
}