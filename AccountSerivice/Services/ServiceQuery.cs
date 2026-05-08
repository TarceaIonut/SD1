using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceQuery : Greeter.GreeterBase {
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context) {
        return Task.FromResult(new HelloReply {
            Message = "Hello " + request.Name
        });
    }
    public override Task<ListAllAccountsReply> ListAllAccounts(ListAllAccountsRequest r, ServerCallContext context) {
        return Task.FromResult(new ListAllAccountsReply());
    }
    public override Task<AccountUserPassReply> findAccount(AccountUserPassRequest r, ServerCallContext context) {
        return Task.FromResult(new AccountUserPassReply());
    }
    public override Task<getAccountByIdReply> getAccountById(getAccountByIdRequest r, ServerCallContext context) {
        return Task.FromResult(new getAccountByIdReply());
    }
}