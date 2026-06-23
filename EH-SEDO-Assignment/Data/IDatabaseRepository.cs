using EH_SEDO_Assignment.Models;

namespace EH_SEDO_Assignment.Data
{
    public interface IDatabaseRepository
    {
        Task<UserInfoModel> GetUserInfo(string userid);
        Task<List<UserListInfo>> GetUserInfoList();
        Task<List<UserAssignmentsListInfo>> GetUserAssetAssignments(string userid);
    }
}
