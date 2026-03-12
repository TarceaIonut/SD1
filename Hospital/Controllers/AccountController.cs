using Hospital.Models;
using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Hospital.Repositories;
using Hospital.Service;

namespace Hospital.Controllers;

public class AccountController : Controller {
    private readonly AccountService _service;
    public AccountController(AccountService service)
    {
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
            HttpContext.Session.SetInt32("UserId", person.First.Id);
            HttpContext.Session.SetString("UserName", person.Second.Username);
            HttpContext.Session.SetString("UserEmail", person.First.Email);
            HttpContext.Session.SetString("UserRole", person.First.Role.ToString());
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
            HttpContext.Session.SetInt32("UserId", person.First.Id);
            HttpContext.Session.SetString("UserName", person.Second.Username);
            HttpContext.Session.SetString("UserEmail", person.First.Email);
            HttpContext.Session.SetString("UserRole", person.First.Role.ToString());
            var account = person.Second;
            ModelState.AddModelError("",  account + " " + account.person + "\n");
        }
        return View(model);
    }
    [HttpPost]
    public IActionResult SignOut() {
        HttpContext.Session.SetInt32("UserId", -1);
        HttpContext.Session.SetString("UserName", "Unsigned");
        HttpContext.Session.SetString("UserEmail", "");
        HttpContext.Session.SetString("UserRole", "");
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

    public IActionResult Accounts()
    {
        var role = GetCurrentRole.Role(HttpContext);
        if (role == Person.UserRole.Admin) {
            return View(_service.FindAll());
        }
        ModelState.AddModelError("",  "Admin role needed");
        return View(new List<Pair<Person, Account>>());
        return RedirectToAction("Index", "Home");
    }
}