public class Person {
    public enum UserRole
    {
        Admin = 1,
        Doctor = 2,
        Patient = 3
    }
    public int Id { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public string? specialty { get; set; }
    
}
