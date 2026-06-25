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
            //get the user list from the database and assign it to the view model
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

            //If the id is blank, get information for the current user
            //Regular user can view their own assets, but will be redirected to the access denied page if trying to view somebody else
            if (string.IsNullOrEmpty(id))
            {
                id = currentUser;
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            //set the current userid as a variable in the session cookie
            HttpContext.Session.SetString("current_userid", id);

            //get user info from the database
            var usermodel = await databaseRepository.GetUserInfo(id);
            model.Name = usermodel.FirstName + " " + usermodel.LastName;

            //get a list of the users assignments from the database
            model.AssetAssignmentList = await databaseRepository.GetUserAssetAssignments(id);

            //determines whether the get asset button will be shown, true if the current user is browsing their own assets
            model.CanAddAsset = id == currentUser;

            //determine whether an alert should be shown, and what the message will be
            model.ShowAlert = showAlert;
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
            //if the user is an admin, returns the userid from the session cookie, allowing them to return to the user asset page they were viewing when check in was clicked
            if (User.IsInRole("Admin"))
            {
                id = HttpContext.Session.GetString("current_userid");
            }

            //checks in the asset, then refreshes the page with an alert if successful
            bool success = await databaseRepository.CheckInAsset(assignmentId);
            if (success)
            {
                return RedirectToAction("UserAssets", "Users", new { id = id, showAlert = true, alert = "ChkIn" });
            }

            return RedirectToAction("UserAssets", "Users", new { id = id, showAlert = true, alert = "Fail" });
        }

        //Determines which alert message should be shown based on a switch
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
