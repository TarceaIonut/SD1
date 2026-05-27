using Microsoft.AspNetCore.SignalR;

namespace Hospital.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChatSession(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}