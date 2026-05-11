using AccountDiffService;
using DoctorCheckupsService.Models;
using Grpc.Core;
using DoctorCheckupsService.Repositories;
namespace DoctorCheckupsService.Services;

public class ServiceWrite : DoctorCheckupWrite.DoctorCheckupWriteBase
{
    private readonly AppDbContext _dbContext;
    private readonly AccountServiceRead.AccountServiceReadClient _accountServiceRead;

    public ServiceWrite(AppDbContext dbContext, AccountServiceRead.AccountServiceReadClient accountServiceRead) {
        _accountServiceRead = accountServiceRead;
        _dbContext = dbContext;
    }
    public override Task<UpdateDoctorCheckupResponse> Update(UpdateDoctorCheckupRequest request, ServerCallContext context)
    {
        var dc = _dbContext.GetById(request.Id);
        if (dc == null) {
            throw new RpcException(new Status(StatusCode.NotFound, "doctor checkup Not found"));
        }
        dc.Description = request.Description;
        dc.AppointmentDate = request.CheckupDate.ToDateTime();
        _dbContext.UpdateDoctorCheckup(dc);
        return Task.FromResult(new UpdateDoctorCheckupResponse());
    }

    public override Task<deleteResponse> Delete(deleteRequest request, ServerCallContext context) {
        _dbContext.deleteById(request.Id);
        return Task.FromResult(new deleteResponse());
    }

    public override Task<InsertCheckupResponse> Insert(InsertCheckupRequest request, ServerCallContext context) {
        var patient = _accountServiceRead.getAccountByUserEmail(new getAccountByUserEmailRequest
            {User = request.PatientName, Email = request.PatientEmail});
        var doctor = _accountServiceRead.getAccountByUserEmail(new getAccountByUserEmailRequest
            {User = request.DoctorName, Email = request.DoctorEmail});
        if (patient == null)
            throw new Exception("patient not found");
        if (doctor == null)
            throw new Exception("doctor not found");
        var newDc = _dbContext.AddDoctorCheckup(new DoctorCheckup {
            Description = request.Description, AppointmentDate = request.CheckupDate.ToDateTime(),
            PatientId = patient.Result.Id, DoctorId = doctor.Result.Id
        });
        return Task.FromResult(new InsertCheckupResponse{Id = newDc});
    }
}