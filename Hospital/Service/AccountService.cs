using Hospital.Models;
using Hospital.Repositories;
using Hospital.Models.ViewModels;

namespace Hospital.Service;

public class AccountService
{
    private readonly AccountRepository _repository;
    public AccountService(AccountRepository repository) {
        _repository = repository;
    }

    public Result<Pair<Person, Account>, string> SignUp(SignUpModel model) {
        var res = model.ToPerson();
        if (res.HasValue()) {
            Account account = res.Value!;
            if (!_repository.NewAccount(account, account.person)) {
                return Result<Pair<Person, Account>, string>.Error_("username, email already exists");
            }
            return Result<Pair<Person,Account>, string>.Success(new(account.person, account));
        }
        return Result<Pair<Person, Account>, string>.Error_(res.Error!);
    }
    public Result<Pair<Person, Account>, string> SignIn(SignInModel model) {
        var person = _repository.FindByNamePass(model.name, model.password);
        if (person == null) {
            return Result<Pair<Person, Account>, string>.Error_("username or password is incorrect");
        }
        return Result<Pair<Person, Account>, string>.Success(person);
    }
    public List<Pair<Person, Account>> FindAll() => _repository.FindAll();
    public bool DeleteAccountId(int id) => _repository.deleteAccountById(id);
    public List<Person> GetAllPersons() => _repository.GetAllPersons();
    public List<Person> GetAllPersons(Person.UserRole role) => _repository.GetAllPersons(role);

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

}