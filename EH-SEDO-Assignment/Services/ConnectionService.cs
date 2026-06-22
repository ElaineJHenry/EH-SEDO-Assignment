namespace EH_SEDO_Assignment.Services
{
    public class ConnectionService
    {
        public static string GetDbConnectionString(IConfiguration config, IWebHostEnvironment env)
        {
            string connectionString = env.IsDevelopment() ? config.GetSection("ConnectionStrings")["DevConnection"] : Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DbConnectionString");

            return connectionString;
        }
    }
}
