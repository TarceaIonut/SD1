using System.Windows.Input;
using AccountDiffService;
using Hospital.Controllers.Command;
using Hospital.Models;
using Hospital.Models.HelperStructures;
using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Hospital.Controllers;

public class AccountController : Controller {
    private readonly IUserService  _userService;
    
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;
    private readonly AccountServiceWrite.AccountServiceWriteClient _accountWrite;
    
    private readonly PersonsServiceRead.PersonsServiceReadClient _personsClientRead;
    private readonly PersonsServiceWrite.PersonsServiceWriteClient _personsClientWrite;

    private readonly DoctorCheckupRead.DoctorCheckupReadClient _checkupRead;
    private readonly DoctorCheckupWrite.DoctorCheckupWriteClient _checkupWrite;
    
    //private readonly ILogger<AccountController> _logger;
    public AccountController(IUserService userService,  
        AccountServiceRead.AccountServiceReadClient accountRead,AccountServiceWrite.AccountServiceWriteClient accountWrite)
    {
        _accountRead = accountRead;
        _accountWrite = accountWrite;
        _userService = userService;
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
        var c = new GetAccountCommand(_accountRead);
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
        Console.WriteLine("_userService.GetRole() = " + _userService.GetRole().ToString());
        Console.WriteLine("model.speciality = " + (model.specialty));
        if (_userService.GetRole() == Person.UserRole.Doctor) {
            if (model.specialty == null || model.specialty.Length == 0) {
                ModelState.AddModelError("", "speciality can not be null");
                return View(model);
            }
        }else {
            model.specialty = "";
        }
        
        var c = new NewAccountCommand(_accountWrite);
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
    public IActionResult DeleteMyAccount() {
        int? id = HttpContext.Session.GetInt32("UserId");
        if (id == null) {
            ModelState.AddModelError("",  "User id not found");
        }else {
            try
            {
                var r = _accountWrite.deleteById(new deleteByIdRequest { Id = (uint)id.Value });
                if (r == null || r.Result == false) {
                    ModelState.AddModelError("",  "Account not found");
                    return SignOut();
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
            var c = new GetAllAccountsCommand(_accountRead);
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

    private List<Person> GetAllPersons__(Person.UserRole? role) {
        var c = new GetAllAccountsCommand(_accountRead);
        var list = c.ExecuteAsync().Result; 
        var personsList = new List<Person>();

        foreach (var account in list) {
            if (role != null && account.role != role.Value) {
                continue;
            }
            Person newPerson = account.role switch {
                Person.UserRole.Admin => new Admin(account.Email, account.role),
                Person.UserRole.Doctor => new Doctor(account.Email, account.role, account.Speciality),
                Person.UserRole.Patient => new Patient(account.Email, account.role),
                _ => throw new InvalidOperationException($"Unknown role: {account.role}")
            };
            newPerson.Account = new Account { Username = account.Username };
            personsList.Add(newPerson);
        }
        return personsList;
    }
    public List<Person> GetAllPersons_(DoctorCheckupsFunctions.SortOrder? sortOrder, Person.UserRole? role) {
        var v = role == null ? GetAllPersons__(null) : GetAllPersons__(role!.Value);
        if (sortOrder != null) {
            SortPersonsByName(v, sortOrder.Value);
        }
        return v;
    }
    public static void SortPersonsByName(List<Person> list, DoctorCheckupsFunctions.SortOrder order) {
        switch (order) {
            case DoctorCheckupsFunctions.SortOrder.ASC :
                list.Sort((person, person1) =>
                    String.Compare(person.Account.Username, person1.Account.Username, StringComparison.Ordinal));
                break;
            case DoctorCheckupsFunctions.SortOrder.DESC : 
                list.Sort((person, person1) => String.Compare(person1.Account.Username, person.Account.Username, StringComparison.Ordinal));
                break;
        }
    }

    public async Task<IActionResult> test_GRPC() {
        // ModelState.AddModelError("", "query service Accounts");
        // var command = new TestCommand(_greeterClient);
        // string s = await command.ExecuteAsync();
        //
        // ModelState.AddModelError("", "query service Accounts " + s);
        //
        // var model = new AccountsViewModel 
        // {
        //     Show = false, 
        //     _persons = new List<AccountPrint>() 
        // };
        return View("Accounts");
    }
}