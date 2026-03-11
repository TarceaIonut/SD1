using Hospital.Models;
using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Hospital.Repositories;

namespace Hospital.Controllers
{
    
}

public class AccountController : Controller {
    private readonly AccountRepository _repository;
    public AccountController(AccountRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult SignIn() => View();
    [HttpPost]
    public async Task<ActionResult> SignIn(SignInModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var person = _repository.FindByNamePass(model.name, model.password);
        if (person == null)
        {
            ModelState.AddModelError("", "Invalid username or password");
        }
        else {
            HttpContext.Session.SetInt32("UserId", person.First.Id);
            HttpContext.Session.SetString("UserName", person.Second.Username);
            HttpContext.Session.SetString("UserEmail", person.First.Email);
            HttpContext.Session.SetString("UserRole", person.First.Role.ToString());
            ModelState.AddModelError("",  AppDbContext.ListToString(_repository._context.Accounts));
        }
        
        return View(model);
    }
    [HttpGet]
    public IActionResult SignUp() =>  View();
    [HttpPost]
    public async Task<ActionResult> SignUp(SignUpModel model) {
        if (!ModelState.IsValid) return View(model);
        var res = model.ToPerson();
        if (!res.HasValue()) {
            ModelState.AddModelError("",  res.Error);
        }else {
            Account account = res.Value;
            ModelState.AddModelError("",  account + " " + account.person + "\n");
            if (!_repository.NewAccount(account, account.person)) {
                ModelState.AddModelError("", "username, email already exists");
            }else {
                ModelState.AddModelError("",  AppDbContext.ListToString(_repository._context.Accounts));
            }
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
}