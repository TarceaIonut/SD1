public class Accounts
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public int personId { get; set; }
    public Person person { get; set; }

    public Accounts()
    {
    }

    public Accounts(string username, string password)
    {
        this.Username = username;
        this.Password = password;
    }
    

    public Accounts(string username, string password, int personId) {
        this.Username = username;
        this.Password = password;
        this.personId = personId;
    }

    public bool Equals(Accounts account)
    {
        return this.Username == account.Username && this.Password == account.Password;
    }

    public Accounts(string username, string password, Person person)
    {
        this.Username = username;
        this.Password = password;
        this.person = person;
    }

    public override string ToString()
    {
        return "Username = " + Username + " Password = " + Password;
    }
}