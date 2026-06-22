using EH_SEDO_Assignment.Data;
using EH_SEDO_Assignment.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    }
}
