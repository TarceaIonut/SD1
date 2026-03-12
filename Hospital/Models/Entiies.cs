using System.ComponentModel.DataAnnotations.Schema;
using Hospital.Models;
namespace Hospital.Models
{
    public abstract class Person : IVisitor {
        public enum UserRole
        {
            Admin = 1,
            Doctor = 2,
            Patient = 3
        }
        public int Id { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public Account Account { get; set; }
        protected void SetPerson(string email, UserRole role) {
            Email = email;
            Role = role;
        }
        public override string ToString() {
            return "Email = " +  Email +  "; Role = " + Role;
        }
    }
    public class Doctor : Person, IDoctor {
        public List<DoctorCheckups> DoctorCheckups { get; set; }
        protected Doctor() {}
        public Doctor(string email, UserRole role, string? specialty) {
            base.SetPerson(email, role);
            Specialty = specialty;
        }
        public string Specialty { get; set; }
    }

    public class Admin : Person, IAdministrator {
        protected Admin() {}
        public Admin(string email, UserRole role) {
            base.SetPerson(email, role);
        }
    }

    public class Patient : Person, IPatient {
        public List<DoctorCheckups> DoctorCheckups { get; set; }
        protected Patient() {}
        public Patient(string email, UserRole role) {
            base.SetPerson(email, role);
        }
    }
}