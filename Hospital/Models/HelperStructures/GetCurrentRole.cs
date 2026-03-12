namespace Hospital.Models;

public class GetCurrentRole
{
    private readonly HttpContext _context;

    public Person.UserRole? Role() {
        return Role(_context);
    }
    public static Person.UserRole? Role(HttpContext context) {
        var v = context.Session.GetString("UserRole");
        if (v == null) return null;
        return v! switch {
            "Admin" => Person.UserRole.Admin, "Doctor" => Person.UserRole.Doctor, "Patient" => Person.UserRole.Patient,
            _ => null
        };
    }
}