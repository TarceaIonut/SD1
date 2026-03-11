namespace Hospital.Models { 
    public class Account
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public int personId { get; set; }
        public Person person { get; set; }
        
        protected Account() {}

        public Account(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public bool Equals(Account account)
        {
            return this.Username == account.Username &&  this.Password == account.Password;
        }
        public  Account(string username, string password, Person person)
        {
            this.Username = username;
            this.Password = password;
            this.person = person;
        }

        public override string ToString() {
            return "Username = " +  Username + " Password = " + Password;
        }
    }
}

