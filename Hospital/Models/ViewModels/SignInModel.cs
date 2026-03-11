using System.ComponentModel.DataAnnotations;

namespace Hospital.Models.ViewModels;

public class SignInModel
{
    [Required]
    public string name { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string password { get; set; }
}