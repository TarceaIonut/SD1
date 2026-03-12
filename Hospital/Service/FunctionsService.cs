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
        Doctor? doc = _repository.DoctorFind(doctorCheckupVewModel.DoctorEmail, doctorCheckupVewModel.DoctorName);
        Patient? patient = _repository.PatientFind(doctorCheckupVewModel.PatientEmail, doctorCheckupVewModel.PatientName);
        if (doc == null) return Result<DoctorCheckups, string>.Error_("could not find doctor: Email = " 
            + doctorCheckupVewModel.DoctorEmail + "User = " + doctorCheckupVewModel.DoctorName);
        if (patient == null) return Result<DoctorCheckups, string>.Error_("could not find patient: Email = " 
             + doctorCheckupVewModel.PatientEmail + "User = " + doctorCheckupVewModel.PatientName);
        var v = _repository.NewCheckup(new DoctorCheckups(patient, doc));
        return Result<DoctorCheckups, string>.Success(v);
    }
}