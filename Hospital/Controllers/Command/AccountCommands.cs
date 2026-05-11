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
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;
    public GetAllAccountsCommand(AccountServiceRead.AccountServiceReadClient accountRead) {
        _accountRead = accountRead;
    }
    public async Task<List<AccountPrint>> ExecuteAsync() {
        Console.WriteLine("ListAllAccounts rcg command started");
        var v = _accountRead.ListAllAccounts(new ListAllAccountsRequest());
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
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;
    public GetAccountCommand(AccountServiceRead.AccountServiceReadClient accountRead) {
        _accountRead = accountRead;
    }
    public async Task<PersonPrint?> ExecuteAsync(SignInModel account) {
        getAccountReply v;
        try {
            v = _accountRead.getAccount(new getAccountRequest { Password = account.password, Username = account.name });
        }
        catch (RpcException e) {
            if (e.Status.StatusCode == StatusCode.NotFound) return null;
            throw e;
        }
        return new PersonPrint{Id = v.Result.Id, Email = v.Result.Email, Username = v.Result.Username, Role = (Person.UserRole)v.Result.Role};
    }
}

public class NewAccountCommand : ICommand<int, SignUpModel>
{
    private readonly AccountServiceWrite.AccountServiceWriteClient _client;
    public NewAccountCommand(AccountServiceWrite.AccountServiceWriteClient client) {
        _client = client;
    }
    public async Task<int> ExecuteAsync(SignUpModel model) {
        var newAccount = _client.newAccount(new newAccountRequest
            {Email = model.email, Username = model.name, Password =  model.password, Role = (uint)model.Role, Speciality = model.specialty});
        return newAccount.Id;
    }
}