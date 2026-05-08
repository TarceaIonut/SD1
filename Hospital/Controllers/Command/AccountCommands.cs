using AccountDiffService;

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