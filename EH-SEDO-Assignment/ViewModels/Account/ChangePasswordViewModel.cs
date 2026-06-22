using System.ComponentModel.DataAnnotations;

namespace EH_SEDO_Assignment.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType (DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        [StringLength(36, MinimumLength = 8, ErrorMessage = "The {0} must be at least {2} and at most {1} chartacters long")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Password does not match")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmn Password")]
        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
