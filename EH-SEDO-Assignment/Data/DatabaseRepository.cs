using EH_SEDO_Assignment.Models;
using EH_SEDO_Assignment.Services;
using EH_SEDO_Assignment.ViewModels.Assets;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        #region Users ==========================

        public async Task<UserInfoModel> GetUserInfo(string userid)
        {
            UserInfoModel model = new UserInfoModel();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetUserInfo", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserID", userid));
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        model.FirstName = reader.IsDBNull(0) ? "" : reader.GetString(0);
                        model.LastName = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        model.Email = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return model;
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
            }

            return list;
        }

        public async Task<List<UserAssignmentsListInfo>> GetUserAssetAssignments(string userid)
        {
            List<UserAssignmentsListInfo> list = new List<UserAssignmentsListInfo>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetUserAssignedAssets", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserID", userid));
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        var item = new UserAssignmentsListInfo()
                        {
                            AssignmentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            AssetId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                            AssetName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            AssetType = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            CheckOutDate = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4)
                        };

                        list.Add(item);
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return list;
        }

        #endregion

        #region Assets =========================

        public async Task<List<SelectListItem>> GetAssetTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem("", "", true));
            list.Add(new SelectListItem("Laptop", "Laptop"));
            list.Add(new SelectListItem("Tablet", "Tablet"));
            list.Add(new SelectListItem("Mobile", "Mobile"));
            list.Add(new SelectListItem("Other", "Other"));

            return list;
        }

        public async Task<int> AddAsset(AddAssetViewModel model)
        {
            int assetId = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("AddAsset", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Name", model.Name));
                    cmd.Parameters.Add(new SqlParameter("@Type", model.Type));
                    cmd.Parameters.Add(new SqlParameter("@Description", model.Description));
                    cmd.Parameters.Add(new SqlParameter("@Value", model.Value));
                    cmd.Parameters.Add(new SqlParameter("@AcqDate", model.AcquisitionDate));
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        assetId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return assetId;
        }

        public async Task<AssetDetailViewModel> GetAssetInfo(int assetId)
        {
            AssetDetailViewModel model = new AssetDetailViewModel();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetAssetInfo", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AssetId", assetId));
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        model.AssetId = assetId;
                        model.Name = reader.IsDBNull(0) ? "" : reader.GetString(0);
                        model.Type = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        model.Description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        model.AssetValue = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3));
                        model.AcquisitionDate = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4);
                        model.Status = reader.IsDBNull(5) ? "" : reader.GetString(5);
                        model.InUse = reader.IsDBNull(6) ? false : reader.GetBoolean(6);
                        model.AssignmentId = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);

                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return model;
        }

        public async Task<List<AssetAssignmentHistoryModel>> GetAssetAssignmentHistory(int assetId)
        {
            List<AssetAssignmentHistoryModel> list = new List<AssetAssignmentHistoryModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetAssetAssignmentHistory", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AssetId", assetId));
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        var item = new AssetAssignmentHistoryModel()
                        {
                            Name = reader.IsDBNull(0) ? "" : reader.GetString(0),
                            CheckOutDate = reader.IsDBNull(1) ? DateTime.Now : reader.GetDateTime(1),
                            CheckInDate = reader.IsDBNull(2) ? null : reader.GetDateTime(2)
                        };

                        list.Add(item);

                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return list;
        }

        public async Task<bool> AllowUserCheckIn(int assetId, string userId)
        {
            bool result = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("AllowUserCheckIn", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AssetId", assetId));
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        result = reader.IsDBNull(0) ? false : reader.GetString(0) == "Y";
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return result;
        }

        public async Task<bool> UpdateAsset(AssetDetailViewModel model)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("UpdateAsset", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AssetId", model.AssetId));
                    cmd.Parameters.Add(new SqlParameter("@Name", model.Name));
                    cmd.Parameters.Add(new SqlParameter("@Type", model.Type));
                    cmd.Parameters.Add(new SqlParameter("@Description", model.Description));
                    cmd.Parameters.Add(new SqlParameter("@Value", model.AssetValue));
                    cmd.Parameters.Add(new SqlParameter("@AcqDate", model.AcquisitionDate));
                    cmd.Parameters.Add(new SqlParameter("@InUse", model.InUse ? 1 : 0));
                    cmd.ExecuteNonQuery();

                    success = true;

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return success;
        }

        public async Task<List<AssetListInfo>> GetAssetList()
        {
            List<AssetListInfo> list = new List<AssetListInfo>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetAssetList", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();
                    while (await reader.ReadAsync())
                    {
                        var item = new AssetListInfo()
                        {
                            AssetId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            AssetName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            AssetType = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            AssetStatus = reader.IsDBNull(3) ? "" : reader.GetString(3),
                            AssetAssignment = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            InUse = reader.IsDBNull(5) ? false : reader.GetBoolean(5),
                            InUseText = reader.IsDBNull(5) ? "N" : reader.GetBoolean(5) ? "Y" : "N"
                        };

                        list.Add(item);

                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return list;
        }

        public async Task<bool> CheckOutAsset(string userId, int assetId)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("CheckOutAsset", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                    cmd.Parameters.Add(new SqlParameter("@AssetId", assetId));
                    cmd.ExecuteNonQuery();

                    success = true;

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return success;
        }

        public async Task<bool> CheckInAsset(int assignmentId)
        {
            bool success = false;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("CheckInAsset", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@AssignmentId", assignmentId));
                    cmd.ExecuteNonQuery();

                    success = true;

                    connection.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, null);
                    _logger.LogError(ex.StackTrace, null);
                    connection.Close();
                }
            }

            return success;
        }

        #endregion
    }
}
