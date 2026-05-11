using System.Windows.Input;
using AccountDiffService;
using Hospital.Controllers.Command;
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
    private readonly Greeter.GreeterClient _greeterClient;
    private readonly Persons.PersonsClient _personClient;
    //private readonly ILogger<AccountController> _logger;
    public AccountController(AccountService service, IUserService userService, Greeter.GreeterClient greeterClient, Persons.PersonsClient personClient ) {
        _userService = userService;
        _service = service;
        _greeterClient = greeterClient;
        _personClient = personClient;
    }

    [HttpGet]
    public IActionResult SignIn() => View();
    // [HttpPost]
    // public async Task<ActionResult> _SignIn(SignInModel model) {
    //     if (!ModelState.IsValid) return View(model);
    //     var res = _service.SignIn(model);
    //     if (res.HasValue()) {
    //         var person = res.Value!;
    //         _userService.SetUserId(person.First.Id);
    //         _userService.SetUser(person.Second.Username);
    //         _userService.SetEmail(person.First.Email);
    //         _userService.SetRole(person.First.Role);
    //     }else {
    //         var err = res.Error!;
    //         ModelState.AddModelError("", err);
    //     }
    //     return View(model);
    // }
    [HttpPost]
    public async Task<ActionResult> SignIn(SignInModel model) {
        if (!ModelState.IsValid) return View(model);
        var c = new GetAccountCommand(_greeterClient);
        var p = c.ExecuteAsync(model);
        if (p.Result == null) {
            ModelState.AddModelError("", "unknown error");
        }else {
            _userService.SetUserId(p.Result.Id);
            _userService.SetUser(p.Result.Username);
            _userService.SetEmail(p.Result.Email);
            _userService.SetRole(p.Result.Role.ToString());
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult SignUp() =>  View();
    [HttpPost]
    public async Task<ActionResult> SignUp(SignUpModel model) {
        if (!ModelState.IsValid) return View(model);
        //var res = _service.SignUp(model);
        if (_userService.GetRole() == Person.UserRole.Doctor) {
            if (model.specialty == null) {
                ModelState.AddModelError("", "speciality can not be null");
                return View(model);
            }
        }else {
            model.specialty = "";
        }
        
        var c = new NewAccountCommand(_greeterClient, _personClient);
        try {
            int id = await c.ExecuteAsync(model);
            _userService.SetUserId(id);
            _userService.SetUser(model.name);
            _userService.SetEmail(model.email);
            _userService.SetRole(model.Role);
        }catch (Exception e) {
            ModelState.AddModelError("", e.Message);
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

    // public IActionResult _DeleteMyAccount() {
    //     int? id = HttpContext.Session.GetInt32("UserId");
    //     if (id == null) {
    //         ModelState.AddModelError("",  "User id not found");
    //     }else {
    //         if (!_service.DeleteAccountId((int)id!)) {
    //             ModelState.AddModelError("",  "Account not found");
    //         }
    //     }
    //
    //     return SignOut();
    // }
    public IActionResult DeleteMyAccount() {
        int? id = HttpContext.Session.GetInt32("UserId");
        if (id == null) {
            ModelState.AddModelError("",  "User id not found");
        }else {
            try
            {
                var r = _greeterClient.deleteById(new deleteByIdRequest { Id = (uint)id.Value });
            
                if (r == null) {
                    ModelState.AddModelError("",  "Account not found");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return SignOut();
            }

        }

        return SignOut();
    }

    // public IActionResult _Accounts() {
    //     var role = _userService.GetRole();
    //     if (role == Person.UserRole.Admin) {
    //         return View(new AccountsViewModel(_service.GetAllPersons() ) );
    //     }
    //     ModelState.AddModelError("",  "Admin role needed");
    //     return View(new AccountsViewModel());
    // }

    public IActionResult Accounts() {
        var role = _userService.GetRole();
        if (role == Person.UserRole.Admin) {
            var c = new GetAllAccountsCommand(_greeterClient);
            return View(new AccountsViewModel(c.ExecuteAsync().Result ) );
        }
        ModelState.AddModelError("",  "Admin role needed");
        return View(new AccountsViewModel());
    }

    [HttpPost]
    public async Task<ActionResult> ExportAccounts(AccountsViewModel model) {
        var list = GetAllPersons_(model.SortOrder, model.UserRole);
        //model._persons = list;
        if (model.Format == null) {
            ModelState.AddModelError("", "Format field must be set");
        }else {
            var s = ExportHelper.GetStrategy(model.Format!.Value);
            var bytes = s.ExportData(list);
            return File(bytes, s.ContentType, $"Persons_{DateTime.Now:yyyyMMdd}.{s.Extention}");
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

    public async Task<IActionResult> test_GRPC() {
        ModelState.AddModelError("", "query service Accounts");
        var command = new TestCommand(_greeterClient);
        string s = await command.ExecuteAsync();
        
        ModelState.AddModelError("", "query service Accounts " + s);
        
        var model = new AccountsViewModel 
        {
            Show = false, 
            _persons = new List<AccountPrint>() 
        };
        return View("Accounts", model);
    }
}