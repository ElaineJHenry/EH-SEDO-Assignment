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
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IDatabaseRepository databaseRepository;

        public UsersController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IDatabaseRepository databaseRepository)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
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
        public async Task<IActionResult> UserAssets(string id = "")
        {
            UserAssetViewModel model = new UserAssetViewModel();

            //Regular user can view their own assets, but will be redirected to the access denied page if trying to view somebody else
            if(string.IsNullOrEmpty(id))
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var usermodel = await databaseRepository.GetUserInfo(id);
            model.Name = usermodel.FirstName + " " + usermodel.LastName;
            model.AssetAssignmentList = await databaseRepository.GetUserAssetAssignments(id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInAsset(int assetId)
        {
            Console.WriteLine(assetId);

            return RedirectToAction("UserAssets", "Users");
        }
    }
}
