using System.ComponentModel.DataAnnotations;
using Hospital.Models.HelperStructures;

namespace Hospital.Models.ViewModels;

public class DoctorCheckupsFunctions {
    public List<DoctorCheckups>  DoctorCheckups { get; set; }
    public bool Function = false;
    public int? Id { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime? AppointmentDate { get; set; }
    
    public string? Description { get; set; }
    public Formats? Format { get; set; }

    public enum SortOn {
        DATE,
        NAME 
    }

    public enum SortOrder {
        ASC,
        DESC
    }

    public SortOrder? sortOrder { get; set; }
    public SortOn? sortOn { get; set; }

    public DoctorCheckupsFunctions(List<DoctorCheckups> doctorCheckups) {
        DoctorCheckups = doctorCheckups;
        Function = false;
    }

    public DoctorCheckupsFunctions() {
        DoctorCheckups = new();
    }
    public DoctorCheckupsFunctions(List<DoctorCheckups> doctorCheckups, bool function) {
        DoctorCheckups = doctorCheckups;
        Function = function;
    }
}