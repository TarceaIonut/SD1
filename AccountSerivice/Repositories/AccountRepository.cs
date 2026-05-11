namespace Account.Serivice.Repositories;

public class AccountRepository
{
    private readonly AppDbContext _appDbContext;
    public AccountRepository(AppDbContext appDbContext) {
        _appDbContext = appDbContext;
    }
    public List<Accounts> findAll() {
        return _appDbContext.Accounts.ToList();
    }
    public Accounts? GetAccountById(int id) {
        return _appDbContext.Accounts.FirstOrDefault(a => a.Id == id);
    }
    public bool AccountExists(string name) {
        return _appDbContext.Accounts.Any(a => a.Username == name);
    }
    public Accounts? GetAccountByNamePassword(string name, string password) {
        return _appDbContext.Accounts.FirstOrDefault(a => a.Username == name && a.Password == password);
    }
    public Accounts NewAccount(Accounts a) {
        var newA =  _appDbContext.Add(a).Entity;
        if (_appDbContext.SaveChanges() > 0) return  newA;
        throw new Exception("Account could not be created");
    }
    public bool AccountExistsUser (string user) => _appDbContext.Accounts.Any(a => a.Username == user);

    public bool RemoveAccount(Accounts a) {
        _appDbContext.Accounts.Remove(a);
        return (_appDbContext.SaveChanges() > 1);
    }
}