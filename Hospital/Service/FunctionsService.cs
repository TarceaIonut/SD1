using Hospital.Models;
using Hospital.Models.ViewModels;
using Hospital.Repositories;

namespace Hospital.Service;

public class FunctionsService
{
    private readonly FunctionsRepository _repository;
    private readonly IUserService  _userService;
    public FunctionsService(FunctionsRepository repository, IUserService userService) {
        _repository = repository;
        _userService = userService;
    }

    private string nullableSring(string? s) {
        if  (s == null) return "";
        return s!;
    }

    public DoctorCheckupVewModel GetDoctorCheckups() => new(_repository.Doctors(),  _repository.Patients());
    public Result<DoctorCheckups, string> NewCheckup(DoctorCheckupVewModel doctorCheckupVewModel) {
        Doctor? doc = _repository.DoctorFind(doctorCheckupVewModel.DoctorEmail!, doctorCheckupVewModel.DoctorName!);
        Patient? patient = _repository.PatientFind(doctorCheckupVewModel.PatientEmail, doctorCheckupVewModel.PatientName);
        if (doc == null) return Result<DoctorCheckups, string>.Error_("could not find doctor: Email = " 
            + doctorCheckupVewModel.DoctorEmail + "User = " + doctorCheckupVewModel.DoctorName);
        if (patient == null) return Result<DoctorCheckups, string>.Error_("could not find patient: Email = " 
             + doctorCheckupVewModel.PatientEmail + "User = " + doctorCheckupVewModel.PatientName);
        var v = _repository.NewCheckup(new DoctorCheckups(patient, doc, doctorCheckupVewModel.AppointmentDate,  doctorCheckupVewModel.Description));
        var r = Result<DoctorCheckups, string>.ResultIf
            (v, "unknown error: at add new Checkup: saveChanges > 0 == false", v != null);
        if (r.HasValue()) {
            string email_description = r.Value!.GetInfo();
            sendEvents(r.Value!.Doctor, email_description, EventType.CREATE);
            sendEvents(r.Value!.Patient, email_description, EventType.CREATE);
        }
        return r;
    }
    public List<DoctorCheckups> DoctorCheckupsList() => _repository.DoctorCheckupsList();
    public List<DoctorCheckups> DoctorCheckupsListDoctor(Doctor doctor) => _repository.DoctorCheckupsListDoctor(doctor);
    public List<DoctorCheckups> DoctorCheckupsListPatient(Patient patient) => _repository.DoctorCheckupsListPatient(patient);
    public bool DoctorCheckupsDelete(int id) {
        var v = _repository.DoctorCheckupsDelete(id);
        if (v != null) {
            string email_description = v.GetInfo();
            sendEvents(v.Doctor, email_description, EventType.DELETE);
            sendEvents(v.Patient, email_description, EventType.DELETE);
        }
        return v != null;
    }

    public string? DoctorCheckupsUpdate(int id, string description, DateTime date) {
        DoctorCheckups? dc_og = _repository.FindCheckup(id);
        if (dc_og == null) return null;
        DoctorCheckups dc_new = dc_og;
        dc_new.Description = description;
        dc_new.AppointmentDate = date;
        var r = _repository.DoctorCheckupsUpdate(dc_new);
        if (r) {
            string email_description = dc_og.GetInfo() + "\n" + dc_new.GetInfo();
            sendEvents(dc_og.Doctor, email_description, EventType.UPDATE);
            sendEvents(dc_og.Patient, email_description, EventType.UPDATE);
        }
        if (r) return null;
        return "Cound not update: IDK why";
    }

    private void sendEvents(Person person, string description, EventType type) {
        EventBroker.Instance.NewEvent_CRUD(type, description, person.Email, person.Account.Username);
    }

    public string? DoctorCheckupsSort(List<DoctorCheckups> list, DoctorCheckupsFunctions.SortOrder? sortOrder,
        DoctorCheckupsFunctions.SortOn? sortOn) {
        if (sortOrder == null) return "sortOrder is null";
        if (sortOn == null) return "sortOn is null";
        switch (sortOn, sortOrder) {
            case (DoctorCheckupsFunctions.SortOn.DATE, DoctorCheckupsFunctions.SortOrder.ASC):
                list.Sort((x, y) => x.AppointmentDate.CompareTo(y.AppointmentDate));
                break;

            case (DoctorCheckupsFunctions.SortOn.DATE, DoctorCheckupsFunctions.SortOrder.DESC):
                list.Sort((x, y) => y.AppointmentDate.CompareTo(x.AppointmentDate));
                break;
            case (DoctorCheckupsFunctions.SortOn.NAME, DoctorCheckupsFunctions.SortOrder.ASC):
                list.Sort((x, y) => 
                    String.Compare(x.Patient.Account.Username,y.Patient.Account.Username, StringComparison.Ordinal));
                break;
            case (DoctorCheckupsFunctions.SortOn.NAME, DoctorCheckupsFunctions.SortOrder.DESC):
                list.Sort((x, y) => 
                    String.Compare(y.Patient.Account.Username,x.Patient.Account.Username, StringComparison.Ordinal));
                break;
            
        }
        return null;
    }
}