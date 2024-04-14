using EcommerceProject.Models;
using EcommerceProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Drawing;
using System.Linq;

namespace EcommerceProject.Repository
{
    public class AccountRepository
    {
        private readonly UserManager<ApplicationUsers> _userManager;
        private readonly SignInManager<ApplicationUsers> _SignInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        public AccountRepository(UserManager<ApplicationUsers> userManager, SignInManager<ApplicationUsers> signInManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, EmailService emailService)
        {
            _userManager = userManager;
            _SignInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;


        }
        public async Task<IdentityResult> CreateUserAsync(SignUpModel model)
        {
            var user = new ApplicationUsers()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
            };
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                await GenerateEmailConfirmationTokenAsync(user);

            }
            return result;

        }

        public async Task<SignInResult> SignInAsync(LoginModel model)
        {
            var result = await _SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            return result;
        }

        public async Task SignOutAsync()
        {
            await _SignInManager.SignOutAsync();
        }

        public async Task<IdentityResult> CreateRoleAsync(RoleNameModel model)
        {
            IdentityRole role = new IdentityRole
            {
                Name = model.RoleName,
            };
            var result = await _roleManager.CreateAsync(role);


            return result;
        }

        public async Task<List<ApplicationUsers>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            return users;
        }
        public async Task<RoleAssignModel> AssignGetUsersAsync(String id)
        {
            var allroles= await _roleManager.Roles.ToListAsync();
            var users = await _userManager.FindByIdAsync(id);
            var rolesAssigned = await GetRolesForUsersAsync(id);
            var rolesNotAssigned = allroles.Select(r=>r.Name).Except(rolesAssigned);

            var data = new RoleAssignModel
            {
                Id = users.Id,
                UserName = users.UserName,
                RoleList = rolesNotAssigned.Select(r=>new SelectListItem { Text=r,Value=r}).ToList(),
            };
            return data;
        }
        public async Task<RoleAssignModel> GetUsersAsync(String id)
        {
            var users = await _userManager.FindByIdAsync(id);
            var roles = await GetRolesForUsersAsync(id);
            var data = new RoleAssignModel
            {
                Id = users.Id,
                UserName = users.UserName,
                RoleList = roles.Select(r => new SelectListItem { Text = r, Value = r }).ToList()
            };
            return data;
        }
        public async Task<ApplicationUsers> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> AssignRole(RoleAssignModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);
            return result;
        }
        public async Task<List<string>> GetRolesForUsersAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                var roles = await _userManager.GetRolesAsync(user);
                return roles.ToList();
            }
            return null;
        }
        public async Task<IdentityResult> RemoveUserRoleAsync(RoleAssignModel model)
        {

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                return result;
            }
            return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        }

        public IEnumerable<SelectListItem> RoleList()
        {

            var RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            }).ToList();

            return RoleList;

        }

        public async Task<IdentityResult> ChangePasswordAsync(string id, ChangePasswordModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            }

            return null;

        }

        //send email confirmtion

        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUsers users)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(users);
            if (!string.IsNullOrEmpty(token))
            {
                await SendEmailConfirmation(users, token);
            }
        }
        public async Task<IdentityResult> ConfirmEmailAsync(string uid, string token)
        {
            var user = await _userManager.FindByIdAsync(uid);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                return result;
            }
            return null;

        }

        private async Task SendEmailConfirmation(ApplicationUsers users, string Token)
        {
            string appdomain = _configuration.GetSection("Application:AppDomain").Value;
            string EmailConfirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value;

            EmailUsers emailUsers = new EmailUsers
            {
                ToEmails = new List<string> { users.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string> ("{{username}}",users.FirstName),
                    new KeyValuePair<string, string> ("{{link}}",string.Format(appdomain+EmailConfirmationLink,users.Id,Token))
                }
            };
            await _emailService.SendEmailForConfirmation(emailUsers);
        }

        //PasswordReset


        public async Task GenerateResetPasswordToken(ApplicationUsers users)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(users);
            if (!string.IsNullOrEmpty(token))
            {
                await sendResetPasswordConfirmation(users, token);
            }
        }
        private async Task sendResetPasswordConfirmation(ApplicationUsers user, string token)
        {
            string appDomain = _configuration.GetSection("Application:AppDomain").Value;
            string EmailConfirmationLink = _configuration.GetSection("Application:ResetPassword").Value;

            EmailUsers emailUsers = new EmailUsers
            {

                ToEmails = new List<string>() { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{username}}",user.FirstName),
                    new KeyValuePair<string, string>("{{link}}",string.Format(appDomain+EmailConfirmationLink,user.Id,token))
                }

            };

            await _emailService.SendResetPassword(emailUsers);

        }
        public async Task<IdentityResult> ResetNewPassword(ResetPasswordModel model)
        {
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(model.UserId), model.Token, model.NewPassword);
        }

    }
    
    
}
                   
 
