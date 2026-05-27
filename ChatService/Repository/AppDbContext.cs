
using AccountDiffService;
using ChatService.Models;
using Microsoft.EntityFrameworkCore;

namespace Account.Serivice.Repositories;


public class AppDbContext : DbContext
{
    
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Message> Messages { get; set; }

    public List<int> getAllAccountIdsByCurrentUser(int userId)
    {
        
        Console.WriteLine(Messages.ToList());
        return Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .AsEnumerable()
            .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
            .Where(id => id != userId)
            .Distinct()
            .ToList();
    }

    public List<Message> GetChats(int initiatorId, int receverId)
    {
        var m = Messages.Where(m => (m.SenderId == initiatorId && m.ReceiverId == receverId) || 
                            (m.SenderId == receverId && m.ReceiverId == initiatorId)).ToList();
        Console.WriteLine(m);
        return m;
    }
        

    public void AddMessage(Message message)
    {
        Console.WriteLine(message);
        Messages.Add(message);
        this.SaveChanges();
        Console.WriteLine(Messages.ToList());
    }
    
}