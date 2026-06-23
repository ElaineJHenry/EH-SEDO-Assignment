using EH_SEDO_Assignment.Models;

namespace EH_SEDO_Assignment.ViewModels.Users
{
    public class UserAssetViewModel
    {
        public string Name { get; set; }
        public List<UserAssignmentsListInfo> AssetAssignmentList { get; set; }

        public bool ShowAlert { get; set; }
        public string AlertMessage { get; set; }
    }
}
