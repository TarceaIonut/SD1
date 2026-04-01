using System.ComponentModel.DataAnnotations;

namespace Hospital.Models;

public class DoctorCheckups
{
    public int Id  { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }
    
    public string Description { get; set; }
    protected DoctorCheckups() { }

    public DoctorCheckups(Patient patient, Doctor doctor, DateTime appointmentDate, string description) {
        Doctor = doctor;
        Patient = patient;
        AppointmentDate = appointmentDate;
        Description = description;
    }

    public string GetInfo() =>
        $"DoctorCheckup: AppointmentDate = {AppointmentDate.ToShortDateString()}, Description = {Description}, \n" +
               $"DoctorName = {this.Doctor.Account.Username}, DoctorEmail = {this.Doctor.Email}, Speciality = {this.Doctor.Specialty} \n" +
        $"PatientName = {Patient.Account.Username}, PatientEmail = {Patient.Email}" ;
}