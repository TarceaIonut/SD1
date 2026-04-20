namespace Hospital.Models;

public interface IUserService {
    public Person.UserRole? GetRole();
    public string? GetEmail();
    public string? GetUser();
    public void SetRole(Person.UserRole role) => SetRole(role.ToString());
    public void SetRole(string role);
    public void SetEmail(string email);
    public void SetUser(string user);
    public int? GetUserId();
    public void SetUserId(int id);

    public void SetInformation(int id, string role, string email, string user) {
        SetUserId(id);
        SetRole(role);
        SetEmail(email);
        SetUser(user);
    }
}
public class UserService : IUserService {
    private readonly IHttpContextAccessor _context;
    public UserService(IHttpContextAccessor context) => _context = context;
    public Person.UserRole? GetRole() {
        var v = _context.HttpContext?.Session.GetString("UserRole");
        if (v == null) return null;
        return v! switch {
            "Admin" => Person.UserRole.Admin, "Doctor" => Person.UserRole.Doctor, "Patient" => Person.UserRole.Patient,
            _ => null
        };
    }
    public string? GetEmail() => _context.HttpContext?.Session.GetString("UserEmail");
    public string? GetUser() => _context.HttpContext?.Session.GetString("UserName");
    public void SetRole(string role) =>  _context.HttpContext?.Session.SetString("UserRole", role);
    public void SetEmail(string email) =>  _context.HttpContext?.Session.SetString("UserEmail", email);
    public void SetUser(string user) =>  _context.HttpContext?.Session.SetString("UserName", user);
    public int? GetUserId() => _context.HttpContext?.Session.GetInt32("UserId");
    public void SetUserId(int id) =>  _context.HttpContext?.Session.SetInt32("UserId", id);
}