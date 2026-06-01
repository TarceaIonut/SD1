using Account.Serivice.Repositories;
using AccountDiffService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ChatService.Services;

public class ChatServiceQuery(AccountServiceRead.AccountServiceReadClient _accountRead, AppDbContext _context) : ChatRead.ChatReadBase
{
    public override Task<GetAllCurrentAccountsResponse> GetAllCurrentAccounts(GetAllCurrentAccountsRequest request, ServerCallContext context)
    {
        try
        {
            var l = _context.getAllAccountIdsByCurrentUser(request.Id);
            var response = new GetAllCurrentAccountsResponse();
            foreach (var id in l) {
                var accountFullInfo = _accountRead.getAccountById(new getAccountByIdRequest { Id = (uint)id }).Result;
                response.Accounts.Add(accountFullInfo);
            }
            return Task.FromResult(response);
        }catch(Exception e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message));
        }

    }
    public override Task<GetChatsResponce> GetChats(getChatsRequest request, ServerCallContext context)
    {
        var l = _context.GetChats(request.InitiatorId, request.ReceiverId);
        var responce = new GetChatsResponce();
        foreach (var message in l) {
            responce.Messages.Add(new ChatMessage
            {
                Date = DateTime.SpecifyKind(message.Date, DateTimeKind.Utc).ToTimestamp(), Message =  message.Content,
                ReceiverId = message.ReceiverId, SenderId =  message.SenderId
            });
        }
        return Task.FromResult(responce);
    }
}