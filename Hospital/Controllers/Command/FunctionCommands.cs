using System.Windows.Input;
using AccountDiffService;
using Hospital.Models;
using Google.Protobuf.WellKnownTypes;
using Hospital.Models.ViewModels;

namespace Hospital.Controllers.Command;

public class NewCheckupCommand : ICommand<bool, DoctorCheckupVewModel>
{
    private readonly DoctorCheckupWrite.DoctorCheckupWriteClient _client;

    public NewCheckupCommand(DoctorCheckupWrite.DoctorCheckupWriteClient client) {
        _client = client;
    }
    
    public async Task<bool> ExecuteAsync(DoctorCheckupVewModel dc)
    {
        var r = _client.Insert(new InsertCheckupRequest {
            CheckupDate = DateTime.SpecifyKind(dc.AppointmentDate, DateTimeKind.Utc).ToTimestamp(),
            Description = dc.Description,
            DoctorEmail = dc.DoctorEmail,
            DoctorName = dc.DoctorName,
            PatientEmail = dc.PatientEmail,
            PatientName = dc.PatientName,
        });
        if (r == null)
            throw new Exception("could not insert");
        return true;
    }
}

public class ListAllCommand : ICommand<List<DoctorCheckups>> {
    private readonly DoctorCheckupRead.DoctorCheckupReadClient _client;

    public ListAllCommand(DoctorCheckupRead.DoctorCheckupReadClient client) {
        _client = client;
    }
    public async Task<List<DoctorCheckups>> ExecuteAsync()
    {
        var all = _client.GetAllDoctorCheckups(new GetAllRequest());
        var l = new List<DoctorCheckups>();
        foreach (var v in all.DoctorCheckups)
        {
            l.Add(new DoctorCheckups
            {
                AppointmentDate = v.CheckupDate.ToDateTime(),
                Description = v.Description,
                Id = v.Id,
                Doctor = new Doctor{Specialty = v.Speciality, Email = v.DoctorEmail, Account = new Account{Username =  v.PatientEmail}},
                Patient = new Patient{Email = v.PatientEmail, Account = new Account{Username =  v.PatientName}}
            });
        }

        return l;
    }
}

public class DeleteCommand : ICommand<bool, int>
{
    private readonly DoctorCheckupWrite.DoctorCheckupWriteClient _client;

    public DeleteCommand(DoctorCheckupWrite.DoctorCheckupWriteClient client) {
        _client = client;
    }
    public async Task<bool> ExecuteAsync(int g)
    {
        var v = _client.Delete(new deleteRequest { Id = g });
        return v != null;
    }
}
public class UpdateCommand : ICommand<bool, DoctorCheckupsFunctions>
{
    private readonly DoctorCheckupWrite.DoctorCheckupWriteClient _client;

    public UpdateCommand(DoctorCheckupWrite.DoctorCheckupWriteClient client) {
        _client = client;
    }
    public async Task<bool> ExecuteAsync(DoctorCheckupsFunctions g) {
        var v = _client.Update(new UpdateDoctorCheckupRequest
        {
            CheckupDate = g.AppointmentDate.Value.ToTimestamp(),
            Description = g.Description,
            Id = g.Id.Value
        });
        return v != null;
    }
}