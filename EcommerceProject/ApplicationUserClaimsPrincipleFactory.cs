using EcommerceProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EcommerceProject
{
    public class ApplicationUserClaimsPrincipleFactory:UserClaimsPrincipalFactory<ApplicationUsers,IdentityRole>
    {
        public ApplicationUserClaimsPrincipleFactory(UserManager<ApplicationUsers> userManager,RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options):base(userManager,roleManager, options)
        {

        }
         protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUsers user)
        {
            var identity= await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("FirstName", user.FirstName ??""));
            identity.AddClaim(new Claim("LastName", user.LastName ??""));
            identity.AddClaim(new Claim("UserId" ,user.Id));
            return identity;

        }
            
        
    }
}
