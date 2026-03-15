using Hospital.Models;
using Hospital.Models.ViewModels;
using Hospital.Repositories;

namespace Hospital.Service;

public class FunctionsService
{
    private readonly FunctionsRepository _repository;
    public FunctionsService(FunctionsRepository repository) {
        _repository = repository;
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
        return Result<DoctorCheckups, string>.ResultIf(v, "unknown error: at add new Checkup: saveChanges > 0 == false", v != null);
    }
    public List<DoctorCheckups> DoctorCheckupsList() => _repository.DoctorCheckupsList();
    public List<DoctorCheckups> DoctorCheckupsListDoctor(Doctor doctor) => _repository.DoctorCheckupsListDoctor(doctor);
    public List<DoctorCheckups> DoctorCheckupsListPatient(Patient patient) => _repository.DoctorCheckupsListPatient(patient);
    public bool DoctorCheckupsDelete(int id) =>  _repository.DoctorCheckupsDelete(id);

    public string? DoctorCheckupsUpdate(int id, string description, DateTime date) {
        DoctorCheckups? dc = _repository.FindCheckup(id);
        dc!.Description = description;
        dc.AppointmentDate = date;
        var r = _repository.DoctorCheckupsUpdate(dc);
        if (r) return null;
        return "Cound not update: IDK why";
    }
}