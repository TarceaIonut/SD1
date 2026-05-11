using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceQuery : AccountServiceRead.AccountServiceReadBase {
    private readonly PersonsServiceRead.PersonsServiceReadClient _personsClientRead;
    private readonly AccountRepository _repository;
    public ServiceQuery(PersonsServiceRead.PersonsServiceReadClient personsClientRead, AccountRepository repository) {
        _repository = repository;
        _personsClientRead = personsClientRead;
    }
    public override Task<ListAllAccountsReply> ListAllAccounts(ListAllAccountsRequest r, ServerCallContext context) {
        Console.WriteLine("ListAllAccounts rcg command ");
        var a = _repository.findAll();
        var reply = new ListAllAccountsReply();
        foreach(var acc in a) {
            var p = _personsClientRead.getPersonDataById(new getPersonDataByIdRequest {Id = acc.personId} );
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

    public override Task<getAccountByIdReply> getAccountById(getAccountByIdRequest request, ServerCallContext context) {
        var account = _repository.GetAccountById((int)request.Id);
        if (account == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Account Not found"));
        }
        var person = _personsClientRead.getPersonDataById(new getPersonDataByIdRequest {Id = account.personId} );
        if (person == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));
        }
        return Task.FromResult(new getAccountByIdReply{Result = new AccountFullInfo
            {Email = person.Email, Id =  account.Id, Username = account.Username, Role = person.Role}});
    }
    public override Task<getAccountReply> getAccount(getAccountRequest request, ServerCallContext context) {
        var account = _repository.GetAccountByNamePassword(request.Username, request.Password);
        if (account == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Account Not found"));
        }
        var person = _personsClientRead.getPersonDataById(new getPersonDataByIdRequest {Id = account.personId} );
        if (person == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));
        }
        return Task.FromResult(new getAccountReply{Result = new AccountFullInfo
            {Email = person.Email, Id =  account.Id, Username = account.Username, Role = person.Role}});
    }

    public override Task<getAccountReply> getAccountByUserEmail(getAccountByUserEmailRequest request, ServerCallContext context)
    {
        var account = _repository.getByName(request.User);
        if (account == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Account Not found"));
        }
        var person = _personsClientRead.getPersonDataById(new getPersonDataByIdRequest {Id = account.personId} );
        if (person == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "Person not found"));
        }
        if (person.Email != request.Email) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Email could not be found : " + request.Email));
        }
        return Task.FromResult(new getAccountReply{Result = new AccountFullInfo
            {Email = person.Email, Id =  account.Id, Username = account.Username, Role = person.Role}});
    }


    
}