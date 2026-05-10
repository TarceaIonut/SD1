using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceCommand : Greeter.GreeterBase {
    private readonly Persons.PersonsClient _personsClient;
    private readonly AccountRepository _repository;
    public ServiceCommand(AccountRepository repository, Persons.PersonsClient personsClient) {
        _personsClient =  personsClient;
        _repository = repository;
    }
    public override Task<newAccountReply> newAccount(newAccountRequest r, ServerCallContext context) {
        Console.WriteLine("newAccount rcg command ");
        if (_repository.AccountExists(r.Username)) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Account already exists: password = " + r.Password));
        }
        var replyExist = _personsClient.PersonExistsByEmail(new PersonExistsByEmailRequest{Email = r.Email});
        if (replyExist.Result) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Account already exists: email = " + r.Email));
        }
        var idPersonResponse = _personsClient.newPerson(new newPersonRequest{Email = r.Email, Role = r.Role, Speciality = r.Speciality});
        var new_account = new Accounts{Password =  r.Password, Username = r.Username, personId =  idPersonResponse.Id};
        var idAccountResponse = _repository.NewAccount(new_account);
        return Task.FromResult(new newAccountReply {
            Id = idAccountResponse.Id
        });
    }
    public override Task<deleteByIdReply> deleteById(deleteByIdRequest r, ServerCallContext context) {
        return Task.FromResult(new deleteByIdReply());
    }
}