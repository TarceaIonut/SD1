using AccountDiffService;
using Hospital.Controllers.Command;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class DoctorsController : Controller
{
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;

    public DoctorsController( AccountServiceRead.AccountServiceReadClient accountRead)
    {
        _accountRead = accountRead;
    }
    public async Task<IActionResult> Doctors()
    {
        var c = new DoctorsCommand(_accountRead);
        var v = c.ExecuteAsync().Result;
        
        return View(v);
    }
}