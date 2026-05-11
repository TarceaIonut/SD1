using AccountDiffService;
using Grpc.Core;
using DoctorCheckupsService.Repositories;
using Google.Protobuf.WellKnownTypes;

namespace DoctorCheckupsService.Services;


public class ServiceRead : DoctorCheckupRead.DoctorCheckupReadBase
{
    private readonly AppDbContext _dbContext;
    private readonly AccountServiceRead.AccountServiceReadClient _accountServiceRead;

    public ServiceRead(AppDbContext dbContext, AccountServiceRead.AccountServiceReadClient accountServiceRead) {
        _accountServiceRead = accountServiceRead;
        _dbContext = dbContext;
    }
    public override Task<GetAllDoctorCheckupsReceive> GetAllDoctorCheckups(GetAllRequest request, ServerCallContext context)
    {
        var l = _dbContext.GetDoctorCheckups();
        var r = new GetAllDoctorCheckupsReceive();
        foreach (var item in l)
        {
            var patient = _accountServiceRead.getAccountById(new getAccountByIdRequest { Id = (uint)item.PatientId });
            var doctor = _accountServiceRead.getAccountById(new getAccountByIdRequest{Id = (uint)item.DoctorId});
            if (patient == null ||  doctor == null)
                continue;
            r.DoctorCheckups.Add(new DoctorCheckupSend
            {
                CheckupDate = item.AppointmentDate.ToTimestamp(),
                Description = item.Description,
                Id = item.Id,
                DoctorEmail = doctor.Result.Email,
                PatientEmail = patient.Result.Email,
                DoctorName = doctor.Result.Username,
                PatientName = patient.Result.Username
            });
        }
        return Task.FromResult(r);
    }
}