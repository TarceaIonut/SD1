namespace Hospital.Models.ViewModels;

using System.ComponentModel.DataAnnotations;

public class SignUpModel
{
    [Required]
    public string name { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }
    [Required]
    public string email { get; set; }
    [Required]
    public Person.UserRole Role { get; set; }
    public string? specialty { get; set; }

    public Result<Account, string> ToPerson() {
        return this.Role switch {
            Person.UserRole.Admin => Result<Account, string>.Success(
                new Account(name, password, new Admin(email, Role))),
            Person.UserRole.Doctor => Result<Account, string>.ResultIf(
                new(name, password, new Doctor(email, Role, specialty)), 
                "speciality field required", 
                specialty != null),
            _ => Result<Account, string>.Success(
                new Account(name, password, new Patient(email, Role))),
        };
    }
}