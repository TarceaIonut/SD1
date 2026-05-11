namespace DoctorCheckupsService.Models;

using System.ComponentModel.DataAnnotations;


public class DoctorCheckup
{
    public int Id  { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime AppointmentDate { get; set; }
    public string Description { get; set; }
    public DoctorCheckup() { }
}