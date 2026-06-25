namespace EH_SEDO_Assignment.Services
{
    public class ConnectionService
    {
        public static string GetDbConnectionString(IConfiguration config, IWebHostEnvironment env)
        {
            //gets the connection string from user secrets if the environment is development, else gets the connection string from azure environment variables
            string connectionString = env.IsDevelopment() ? config.GetSection("ConnectionStrings")["DevConnection"] : Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DbConnectionString");

            return connectionString;
        }
    }
}
