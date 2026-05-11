using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceQuery : Greeter.GreeterBase {
    private readonly Persons.PersonsClient _personsClient;
    private readonly AccountRepository _repository;
    public ServiceQuery(Persons.PersonsClient personsClient, AccountRepository repository) {
        _repository = repository;
        _personsClient = personsClient;
    }
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
        return Task.FromResult(new HelloReply {
            Message = "Hello " + request.Name
        });
    }
    public override Task<ListAllAccountsReply> ListAllAccounts(ListAllAccountsRequest r, ServerCallContext context) {
        Console.WriteLine("ListAllAccounts rcg command ");
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
                Speciality = p.Specialty
            });
        }
        return Task.FromResult(reply);
    }
    public override Task<AccountExistsUserReply> AccountExistsUser(AccountExistsUserRequest request, ServerCallContext context) {
        return Task.FromResult(new AccountExistsUserReply{Exists = _repository.AccountExistsUser(request.Username)});
    }

    public override Task<getAccountByIdReply> getAccountById(getAccountByIdRequest r, ServerCallContext context) {
        return Task.FromResult(new getAccountByIdReply());
    }
    public override Task<getAccountReply> getAccount(getAccountRequest request, ServerCallContext context) {
        var account = _repository.GetAccountByNamePassword(request.Username, request.Password);
        if (account == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Account Not found"));
        }
        var person = _personsClient.getPersonDataById(new getPersonDataByIdRequest {Id = account.personId} );
        if (person == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));
        }
        return Task.FromResult(new getAccountReply{Result = new AccountFullInfo
            {Email = person.Email, Id =  account.Id, Username = account.Username, Role = person.Role}});
    }
    public override Task<newAccountReply> newAccount(newAccountRequest r, ServerCallContext context) {
        Console.WriteLine("newAccount rcg command ");
        if (_repository.AccountExists(r.Username)) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Account already exists: password = " + r.Password));
        }
        var replyExist = _personsClient.personExistsEmail(new PersonExistsEmailRequest{Email = r.Email});
        if (replyExist.Exists) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Account already exists: email = " + r.Email));
        }
        var idPersonResponse = _personsClient.newPerson(new newPersonRequest{Email = r.Email, Role = r.Role, Speciality = r.Speciality});
        var new_account = new Accounts{Password =  r.Password, Username = r.Username, personId =  idPersonResponse.Id};
        var idAccountResponse = _repository.NewAccount(new_account);
        return Task.FromResult(new newAccountReply {
            Id = idAccountResponse.Id
        });
    }

    public override Task<deleteByIdReply> deleteById(deleteByIdRequest request, ServerCallContext context) {
        var account = _repository.GetAccountById((int)request.Id);
        if (account == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Account Not found"));
        }
        var person = _personsClient.getPersonDataById(new getPersonDataByIdRequest {Id = account.personId} );
        if (person == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));
        }
        var pD = _personsClient.removePersonById(new removePersonByIdRequest{Id = account.Id});
        var rA = _repository.RemoveAccount(account);
        
        return Task.FromResult(new deleteByIdReply{Result = (pD != null && rA)});
    }
}