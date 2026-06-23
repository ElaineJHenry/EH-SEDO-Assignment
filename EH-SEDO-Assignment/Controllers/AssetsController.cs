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
            AssetListViewModel model = new AssetListViewModel();
            model.AssetList = await databaseRepository.GetAssetList();

            if (User.IsInRole("User"))
            {
                model.AssetList = model.AssetList.Where(x => x.InUse == true).ToList();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAsset()
        {
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
            AssetDetailViewModel model = new AssetDetailViewModel();
            model = await databaseRepository.GetAssetInfo(id);
            model.AssignmentHistory = await databaseRepository.GetAssetAssignmentHistory(id);
            model.CanCheckInAsset = await databaseRepository.AllowUserCheckIn(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
            model.ShowAlert  = showAlert;
            ViewBag.AssetTypeList = await databaseRepository.GetAssetTypes();
            HttpContext.Session.SetString("current_assetid", id.ToString());

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
                bool success = await databaseRepository.UpdateAsset(model);
                if (success)
                {
                    return RedirectToAction("AssetDetails", "Assets", new { id = model.AssetId, showAlert = true, alert = "Updated" });
                }

                ModelState.AddModelError(string.Empty, "An error occured when trying to update the asset.");
            }

            model.AssignmentHistory = await databaseRepository.GetAssetAssignmentHistory(model.AssetId);
            model.CanCheckInAsset = await databaseRepository.AllowUserCheckIn(model.AssetId, User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOutAsset(int assetId)
        {

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
            string id = HttpContext.Session.GetString("current_assetid");

            bool success = await databaseRepository.CheckInAsset(assignmentId);

            if (success)
            {
                return RedirectToAction("AssetDetails", "Assets", new { id = id, showAlert = true, alert = "ChkIn" });
            }

            return RedirectToAction("AssetDetails", "Assets", new { id = id, showAlert = true, alert = "ChkInFail" });
        }

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
