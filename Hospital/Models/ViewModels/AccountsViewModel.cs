using System.ComponentModel.DataAnnotations;
using Hospital.Models.HelperStructures;
using Hospital.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Hospital.Models.ViewModels;

public class AccountsViewModel {
    [ValidateNever]
    public List<Person>? _persons  {get;set;}
    public Person.UserRole? UserRole {get;set;} 
    public DoctorCheckupsFunctions.SortOrder? SortOrder {get;set;}
    public Formats? Format {get;set;}
    public bool Show {get;set;} = true;

    public AccountsViewModel(List<Person> persons, Person.UserRole? userRole, DoctorCheckupsFunctions.SortOrder? sortOrder,  Formats? format) {
        _persons = persons;
        UserRole = userRole;
        SortOrder = sortOrder;
        Format = format;
    }

    public AccountsViewModel(List<Person> persons) {
        _persons = persons;
    }

    public AccountsViewModel() {
        Show = false;
    }
}
