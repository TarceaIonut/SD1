using Hospital.Models;
using Hospital.Models.ViewModels;
using Hospital.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hospital.Controllers;

public class FunctionsController : Controller {
    private readonly FunctionsService _service;

    public FunctionsController(FunctionsService service) {
        _service = service;
    }

    [HttpGet]
    public IActionResult DoctorCheckups() {
        var v = _service.GetDoctorCheckups();
        String s = "Doctors\n";
        foreach(var d in v.Doctors) {
            s += d + "\n";
        }
        s += "Patients\n";
        foreach (var p in v.Patients) {
            s += p + "\n";
        }
        //Console.WriteLine(s);
        //ModelState.AddModelError("", s);
        return View(v);
    }
    [HttpPost]
    public async Task<ActionResult> DoctorCheckups(DoctorCheckupVewModel model) {
        var r = _service.NewCheckup(model);
        if (!r.HasValue()) {
            ModelState.AddModelError("", r.Error!);
        }
        return View(model);
    }
}