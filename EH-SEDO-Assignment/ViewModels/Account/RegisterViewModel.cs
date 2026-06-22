using System.ComponentModel.DataAnnotations;

namespace EH_SEDO_Assignment.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        [StringLength(36)]
        public string FirstName {  get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(36, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at most {1} chartacters long and must contain 1 uppercase, 1 lowercase, 1 digit and 1 non-alphanumerical chatacter.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display (Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
