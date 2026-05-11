using System.Text.Json;
using AccountDiffService;
using Hospital.Controllers.Command;
using Hospital.Models;
using Hospital.Models.HelperStructures;
using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
namespace Hospital.Controllers;

public class FunctionsController : Controller {
    private readonly IUserService  _userService;
    
    private readonly DoctorCheckupWrite.DoctorCheckupWriteClient _docCheckWrite;
    private readonly DoctorCheckupRead.DoctorCheckupReadClient _docCheckRead;
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;
    public FunctionsController(IUserService userService, DoctorCheckupWrite.DoctorCheckupWriteClient docCheckWrite,
        AccountServiceRead.AccountServiceReadClient accountRead, DoctorCheckupRead.DoctorCheckupReadClient docCheckRead) {
        _userService = userService;
        _docCheckWrite = docCheckWrite;
        _accountRead = accountRead;
        _docCheckRead = docCheckRead;
    }

    [HttpGet]
    public IActionResult DoctorCheckups() {
        var v = new DoctorCheckupVewModel(_getAllDoctors(), _getAllPatients());
        var r = _userService.GetRole();
        if (r == Person.UserRole.Admin) v.canSelectDoctor = true;
        else if (r == Person.UserRole.Doctor) v.canSelectDoctor = false;
        else {
            ModelState.AddModelError("",  "Admin/Doctor role needed");
            return View(DoctorCheckupVewModel.EmptyInstance());
        }
        return View(v);
    }

    
    private List<Doctor> _getAllDoctors()
    {
        var all = _accountRead.ListAllAccounts(new ListAllAccountsRequest());
        var res = new List<Doctor>();
        foreach (var doctor in all.Accounts)
        {
            if (doctor != null && (Person.UserRole)doctor.Role == Person.UserRole.Doctor)
                res.Add(new Doctor{Specialty = doctor.Speciality, Email = doctor.Email, Account = new Account{Username = doctor.Username}});
        }
        return res;
    }
    private List<Patient> _getAllPatients()
    {
        var all = _accountRead.ListAllAccounts(new ListAllAccountsRequest());
        var res = new List<Patient>();
        foreach (var patient in all.Accounts)
        {
            if (patient != null && (Person.UserRole)patient.Role == Person.UserRole.Patient)
                res.Add(new Patient{Email = patient.Email, Account = new Account{Username = patient.Username}});
        }
        return res;
    }
    [HttpPost]
    public async Task<ActionResult> DoctorCheckups(DoctorCheckupVewModel model) {
        if (_userService.GetRole() == Person.UserRole.Doctor)
        {
            var de = _userService.GetEmail();
            var dn =  _userService.GetUser();
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(model);
            }
            model.DoctorEmail = de!;
            model.DoctorName = dn!;
        }
        if (_userService.GetRole() == Person.UserRole.Patient)
        {
            var de = _userService.GetEmail();
            var dn =  _userService.GetUser();
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return View(model);
            }
            model.PatientEmail = de!;
            model.PatientName = dn!;
        }
        var c = new NewCheckupCommand(_docCheckWrite);
        try
        {
            var r = c.ExecuteAsync(model);
            if (!r.Result)
                ModelState.AddModelError("", "could not delete");
        }
        catch (Exception e)
        {
            ModelState.AddModelError("", e.Message);
        }
        
        //var v = _service.GetDoctorCheckups();
        var v = new DoctorCheckupVewModel(_getAllDoctors(), _getAllPatients());
        var res = _userService.GetRole();
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
        return View(new DoctorCheckupsFunctions(DoctorCheckupListP(), _userService.GetRole() == Person.UserRole.Admin));
    }

    private List<DoctorCheckups> _doctorCheckupsListAll() {
        var c = new ListAllCommand(_docCheckRead);
        return c.ExecuteAsync().Result;
    }

    private List<DoctorCheckups> _doctorCheckupsListPatient(string name, string email)
    {
        return _doctorCheckupsListAll().Where(d => d.Patient.Email == email && d.Patient.Account.Username == name)
            .ToList();
    }
    private List<DoctorCheckups> _doctorCheckupsListDoctor(string name, string email) {
        return _doctorCheckupsListAll().Where(d => d.Doctor.Email == email && d.Doctor.Account.Username == name)
            .ToList();
    }
    private List<DoctorCheckups> DoctorCheckupListP() {
        var r = _userService.GetRole();
        if (r == null) {
            ModelState.AddModelError("",  "UserEmail/UserName undefined");
            return new List<DoctorCheckups>();
        }
        if (r == Person.UserRole.Admin) {
            return _doctorCheckupsListAll();
        }else if (r == Person.UserRole.Doctor) {
            var de = _userService.GetEmail();
            var dn =  _userService.GetUser();
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return new List<DoctorCheckups>();
            }
            var res = _doctorCheckupsListDoctor(dn, de);
            return res;
        }else if (r == Person.UserRole.Patient) {
            var de = _userService.GetEmail();
            var dn =  _userService.GetUser();
            if (de == null || dn == null) {
                ModelState.AddModelError("",  "UserEmail/UserName undefined = " + de + " / " + dn);
                return new List<DoctorCheckups>();
            }
            return _doctorCheckupsListPatient(dn, de);
        }
        return _doctorCheckupsListAll();
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
    public async Task<ActionResult> DoctorCheckupSort(DoctorCheckupsFunctions model) {
        model.DoctorCheckups = DoctorCheckupListP();
        var s = DoctorCheckupsSort(model.DoctorCheckups, model.sortOrder, model.sortOn);
        if (s != null) {
            ModelState.AddModelError("", s!);
        }
        return View("DoctorCheckupList", model);
    }
    public string? DoctorCheckupsSort(List<DoctorCheckups> list, DoctorCheckupsFunctions.SortOrder? sortOrder,
        DoctorCheckupsFunctions.SortOn? sortOn) {
        if (sortOrder == null) return "sortOrder is null";
        if (sortOn == null) return "sortOn is null";
        switch (sortOn, sortOrder) {
            case (DoctorCheckupsFunctions.SortOn.DATE, DoctorCheckupsFunctions.SortOrder.ASC):
                list.Sort((x, y) => x.AppointmentDate.CompareTo(y.AppointmentDate));
                break;

            case (DoctorCheckupsFunctions.SortOn.DATE, DoctorCheckupsFunctions.SortOrder.DESC):
                list.Sort((x, y) => y.AppointmentDate.CompareTo(x.AppointmentDate));
                break;
            case (DoctorCheckupsFunctions.SortOn.NAME, DoctorCheckupsFunctions.SortOrder.ASC):
                list.Sort((x, y) => 
                    String.Compare(x.Patient.Account.Username,y.Patient.Account.Username, StringComparison.Ordinal));
                break;
            case (DoctorCheckupsFunctions.SortOn.NAME, DoctorCheckupsFunctions.SortOrder.DESC):
                list.Sort((x, y) => 
                    String.Compare(y.Patient.Account.Username,x.Patient.Account.Username, StringComparison.Ordinal));
                break;
            
        }
        return null;
    }
    [HttpPost]
    public async Task<ActionResult> Export(DoctorCheckupsFunctions model) {
        model.DoctorCheckups = DoctorCheckupListP();
        if (model.sortOn != null && model.sortOrder != null) {
            var s = DoctorCheckupsSort(model.DoctorCheckups, model.sortOrder, model.sortOn);
            if (s != null) {
                ModelState.AddModelError("", s!);
            }
        }
        if (model.Format == null) {
            ModelState.AddModelError("", "Format field must be set");
        }else {
            var s = ExportHelper.GetStrategy(model.Format!.Value);
            var bytes = s.ExportData(model.DoctorCheckups);
            return File(bytes, s.ContentType, $"DoctorCheckups_{DateTime.Now:yyyyMMdd}.{s.Extention}");
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
            //var res = _service.DoctorCheckupsUpdate(model.Id!.Value, model.Description!, model.AppointmentDate!.Value);
            var c = new UpdateCommand(_docCheckWrite);
            var r = await c.ExecuteAsync(model);
            if (r == false) {
                ModelState.AddModelError("", "update failed");
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
        //if (_service.DoctorCheckupsDelete(id!.Value)) return null;
        var c = new DeleteCommand(_docCheckWrite);
        if (c.ExecuteAsync(id.Value).Result)
            return null;
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