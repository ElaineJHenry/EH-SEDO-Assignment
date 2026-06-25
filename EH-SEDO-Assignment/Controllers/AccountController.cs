using EH_SEDO_Assignment.Data;
using EH_SEDO_Assignment.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EH_SEDO_Assignment.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login(string accountCreated = "")
        {
            
            LoginViewModel model = new LoginViewModel
            {
                //This causes an alert to be shown after being redirected to the log in page after registering
                UserCreated = accountCreated == "Y" ? true : false
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Attempts to log in with supplied credentials, if successful, the user is signed in and redirected to the home page
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid log in attempt.");
                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    NormalizedUserName = model.Email.ToUpperInvariant(),
                    Email = model.Email,
                    NormalizedEmail = model.Email.ToUpperInvariant()
                };

                //Create the user and assign the account the 'User' role, then redirect to the log in page
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");

                    return RedirectToAction("Login", "Account", new { accountCreated = "Y" });
                }

                //If user creation fails, errors are added to the model state to be displayed to the user
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> ChangePassword()
        {
            //Gets the User ID of the User attempting to change password
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ChangePasswordViewModel model = new ChangePasswordViewModel()
            {
                UserId = userid
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Gets the user with the supplied userId
                var user = await userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "No user with that user id was found");
                    return View(model);
                }

                //Updates the password for the user if the correct password has been supplied, then redirects to the home page with parameters to display an alert
                var result = await userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home", new {showAlert = true, alert = "CP"});
                }
                else
                {
                    //If password update fails, errors are added to the model state to be displayed to the user
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View(model);
                }

            }

            ModelState.AddModelError(string.Empty, "An error occured when trying to change the password");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            //Signs the user out from the application, then redirects them to the home page.
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}
