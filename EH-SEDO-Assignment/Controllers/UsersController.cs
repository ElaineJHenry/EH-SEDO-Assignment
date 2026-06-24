using EH_SEDO_Assignment.Data;
using EH_SEDO_Assignment.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EH_SEDO_Assignment.Controllers
{
    public class UsersController : Controller
    {
        private readonly IDatabaseRepository databaseRepository;

        public UsersController(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserList()
        {
            UserListViewModel model = new UserListViewModel();
            model.UserList = await databaseRepository.GetUserInfoList();

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UserAssets(string id = "", bool showAlert = false, string alert = "")
        {
            UserAssetViewModel model = new UserAssetViewModel();
            var currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Regular user can view their own assets, but will be redirected to the access denied page if trying to view somebody else
            if (string.IsNullOrEmpty(id))
            {
                id = currentUser;
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            HttpContext.Session.SetString("current_userid", id);
            var usermodel = await databaseRepository.GetUserInfo(id);
            model.Name = usermodel.FirstName + " " + usermodel.LastName;
            model.AssetAssignmentList = await databaseRepository.GetUserAssetAssignments(id);
            model.ShowAlert = showAlert;
            model.CanAddAsset = id == currentUser;

            if (model.ShowAlert)
            {
                model.AlertMessage = GetAlertMessage(alert);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInAsset(int assignmentId)
        {
            string id = "";
            if (User.IsInRole("Admin"))
            {
                id = HttpContext.Session.GetString("current_userid");
            }

            bool success = await databaseRepository.CheckInAsset(assignmentId);

            if (success)
            {
                return RedirectToAction("UserAssets", "Users", new { id = id, showAlert = true, alert = "ChkIn" });
            }

            return RedirectToAction("UserAssets", "Users", new { id = id, showAlert = true, alert = "Fail" });
        }

        public string GetAlertMessage(string alert)
        {
            switch (alert)
            {
                case "ChkIn":
                    return "Asset was sucessfully checked in.";
                case "Fail":
                    return "There was an issue when trying to check in the asset.";
                default:
                    return "Alert message displayed successfully.";
            }
        }
    }
}
