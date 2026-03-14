using System.Text.Json;
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
        var r = GetCurrentRole.Role(HttpContext);
        if (r == Person.UserRole.Admin) v.canSelectDoctor = true;
        else if (r == Person.UserRole.Doctor) v.canSelectDoctor = false;
        else {
            ModelState.AddModelError("",  "Admin/Doctor role needed");
            return View(DoctorCheckupVewModel.EmptyInstance());
        }
        return View(v);
    }
    [HttpPost]
    public async Task<ActionResult> DoctorCheckups(DoctorCheckupVewModel model) {
        if (GetCurrentRole.Role(HttpContext) == Person.UserRole.Doctor) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(model);
            }
            model.DoctorEmail = de!;
            model.DoctorName = dn!;
        }
        ModelState.AddModelError("", model.ToString());
        var r = _service.NewCheckup(model);
        //ModelState.AddModelError("", JsonSerializer.Serialize(_service.DoctorCheckupsList()));
        if (!r.HasValue()) {
            ModelState.AddModelError("", r.Error!);
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult DoctorCheckupList() {
        //ModelState.AddModelError("", JsonSerializer.Serialize(_service.DoctorCheckupsList()));
        var r = GetCurrentRole.Role(HttpContext);
        if (r == null) {
            ModelState.AddModelError("",  "UserEmail/UserName undefined");
            return View(new List<DoctorCheckups>());
        }
        if (r == Person.UserRole.Admin) {
            return View(_service.DoctorCheckupsList());
        }else if (r == Person.UserRole.Doctor) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(new List<DoctorCheckups>());
            }
            var res = _service.DoctorCheckupsListDoctor(FromUserEmail(de, dn));
            return View(res);
        }else if (r == Person.UserRole.Patient) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(new List<DoctorCheckups>());
            }
            return View(_service.DoctorCheckupsListPatient(PFromUserEmail(de, dn)));
        }
        return View(_service.DoctorCheckupsList());
    }

    private static Doctor FromUserEmail(string email, string user) {
        var d = new Doctor(email, Person.UserRole.Doctor, "");
        d.Account = new Account(user, "");
        return d;
    } 
    private static Patient PFromUserEmail(string email, string user) {
        var d = new Patient(email, Person.UserRole.Patient);
        d.Account = new Account(user, "");
        return d;
    } 
}