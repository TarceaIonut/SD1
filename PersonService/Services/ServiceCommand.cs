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

    public override Task<newPersonReply> newPerson(newPersonRequest request, ServerCallContext context) {
        var newId = _dbContext.NewPerson(new Person
            {Email = request.Email, Role = (Person.UserRole)request.Role, specialty = request.Speciality});
        return Task.FromResult(new newPersonReply {Id = newId});
    }

}