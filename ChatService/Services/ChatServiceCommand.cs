using Account.Serivice.Repositories;
using AccountDiffService;
using ChatService.Models;
using Grpc.Core;

namespace ChatService.Services;

public class ChatServiceCommand(AppDbContext _context) : ChatWrite.ChatWriteBase
{
    public override Task<writeMessageResponse> writeMessage(writeMessageRequest request, ServerCallContext context)
    {
        _context.AddMessage(new Message
        {
            Content = request.Message, ReceiverId = request.ReceiverId, SenderId = request.InitiatorId,
            Date = DateTime.UtcNow
        });
        return Task.FromResult(new writeMessageResponse());
    }
}