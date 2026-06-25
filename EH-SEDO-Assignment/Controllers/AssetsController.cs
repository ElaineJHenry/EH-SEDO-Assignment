using EH_SEDO_Assignment.Data;
using EH_SEDO_Assignment.ViewModels.Assets;
using EH_SEDO_Assignment.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EH_SEDO_Assignment.Controllers
{
    public class AssetsController : Controller
    {
        private readonly IDatabaseRepository databaseRepository;

        public AssetsController(IDatabaseRepository databaseRepository)
        {
            this.databaseRepository = databaseRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> AssetList()
        {
            //Get asset list information from the database and assign it to the view model
            AssetListViewModel model = new AssetListViewModel();
            model.AssetList = await databaseRepository.GetAssetList();

            //If the user is a regular user, prevent them from being able to see items that are not in use and are not on shelf.
            if (User.IsInRole("User"))
            {
                model.AssetList = model.AssetList.Where(x => x.InUse == true).ToList();
                model.AssetList = model.AssetList.Where(x => x.AssetStatus == "On Shelf").ToList();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAsset()
        {
            //gets a list of asset types that can be used in the select input
            ViewBag.AssetTypeList = await databaseRepository.GetAssetTypes();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAsset(AddAssetViewModel model)
        {
            ViewBag.AssetTypeList = await databaseRepository.GetAssetTypes();

            if (ModelState.IsValid)
            {
                //Saves the asset to the database, then redirects the user to the asset details page if successful with parameters to show an alert.
                int assetId = await databaseRepository.AddAsset(model);
                if (assetId == 0)
                {
                    ModelState.AddModelError(string.Empty, "An error occured when trying to add the asset.");
                    return View(model);
                }

                return RedirectToAction("AssetDetails", "Assets", new { id = assetId, showAlert = true, alert = "Added" });
            }


            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> AssetDetails(int id, bool showAlert = false, string alert = "")
        {
            //Gets the asset info and assigns it to the view model
            AssetDetailViewModel model = new AssetDetailViewModel();
            model = await databaseRepository.GetAssetInfo(id);

            //Prevent regular user from accessing an asset that is not in use.
            if(User.IsInRole("User") && !model.InUse)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            //Get the asset history, determine whether the current user should be able to check in the asset, determine whether an alert should be shown, and assign these values to the viewmodel
            model.AssignmentHistory = await databaseRepository.GetAssetAssignmentHistory(id);
            model.CanCheckInAsset = await databaseRepository.AllowUserCheckIn(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            model.ShowAlert  = showAlert;
            ViewBag.AssetTypeList = await databaseRepository.GetAssetTypes();

            //Set the current asset as a variable in the Session cookie, this is used when checking in an asset
            HttpContext.Session.SetString("current_assetid", id.ToString());

            //If an alert will be shown, determine what message should be shown
            if (model.ShowAlert)
            {
                model.AlertMessage = GetAlertMessage(alert);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssetDetails(AssetDetailViewModel model)
        {
            ViewBag.AssetTypeList = await databaseRepository.GetAssetTypes();

            if (ModelState.IsValid)
            {
                //Update the asset in the database using the supplied information and refresh the page with an alert if successful
                bool success = await databaseRepository.UpdateAsset(model);
                if (success)
                {
                    return RedirectToAction("AssetDetails", "Assets", new { id = model.AssetId, showAlert = true, alert = "Updated" });
                }

                ModelState.AddModelError(string.Empty, "An error occured when trying to update the asset.");
            }

            //Get model information that was not included in the form when posted
            model.AssignmentHistory = await databaseRepository.GetAssetAssignmentHistory(model.AssetId);
            model.CanCheckInAsset = await databaseRepository.AllowUserCheckIn(model.AssetId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutAsset(int assetId)
        {
            //Check out the asset as the current user, then refresh the page with an alert if successful
            bool success = await databaseRepository.CheckOutAsset(User.FindFirstValue(ClaimTypes.NameIdentifier),assetId);
            if (success)
            {
                return RedirectToAction("AssetDetails", "Assets", new { id = assetId, showAlert = true, alert = "ChkOut" });
            }

            return RedirectToAction("AssetDetails", "Assets", new { id = assetId, showAlert = true, alert = "ChkOutFail" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInAsset(int assignmentId)
        {
            //Get the current asset id from the Session Cookie
            string id = HttpContext.Session.GetString("current_assetid");

            //Check in the asset and refresh the page with an alert if successful
            bool success = await databaseRepository.CheckInAsset(assignmentId);
            if (success)
            {
                return RedirectToAction("AssetDetails", "Assets", new { id = id, showAlert = true, alert = "ChkIn" });
            }

            return RedirectToAction("AssetDetails", "Assets", new { id = id, showAlert = true, alert = "ChkInFail" });
        }

        //Determines which alert message should be shown based on a switch
        public string GetAlertMessage(string alert)
        {
            switch (alert)
            {
                case "Added":
                    return "Asset was sucessfully added to the database.";
                case "Updated":
                    return "Asset was sucessfully updated.";
                case "ChkIn":
                    return "Asset was sucessfully checked in.";
                case "ChkInFail":
                    return "There was an issue when trying to check in the asset.";
                case "ChkOut":
                    return "Asset was sucessfully checked out.";
                case "ChkOutFail":
                    return "There was an issue when trying to check out the asset.";
                default:
                    return "Alert message displayed successfully.";
            }
        }
    }
}
