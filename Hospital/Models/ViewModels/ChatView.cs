using Hospital.Controllers.Command;

namespace Hospital.Models.ViewModels;

public class ChatView {
    public class MessageView {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool SetByUser { get; set; }
    }
    public List<MessageView> Messages { get; set; } = new();
    public List<AccountPrint> Accounts { get; set; } = new();
    public string UserName { get; set; } = "";
    public string MessageNewUser { get; set; } = "";
    public string MessageCurrentUser { get; set; } = "";
}