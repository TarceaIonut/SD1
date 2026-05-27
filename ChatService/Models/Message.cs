namespace ChatService.Models;

public class Message
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime Date { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
}