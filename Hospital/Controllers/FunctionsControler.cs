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
        var r = _service.NewCheckup(model);
        if (!r.HasValue()) {
            ModelState.AddModelError("", r.Error!);
        }
        var v = _service.GetDoctorCheckups();
        var res = GetCurrentRole.Role(HttpContext);
        if (res == Person.UserRole.Admin) v.canSelectDoctor = true;
        else if (res == Person.UserRole.Doctor) v.canSelectDoctor = false;
        else {
            ModelState.AddModelError("",  "Admin/Doctor role needed");
            return View(DoctorCheckupVewModel.EmptyInstance());
        }
        return View("DoctorCheckups", v);
    }

    [HttpGet]
    public IActionResult DoctorCheckupList() {
        //ModelState.AddModelError("", JsonSerializer.Serialize(_service.DoctorCheckupsList()));
        return View(new DoctorCheckupsFunctions(DoctorCheckupListP(), GetCurrentRole.Role(HttpContext) == Person.UserRole.Admin));
    }

    private List<DoctorCheckups> DoctorCheckupListP() {
        var r = GetCurrentRole.Role(HttpContext);
        if (r == null) {
            ModelState.AddModelError("",  "UserEmail/UserName undefined");
            return new List<DoctorCheckups>();
        }
        if (r == Person.UserRole.Admin) {
            return _service.DoctorCheckupsList();
        }else if (r == Person.UserRole.Doctor) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return new List<DoctorCheckups>();
            }
            var res = _service.DoctorCheckupsListDoctor(FromUserEmail(de, dn));
            return res;
        }else if (r == Person.UserRole.Patient) {
            var de = HttpContext.Session.GetString("UserEmail");
            var dn =  HttpContext.Session.GetString("UserName");
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return new List<DoctorCheckups>();
            }
            return _service.DoctorCheckupsListPatient(PFromUserEmail(de, dn));
        }
        return _service.DoctorCheckupsList();
    }

    [HttpPost]
    public async Task<ActionResult> DoctorCheckupDelete(DoctorCheckupsFunctions model) {
        
        var s = DoctorCheckupDeleteP(model);
        if (s != null) {
            ModelState.AddModelError("", s);
        }else {
            model.DoctorCheckups = DoctorCheckupListP();
        }
        return View("DoctorCheckupList", model);
    }

    [HttpPost]
    public async Task<ActionResult> DoctorCheckupUpdate(DoctorCheckupsFunctions model) {
        var s = checkModel(model);
        if (s != null) {
            ModelState.AddModelError("", s);
        }else {
            model.AppointmentDate = DateTime.SpecifyKind(model.AppointmentDate!.Value, DateTimeKind.Utc);
            var res = _service.DoctorCheckupsUpdate(model.Id!.Value, model.Description!, model.AppointmentDate!.Value);
            if (res != null) {
                ModelState.AddModelError("", res);
            }else {
                model.DoctorCheckups = DoctorCheckupListP();
            }
        }
        return View("DoctorCheckupList", model);
    }

    private string? checkModel(DoctorCheckupsFunctions model) {
        if (model.Id == null) return "Id is null";
        if (model.Description == null) return "Description in null";
        if (model.AppointmentDate == null) return "AppointmentDate in null";
        return null;
    } 

    private string? DoctorCheckupDeleteP(DoctorCheckupsFunctions model) {
        var id = model.Id;
        if (model.Id == null) return "Id of a checkup required\n";
        if (_service.DoctorCheckupsDelete(id!.Value)) return null;
        return "Delete cound not be made: IDK why";
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