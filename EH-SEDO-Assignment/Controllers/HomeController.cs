using EH_SEDO_Assignment.Data;
using EH_SEDO_Assignment.Models;
using EH_SEDO_Assignment.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EH_SEDO_Assignment.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IDatabaseRepository databaseRepository;

        public HomeController(SignInManager<ApplicationUser> signInManager, IDatabaseRepository databaseRepository)
        {
            this.signInManager = signInManager;
            this.databaseRepository = databaseRepository;
        }

        public async Task <IActionResult> Index(bool showAlert = false, string alert = "")
        {
            string name = "", alertMessage = "";

            if (signInManager.IsSignedIn(User))
            {
                var usermodel = await databaseRepository.GetUserInfo(User.FindFirstValue(ClaimTypes.NameIdentifier));
                name = usermodel.FirstName;
            }
            if (showAlert)
            {
                alertMessage = GetAlertMessage(alert);
            }

            HomeViewModel model = new HomeViewModel()
            {
                Name = name,
                ShowAlert = showAlert,
                AlertMessage = alertMessage
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string GetAlertMessage(string alert)
        {
            switch (alert)
            {
                case "CP":
                    return "Password has been changed successfully.";
                default:
                    return "Alert message displayed successfully.";
            }
        }
    }
}
