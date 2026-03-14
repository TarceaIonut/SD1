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
        if (!model.canSelectDoctor) {
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
        if (!r.HasValue()) {
            ModelState.AddModelError("", r.Error!);
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult DoctorCheckupList() {
        var r = GetCurrentRole.Role(HttpContext);
        if (r == Person.UserRole.Admin) {
            return View(_service.DoctorCheckupsList());
        }else if (r == Person.UserRole.Doctor) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(new List<DoctorCheckupVewModel>());
            }
            var res = _service.DoctorCheckupsListDoctor(FromUserEmail(de, dn));
            return View(res.Select(dc => new DoctorCheckupVewModel(dc)).ToList());
        }else if (r == Person.UserRole.Patient) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(new List<DoctorCheckupVewModel>());
            }
        }
        return View(_service.DoctorCheckupsList());
    }

    private static Doctor FromUserEmail(string email, string user) {
        var d = new Doctor(email, Person.UserRole.Doctor, "");
        d.Account = new Account(user, "");
        return d;
    } 
}