namespace Hospital.Models;

public class DoctorCheckups
{
    public int Id  { get; set; }
    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    
    protected DoctorCheckups() { }

    public DoctorCheckups(Patient patient, Doctor doctor) {
        Doctor = doctor;
        Patient = patient;
    }
}