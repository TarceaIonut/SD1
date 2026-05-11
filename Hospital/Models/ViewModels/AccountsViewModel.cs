using System.ComponentModel.DataAnnotations;
using Hospital.Controllers.Command;
using Hospital.Models.HelperStructures;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Hospital.Models.ViewModels;

public class AccountsViewModel {
    [ValidateNever]
    public List<AccountPrint>? _persons  {get;set;}
    public Person.UserRole? UserRole {get;set;} 
    public DoctorCheckupsFunctions.SortOrder? SortOrder {get;set;}
    public Formats? Format {get;set;}
    public bool Show {get;set;} = true;

    public AccountsViewModel(List<AccountPrint> persons, Person.UserRole? userRole, DoctorCheckupsFunctions.SortOrder? sortOrder,  Formats? format) {
        _persons = persons;
        UserRole = userRole;
        SortOrder = sortOrder;
        Format = format;
    }

    public AccountsViewModel(List<AccountPrint> accounts) {
        _persons = accounts;
    }

    public AccountsViewModel() {
        Show = false;
    }
}
