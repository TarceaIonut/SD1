using Hospital.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Repositories {
    public class AccountRepository {
        public readonly AppDbContext _context;
        public AccountRepository(AppDbContext context) {
            _context = context;
        }
        public Pair<Person, Account>? FindByNamePass(string name, string password) {
            var a = _context.Accounts.Include(a => a.person).
                FirstOrDefault(a => a.Username == name && a.Password == password);
            if  (a == null) return null;
            return new Pair<Person, Account>(a.person , a);
        }
        public bool PersonExists(string email) {
            return _context.Persons.FirstOrDefault(a => a.Email == email) != null;
        }

        public bool AccountExists(string name, string password) {
            Account a = new Account(name, password);
            return _context.Accounts.
                FirstOrDefault(account => account.Username == a.Username) != null;
        }
        public bool AccountExists(Account account) {
            return AccountExists(account.Username, account.Password);
        }
        public bool NewAccount(Account account, Person person) {
            if (AccountExists(account) || PersonExists(person.Email)) return false;
            _context.Accounts.Add(account);
            _context.Persons.Add(person);
            int nr = _context.SaveChanges();
            return nr == 2;
        }

    }
}
