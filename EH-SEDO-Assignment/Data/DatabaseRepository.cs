using EH_SEDO_Assignment.Models;
using EH_SEDO_Assignment.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EH_SEDO_Assignment.Data
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<DatabaseRepository> _logger;
       
        public DatabaseRepository(ILogger<DatabaseRepository> logger, IConfiguration config, IWebHostEnvironment env)
        {
            _connectionString = ConnectionService.GetDbConnectionString(config, env);
            _logger = logger;
        }

        public async Task<List<UserListInfo>> GetUserInfoList()
        {
            List<UserListInfo> list = new List<UserListInfo>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetUserList", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync()) 
                    {
                        var item = new UserListInfo()
                        {
                            UserId = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            Name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            Email = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            AssignedAssetCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                        };

                        list.Add(item);
                    }

                    connection.Close();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }

                return list;
            }
        }
    }
}
