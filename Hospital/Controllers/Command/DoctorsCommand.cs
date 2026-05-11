using AccountDiffService;
using Hospital.Models;

namespace Hospital.Controllers.Command;

public class DoctorsCommand : ICommand<List<AccountPrint>>
{
    private readonly AccountServiceRead.AccountServiceReadClient _accountRead;
    public DoctorsCommand( AccountServiceRead.AccountServiceReadClient accountRead) {
        _accountRead = accountRead;
    }
    public async Task<List<AccountPrint>> ExecuteAsync()
    {
        var v = _accountRead.ListAllAccounts(new ListAllAccountsRequest());
        var list = new List<AccountPrint>(v.Accounts.Count);
        foreach (AccountFullInfo account in v.Accounts)
        {
            if ((Person.UserRole)account.Role == Person.UserRole.Doctor)
            {
                list.Add(new AccountPrint
                {
                    Email = account.Email, Speciality = account.Speciality, Username = account.Username,
                    role = (Person.UserRole)account.Role
                });
            }
        }
        return list;
    }
}