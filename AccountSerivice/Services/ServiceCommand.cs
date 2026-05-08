


using AccountDiffService;
using Grpc.Core;

namespace Account.Serivice.Services;

public class ServiceCommand : Greeter.GreeterBase {
    public override Task<newAccountReply> newAccount(newAccountRequest r, ServerCallContext context) {
        return Task.FromResult(new newAccountReply());
    }
    public override Task<deleteByIdReply> deleteById(deleteByIdRequest r, ServerCallContext context) {
        return Task.FromResult(new deleteByIdReply());
    }
}