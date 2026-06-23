using EH_SEDO_Assignment.Models;
using EH_SEDO_Assignment.ViewModels.Assets;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EH_SEDO_Assignment.Data
{
    public interface IDatabaseRepository
    {
        Task<UserInfoModel> GetUserInfo(string userid);
        Task<List<UserListInfo>> GetUserInfoList();
        Task<List<UserAssignmentsListInfo>> GetUserAssetAssignments(string userid);

        Task<List<SelectListItem>> GetAssetTypes();
        Task<int> AddAsset(AddAssetViewModel model);
        Task<AssetDetailViewModel> GetAssetInfo(int assetId);
        Task<List<AssetAssignmentHistoryModel>> GetAssetAssignmentHistory(int assetId);
        Task<bool> AllowUserCheckIn(int assetId, string userId);
        Task<bool> UpdateAsset(AssetDetailViewModel model);
        Task<List<AssetListInfo>> GetAssetList();
        Task<bool> CheckOutAsset(string userId, int assetId);
        Task<bool> CheckInAsset(int assignmentId);
    }
}
