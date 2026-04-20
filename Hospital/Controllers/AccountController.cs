using Hospital.Models;
using Hospital.Models.HelperStructures;
using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Hospital.Repositories;
using Hospital.Service;

namespace Hospital.Controllers;

public class AccountController : Controller {
    private readonly AccountService _service;
    private readonly IUserService  _userService;
    public AccountController(AccountService service, IUserService userService) {
        _userService = userService;
        _service = service;
    }

    [HttpGet]
    public IActionResult SignIn() => View();
    [HttpPost]
    public async Task<ActionResult> SignIn(SignInModel model) {
        if (!ModelState.IsValid) return View(model);
        var res = _service.SignIn(model);
        if (res.HasValue()) {
            var person = res.Value!;
            _userService.SetUserId(person.First.Id);
            _userService.SetUser(person.Second.Username);
            _userService.SetEmail(person.First.Email);
            _userService.SetRole(person.First.Role);
        }else {
            var err = res.Error!;
            ModelState.AddModelError("", err);
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult SignUp() =>  View();
    [HttpPost]
    public async Task<ActionResult> SignUp(SignUpModel model) {
        if (!ModelState.IsValid) return View(model);
        var res = _service.SignUp(model);
        if (!res.HasValue()) {
            ModelState.AddModelError("",  res.Error!);
        }else {
            var person = res.Value!;
            _userService.SetUserId(person.First.Id);
            _userService.SetUser(person.Second.Username);
            _userService.SetEmail(person.First.Email);
            _userService.SetRole(person.First.Role);
            var account = person.Second;
            //ModelState.AddModelError("",  account + " " + account.person + "\n");
        }
        return View(model);
    }
    [HttpPost]
    public IActionResult SignOut() {
        _userService.SetUserId(-1);
        _userService.SetUser("Unsigned");
        _userService.SetEmail("");
        _userService.SetRole("");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult DeleteMyAccount() {
        int? id = HttpContext.Session.GetInt32("UserId");
        if (id == null) {
            ModelState.AddModelError("",  "User id not found");
        }else {
            if (!_service.DeleteAccountId((int)id!)) {
                ModelState.AddModelError("",  "Account not found");
            }
        }

        return SignOut();
    }

    public IActionResult Accounts() {
        var role = _userService.GetRole();
        if (role == Person.UserRole.Admin)
        {
            return View(new AccountsViewModel(_service.GetAllPersons() ) );
        }
        ModelState.AddModelError("",  "Admin role needed");
        return View(new AccountsViewModel());
    }

    [HttpPost]
    public async Task<ActionResult> ExportAccounts(AccountsViewModel model) {
        var list = GetAllPersons_(model.SortOrder, model.UserRole);
        model._persons = list;
        if (model.Format == null) {
            ModelState.AddModelError("", "Format field must be set");
        }else {
            var s = ExportHelper.GetStrategy(model.Format!.Value);
            var bytes = s.ExportData(list);
            return File(bytes, s.ContentType, $"Persons_{DateTime.Now:yyyyMMdd}.{s.extention}");
        }
        return View("Accounts", model);
    }
    
    public List<Person> GetAllPersons_(DoctorCheckupsFunctions.SortOrder? sortOrder, Person.UserRole? role) {
        var v = role == null ? _service.GetAllPersons() : _service.GetAllPersons(role!.Value);
        if (sortOrder != null) {
            AccountService.SortPersonsByName(v, sortOrder.Value);
        }
        return v;
    }
}