using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class ChangePasswordModel
    {

        [Required(ErrorMessage = "Enter your current password"), Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Enter your New password"), Display(Name = "New Password")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Enter your New password Again"), Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Password doesnot match")]
        public string ConfirmPassword { get; set; }
    }
}
