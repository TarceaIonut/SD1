using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace PersonService.Services;

public class ServiceQuery : PersonsServiceRead.PersonsServiceReadBase {
    private readonly AppDbContext _dbContext;
    public ServiceQuery(AppDbContext dbContext) {
        _dbContext = dbContext;
    }

    public override Task<PersonExistsEmailReply> personExistsEmail(PersonExistsEmailRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PersonExistsEmailReply{Exists = _dbContext.PersonExistsByEmail(request.Email)});
    }

    public override Task<getPersonDataByIdReplay> getPersonDataById(getPersonDataByIdRequest request, ServerCallContext context) {
        var p = _dbContext.GetPersonById(request.Id);
        if (p == null) {
            return Task.FromResult(new getPersonDataByIdReplay{Found = false});
        }
        return Task.FromResult(new getPersonDataByIdReplay{Found = true, Email =  p.Email, Role = (uint)p.Role, Specialty = p.specialty});
    }
}