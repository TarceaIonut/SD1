using Account.Serivice.Repositories;

namespace PersonService.Services;

using Grpc.Core;
using AccountDiffService;
public class ServiceCommand : Persons.PersonsBase
{
    private readonly AppDbContext _dbContext;

    public ServiceCommand(AppDbContext dbContext) {
        _dbContext = dbContext;
    }
    public override Task<getPersonDataByIdReplay> getPersonDataById(getPersonDataByIdRequest request, ServerCallContext context) {
        var p = _dbContext.GetPersonById(request.Id);
        if (p == null) {
            return Task.FromResult(new getPersonDataByIdReplay{Found =  false});
        }
        string? s = null;
        if (p is Doctor) {
            s = ((Doctor)p).Specialty;
        }
        return Task.FromResult(new getPersonDataByIdReplay{Found = true, Email =  p.Email, Role = (uint)p.Role, Specialty = s});
    }
}