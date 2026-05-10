using AccountDiffService;
using Hospital.Models;

namespace Hospital.Controllers.Command;
using Hospital.Controllers.Command;


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

public class GetAllAccountsCommand : ICommand<List<GetAllAccountsCommand.AccountPrint>> {
    private readonly Greeter.GreeterClient _grpcClient;
    private readonly ILogger<AccountController> _logger;
    public GetAllAccountsCommand(Greeter.GreeterClient grpcClient, ILogger<AccountController> logger) {
        _logger = logger;
        _grpcClient = grpcClient;
    }

    public class AccountPrint {
        public string Email { get; set; }
        public string Username { get; set; }
        public Person.UserRole role { get; set; }
        public AccountPrint(string email, string username, Person.UserRole role) {
            Email = email;
            Username = username;
            this.role = role;
        }
    }
    public async Task<List<AccountPrint>> ExecuteAsync() {
        _logger.LogInformation("ListAllAccounts rcg command started");
        var v = _grpcClient.ListAllAccounts(new ListAllAccountsRequest());
        var list = new List<AccountPrint>(v.Accounts.Count);
        foreach (AccountFullInfo account in v.Accounts) {
            list.Add(new AccountPrint(account.Email, account.Username, (Person.UserRole)account.Role));
        }
        return list;
    }
}