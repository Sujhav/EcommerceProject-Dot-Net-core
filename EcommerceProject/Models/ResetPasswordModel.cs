using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }

        public bool IsSuccess { get; set; }
    }
}
