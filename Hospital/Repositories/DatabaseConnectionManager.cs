namespace Hospital.Repositories 
{
    public class DatabaseConnectionManager
    {
        private static DatabaseConnectionManager? _instance;
        private static readonly object _lock = new object();
        public string ConnectionString { get; private set; }
        private DatabaseConnectionManager() {
            ConnectionString = "Data Source=hospital.db";
        }
        public static DatabaseConnectionManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DatabaseConnectionManager();
                    }
                    return _instance;
                }
            }
        }
    }
}