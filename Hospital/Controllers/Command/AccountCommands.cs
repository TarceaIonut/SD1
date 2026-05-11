using AccountDiffService;
using Grpc.Core;
using Hospital.Models;
using Hospital.Models.ViewModels;

namespace Hospital.Controllers.Command;

public class AccountPrint {
    public string Email { get; set; }
    public string Username { get; set; }
    public Person.UserRole role { get; set; }
    public string? Speciality { get; set; }
    public AccountPrint(string email, string username, Person.UserRole role) {
        Email = email;
        Username = username;
        this.role = role;
    }
    public AccountPrint() {}
}
public class TestCommand : Command.ICommand<string> {
    private readonly Greeter.GreeterClient _grpcClient;
    public TestCommand(Greeter.GreeterClient client) {
        _grpcClient = client;
    }
    public async Task<string> ExecuteAsync() {
        var v = await _grpcClient.SayHelloAsync(new HelloRequest {
            Name = "Test"
        });
        return v.Message;
    }
}

public class GetAllAccountsCommand : ICommand<List<AccountPrint>> {
    private readonly Greeter.GreeterClient _grpcClient;
    public GetAllAccountsCommand(Greeter.GreeterClient grpcClient) {
        _grpcClient = grpcClient;
    }
    public async Task<List<AccountPrint>> ExecuteAsync() {
        Console.WriteLine("ListAllAccounts rcg command started");
        var v = _grpcClient.ListAllAccounts(new ListAllAccountsRequest());
        var list = new List<AccountPrint>(v.Accounts.Count);
        foreach (AccountFullInfo account in v.Accounts) {
            list.Add(new AccountPrint(account.Email, account.Username, (Person.UserRole)account.Role));
        }
        return list;
    }
}

public class PersonPrint {
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public Person.UserRole Role { get; set; }
    public PersonPrint() {}
}
public class GetAccountCommand : ICommand<PersonPrint?, SignInModel> {
    private readonly Greeter.GreeterClient _grpcClient;
    public GetAccountCommand(Greeter.GreeterClient grpcClient) {
        _grpcClient = grpcClient;
    }
    public async Task<PersonPrint?> ExecuteAsync(SignInModel account) {
        getAccountReply v;
        try {
            v = _grpcClient.getAccount(new getAccountRequest { Password = account.password, Username = account.name });
        }
        catch (RpcException e) {
            if (e.Status.StatusCode == StatusCode.NotFound) return null;
            throw e;
        }
        return new PersonPrint{Id = v.Result.Id, Email = v.Result.Email, Username = v.Result.Username, Role = (Person.UserRole)v.Result.Role};
    }
}

public class NewAccountCommand : ICommand<int, SignUpModel> {
    private readonly Greeter.GreeterClient _grpcClient;
    private readonly Persons.PersonsClient _personClient;
    public NewAccountCommand(Greeter.GreeterClient grpcClient, Persons.PersonsClient personClient) {
        _personClient = personClient;
        _grpcClient = grpcClient;
    }
    public async Task<int> ExecuteAsync(SignUpModel model) {
        // var findAccount = _grpcClient.AccountExistsUser(new AccountExistsUserRequest{Username = model.name});
        // if (findAccount.Exists == true) {
        //     throw new Exception("Username already exists");
        // }
        // var findPerson = _personClient.PersonExistsEmail(new PersonExistsEmailRequest{Email = model.email});
        // if (findPerson.Exists == true) {
        //     throw new Exception("Email already exists");
        // }
        var newAccount = _grpcClient.newAccount(new newAccountRequest
            {Email = model.email, Username = model.name, Password =  model.password, Role = (uint)model.Role, Speciality = model.specialty});
        return newAccount.Id;
    }
}