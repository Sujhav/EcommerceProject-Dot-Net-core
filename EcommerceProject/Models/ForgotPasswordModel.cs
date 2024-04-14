using System.ComponentModel.DataAnnotations;

namespace EcommerceProject.Models
{
    public class ForgotPasswordModel
    {

        [Required, Display(Name = "Enter your valid email address")]
        [EmailAddress]
        public string Email { get; set; }

        public bool EmailSent { get; set; }
    }
}
