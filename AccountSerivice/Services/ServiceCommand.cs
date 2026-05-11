using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceCommand : AccountServiceWrite.AccountServiceWriteBase {
    private readonly PersonsServiceRead.PersonsServiceReadClient _personsClientRead;
    private readonly PersonsServiceWrite.PersonsServiceWriteClient _personsClientWrite;
    private readonly AccountRepository _repository;
    public ServiceCommand(AccountRepository repository, PersonsServiceRead.PersonsServiceReadClient personsClientRead
    , PersonsServiceWrite.PersonsServiceWriteClient personsClientWrite) {
        _personsClientRead = personsClientRead;
        _personsClientWrite =  personsClientWrite;
        _repository = repository;
    }
    public override Task<newAccountReply> newAccount(newAccountRequest r, ServerCallContext context) {
        Console.WriteLine("newAccount rcg command ");
        if (_repository.AccountExists(r.Username)) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Account already exists: password = " + r.Password));
        }
        var replyExist = _personsClientRead.personExistsEmail(new PersonExistsEmailRequest{Email = r.Email});
        if (replyExist.Exists) {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Account already exists: email = " + r.Email));
        }
        var idPersonResponse = _personsClientWrite.newPerson(new newPersonRequest{Email = r.Email, Role = r.Role, Speciality = r.Speciality});
        var new_account = new Accounts{Password =  r.Password, Username = r.Username, personId =  idPersonResponse.Id};
        var idAccountResponse = _repository.NewAccount(new_account);
        return Task.FromResult(new newAccountReply {
            Id = idAccountResponse.Id
        });
    }

    public override Task<deleteByIdReply> deleteById(deleteByIdRequest r, ServerCallContext context)
    {
        var account = _repository.GetAccountById((int)r.Id);
        if (account == null)
            throw new Exception("account by is not found: id = " + r.Id);
        var vv = _repository.DeleteId(account.Id);
        var dp = _personsClientWrite.removePersonById(new removePersonByIdRequest { Id = account.personId });
        if (dp.Deleted)
            throw new Exception("person could not be deleted: id = " + account.personId);
        Console.WriteLine("_repository.DeleteId(account.Id) = " + vv);
        return Task.FromResult(new deleteByIdReply { Result = true });
    }

}