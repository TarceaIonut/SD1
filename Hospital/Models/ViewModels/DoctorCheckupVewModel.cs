using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.ViewModels;

public class DoctorCheckupVewModel {
    [Required]
    public string DoctorName;
    [Required]
    public string DoctorEmail;
    [Required]
    public string PatientName;
    [Required]
    public string PatientEmail;

    public bool canSelectDoctor;
    
    public List<Doctor> Doctors;
    public List<Patient> Patients;

    public DoctorCheckupVewModel(List<Doctor> doctors, List<Patient> patients) {
        Doctors = doctors;
        Patients = patients;
    }
}