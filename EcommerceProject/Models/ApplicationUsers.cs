using Microsoft.AspNetCore.Identity;

namespace EcommerceProject.Models
{
    public class ApplicationUsers:IdentityUser
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
      
    }
}
