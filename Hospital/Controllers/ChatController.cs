using AccountDiffService;
using Hospital.Controllers.Command;
using Hospital.Hubs;
using Hospital.Models;
using Hospital.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Hospital.Controllers;

public class ChatController(
    ChatRead.ChatReadClient _client,
    ChatWrite.ChatWriteClient _clientWrite,
    AccountServiceRead.AccountServiceReadClient _accountClient,
    IHubContext<ChatHub> _hubContext,
    IUserService _userService) : Controller
{
    public IActionResult Chat()
    {
        int? taltingToId = _userService.GetMessageUserId();
        if (taltingToId != null)
            return ShowMessages(taltingToId.Value);
        var v = new ChatView();
        int? userId = _userService.GetUserId();
        if (userId == null)
        {
            ModelState.AddModelError("", "not signed in");
            return View(v);
        }

        int _id = userId.Value;
        var responce = _client.GetAllCurrentAccounts(new GetAllCurrentAccountsRequest { Id = _id }).Accounts;
        foreach (var account in responce)
        {
            v.Accounts.Add(new AccountPrint
            {
                Username = account.Username, Email = account.Email, Speciality = account.Speciality,
                role = (Person.UserRole)account.Role, Id = account.Id
            });
        }

        return View(v);
    }

    [HttpGet]
    public IActionResult ShowMessages(int id)
    {
        var v = new ChatView();
        int? userId = _userService.GetUserId();
        string? userName = _userService.GetUser();
        if (userId == null)
        {
            ModelState.AddModelError("", "not signed in");
            return View("Chat", v);
        }
        _userService.SetMessageUserId(id);
        var responce = _client.GetAllCurrentAccounts(new GetAllCurrentAccountsRequest { Id = userId.Value }).Accounts;
        string messageSender = "unknown";
        foreach (var account in responce)
        {
            if (account.Id == id)
                messageSender =  account.Username;
            v.Accounts.Add(new AccountPrint
            {
                Username = account.Username, Email = account.Email, Speciality = account.Speciality,
                role = (Person.UserRole)account.Role, Id = account.Id
            });
        }

        var messages = _client.GetChats(new getChatsRequest { InitiatorId = userId.Value, ReceiverId = id }).Messages;
        foreach (var message in messages)
        {
            v.Messages.Add(new ChatView.MessageView { Date = message.Date.ToDateTime(), Message = message.Message, 
                SentBy = message.SenderId == userId ? userName! : messageSender});
        }
        
        return View("Chat", v);
    }

    [HttpPost]
    public IActionResult NewConversation(ChatView model) {
        try {
            
            if (model.UserName == "")
                throw new Exception("user name can not be empty:");
            var id = _accountClient.getByUser(new AccountExistsUserRequest { Username = model.UserName }).Result;
            if (id == null)
                throw new Exception("Account not found");
            int? userId = _userService.GetUserId();
            if (userId == null) 
                throw new Exception("Not signed in");
            _userService.SetMessageUserId(id.Id);
            _clientWrite.writeMessage(new writeMessageRequest {
                InitiatorId = userId.Value, ReceiverId = id.Id,
                Message = model.MessageNewUser
            });
            var responce = _client.GetAllCurrentAccounts(new GetAllCurrentAccountsRequest 
                { Id = userId.Value }).Accounts;
            foreach (var account in responce) {
                model.Accounts.Add(new AccountPrint {
                    Username = account.Username, Email = account.Email, Speciality = account.Speciality,
                    role = (Person.UserRole)account.Role, Id = account.Id
                });
            }
            var messages = _client.GetChats(new getChatsRequest 
                { InitiatorId = userId.Value, ReceiverId = id.Id }).Messages;
            foreach (var message in messages) {
                model.Messages.Add(new ChatView.MessageView { Date = message.Date.ToDateTime(), Message = message.Message });
            }
        }
        catch (Exception e) {
            ModelState.AddModelError("", e.Message);
        }
        return View("Chat", model);
    }

    [HttpPost]
    public async Task<IActionResult> NewMessage(ChatView model) {
        try {
            int? currentId = _userService.GetMessageUserId();
            if (currentId == null) throw new Exception("user to talk to not found");
            int? userId = _userService.GetUserId();
            if (userId == null)
                throw new Exception("Not signed in");
            _clientWrite.writeMessage(new writeMessageRequest
                { InitiatorId = userId.Value, ReceiverId = currentId.Value, Message = model.MessageCurrentUser });
            var responce = _client.GetAllCurrentAccounts(new GetAllCurrentAccountsRequest 
                { Id = userId.Value }).Accounts;
            foreach (var account in responce) {
                model.Accounts.Add(new AccountPrint {
                    Username = account.Username, Email = account.Email, Speciality = account.Speciality,
                    role = (Person.UserRole)account.Role, Id = account.Id
                });
            }
            var messages = _client.GetChats(new getChatsRequest 
                { InitiatorId = userId.Value, ReceiverId = currentId.Value }).Messages;
            foreach (var message in messages) {
                model.Messages.Add(new ChatView.MessageView
                {
                    Date = message.Date.ToDateTime(), Message = message.Message,
                    
                });
            }
            string? userName = _userService.GetUser(); 
            if (userName == null) userName = "Unknown";

            string timestamp = DateTime.Now.ToString("g");
            
            await _hubContext.Clients.Group(currentId.Value.ToString())
                .SendAsync("ReceiveMessage", userId.Value, userName, model.MessageCurrentUser, timestamp);

            await _hubContext.Clients.Group(userId.Value.ToString())
                .SendAsync("ReceiveMessage", userId.Value, userName, model.MessageCurrentUser, timestamp);
        }catch (Exception e) {
            ModelState.AddModelError("", e.Message);
        }
        return View("Chat", model);
    }
}