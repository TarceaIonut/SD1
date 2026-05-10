using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceQuery : Greeter.GreeterBase {
    private readonly Persons.PersonsClient _personsClient;
    private readonly AccountRepository _repository;
    private readonly ILogger<ServiceQuery> _logger;
    public ServiceQuery(Persons.PersonsClient personsClient, AccountRepository repository, ILogger<ServiceQuery> logger) {
        _repository = repository;
        _personsClient = personsClient;
        _logger = logger;
    }
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
        return Task.FromResult(new HelloReply {
            Message = "Hello " + request.Name
        });
    }
    public override Task<ListAllAccountsReply> ListAllAccounts(ListAllAccountsRequest r, ServerCallContext context) {
        _logger.LogInformation("ListAllAccounts rcg command ");
        var a = _repository.findAll();
        var reply = new ListAllAccountsReply();
        foreach(var acc in a) {
            var p = _personsClient.getPersonDataById(new getPersonDataByIdRequest {Id = acc.personId} );
            if (p == null) {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }
            reply.Accounts.Add(new AccountFullInfo
            {
                Id = acc.Id,
                Email = p.Email,
                Username = acc.Username,
                Role = p.Role,
            });
        }
        return Task.FromResult(reply);
    }
    public override Task<AccountUserPassReply> findAccount(AccountUserPassRequest r, ServerCallContext context) {
        return Task.FromResult(new AccountUserPassReply());
    }
    public override Task<getAccountByIdReply> getAccountById(getAccountByIdRequest r, ServerCallContext context) {
        return Task.FromResult(new getAccountByIdReply());
    }
}