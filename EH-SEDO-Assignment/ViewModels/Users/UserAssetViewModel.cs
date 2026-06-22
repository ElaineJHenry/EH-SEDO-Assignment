using EH_SEDO_Assignment.Models;

namespace EH_SEDO_Assignment.ViewModels.Users
{
    public class UserAssetViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<UserAssignmentsListInfo> AssetAssignmentList { get; set; }
    }
}
