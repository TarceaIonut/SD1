using Account.Serivice.Repositories;
using AccountDiffService;
using Grpc.Core;

namespace PersonService.Services;

public class ServiceQuery : Persons.PersonsBase {
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
    public override Task<newPersonReply> newPerson(newPersonRequest request, ServerCallContext context) {
        var newId = _dbContext.NewPerson(new Person
            {Email = request.Email, Role = (Person.UserRole)request.Role, specialty = request.Speciality});
        return Task.FromResult(new newPersonReply {Id = newId});
    }

    public override Task<removePersonByIdResponse> removePersonById(removePersonByIdRequest request, ServerCallContext context)
    {
        return Task.FromResult(new removePersonByIdResponse{Deleted = _dbContext.RemoveId(request.Id)});
    }
}