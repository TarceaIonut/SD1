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
        try
        {
            var l = _dbContext.GetDoctorCheckups();
            var r = new GetAllDoctorCheckupsReceive();
            foreach (var item in l)
            {
                getAccountByIdReply patient;
                getAccountByIdReply doctor;
                
                patient = _accountServiceRead.getAccountById(new getAccountByIdRequest { Id = (uint)item.PatientId });
                doctor = _accountServiceRead.getAccountById(new getAccountByIdRequest { Id = (uint)item.DoctorId });
                if (patient == null ||  doctor == null)
                    continue;
                if (patient?.Result == null || doctor?.Result == null)
                    continue;
                r.DoctorCheckups.Add(new DoctorCheckupSend
                {
                    CheckupDate = DateTime.SpecifyKind(item.AppointmentDate, DateTimeKind.Utc).ToTimestamp(),
                    Description = item.Description ?? string.Empty,
                    Id = item.Id,
                    DoctorEmail = doctor.Result.Email,
                    PatientEmail = patient.Result.Email,
                    DoctorName = doctor.Result.Username,
                    PatientName = patient.Result.Username,
                    Speciality = doctor.Result.Speciality
                });
            }
            return Task.FromResult(r);
        }
        catch(Exception e)
        {
            throw new RpcException(new Status(StatusCode.Internal, e.Message + e.StackTrace));
        }
        
    }
}