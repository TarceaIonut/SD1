using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Hospital.Models.ViewModels;

public class DoctorCheckupVewModel {
    public string? DoctorName { get; set; }
    public string? DoctorEmail { get; set; }
    [Required]
    public string PatientName { get; set; }
    [Required]
    public string PatientEmail { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }
    
    public string Description { get; set; }

    public bool canSelectDoctor = false;
    public bool canSelect = false;
    
    public List<Doctor> Doctors = new List<Doctor>();
    public List<Patient> Patients = new List<Patient>();

    public DoctorCheckupVewModel(List<Doctor> doctors, List<Patient> patients) {
        Doctors = doctors;
        Patients = patients;
        canSelectDoctor = true;
        canSelect =  true;
    }

    public DoctorCheckupVewModel(DoctorCheckups doctorCheckups) {
        this.canSelectDoctor = true;
        this.canSelect = true;
        this.DoctorEmail = doctorCheckups.Doctor.Email;
        this.DoctorName = doctorCheckups.Doctor.Account.Username;
        this.PatientEmail = doctorCheckups.Patient.Email;
        this.PatientName = doctorCheckups.Patient.Account.Username;
    }
    public static DoctorCheckupVewModel EmptyInstance() {
        var v = new DoctorCheckupVewModel(new List<Doctor>(), new List<Patient>());
        v.canSelectDoctor = false;
        v.canSelect =  false;
        return v;
    }
    public DoctorCheckupVewModel() {}
    public override string ToString() {
        return JsonSerializer.Serialize(this);
    }
}