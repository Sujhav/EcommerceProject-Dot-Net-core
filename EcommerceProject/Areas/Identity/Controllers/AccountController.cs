using EcommerceProject.Models;
using EcommerceProject.Repository;
using EcommerceProject.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net.WebSockets;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EcommerceProject.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly AccountRepository _accountRepository;
        private readonly CartRepository _cartRepository;
        public AccountController(AccountRepository accountRepository,
            CartRepository cartRepository) 
        {
            _accountRepository = accountRepository;
            _cartRepository = cartRepository;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (ModelState.IsValid)
            {
                var result= await _accountRepository.CreateUserAsync(model);
                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("",item.Description);
                    }
                    return View(model);
                }
                ModelState.Clear();
                
                return RedirectToAction("ConfirmEmail", new { email =model.Email });

            }
            return View();
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result= await _accountRepository.SignInAsync(model);
                if (result.Succeeded)
                {

                    return RedirectToAction("ShowAllRecords", "Home", new {area= "Customer" });
                }
                else
                {
                    ModelState.AddModelError("", "invalid credentails");
                }
            }
            return View();

        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Set("CartCount",0);
            await _accountRepository.SignOutAsync();
            return RedirectToAction("ShowAllRecords", "Home", new { area = "Customer" });

        }

        [HttpGet("CreateRole")]
        public IActionResult CreateRole()
        {

            return View();
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(RoleNameModel roleNameModel)
        {

            if (ModelState.IsValid)
            {
                var result = await _accountRepository.CreateRoleAsync(roleNameModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("ShowAllRecords", "Home", new { area = "Customer" });

                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);

                    }

                }
            }
            else
            {
                ModelState.AddModelError("", "invalid");
            }
            return View();
        }
        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _accountRepository.GetAllUsersAsync();
            return View(users);
        }
        [HttpGet]
        public async Task<IActionResult> RoleRemove(String id)
        {

            var users = await _accountRepository.GetUsersAsync(id);
           

            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> RoleRemove(RoleAssignModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.RemoveUserRoleAsync(model);

                if (result.Succeeded)
                {

                    return RedirectToAction("GetAllUsers", "Account");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }


            model.RoleList = _accountRepository.RoleList();
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> RoleAssign(String id)
        {
            var users = await _accountRepository.AssignGetUsersAsync(id);

            return View(users);

        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign( RoleAssignModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _accountRepository.AssignRole(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("GetAllUsers", "Account"); 
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            
            model.RoleList = _accountRepository.RoleList(); 
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (ModelState.IsValid && userID!=null)
            {
                var result = await _accountRepository.ChangePasswordAsync(userID, model);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess= true;
                    ModelState.Clear();
                    return View();
                }
                else
                {
                   
                    foreach (var error in result.Errors)
                    {

                        if (error.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError("", "The current password provided is incorrect.");
                        }
                        else
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                       
                    }
                }
            }
            return View(model);
            
          
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid, string token,string email)
        {
            ResendEmailConfirmationModel model = new ResendEmailConfirmationModel
            {
                Email = email,
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _accountRepository.ConfirmEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    model.EmailVerified = true;
                }
            }

            return View(model);
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ResendEmailConfirmationModel model)
        {
            var user= await _accountRepository.GetUserByEmailAsync(model.Email);
            if(user != null)
            {
                if (user.EmailConfirmed)
                {
                    model.EmailVerified= true;
                    return View(model);
                }
                await _accountRepository.GenerateEmailConfirmationTokenAsync(user);
                model.EmailSent= true;
                ModelState.Clear();
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong.");
            }
            return View(model);
        }

        [HttpGet]
        [Route("Forgot-Password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _accountRepository.GetUserByEmailAsync(forgotPassword.Email);
                if (user != null)
                {
                    await _accountRepository.GenerateResetPasswordToken(user);
                }

                forgotPassword.EmailSent = true;
                ModelState.Clear();
            }
            return View(forgotPassword);
        }

        [HttpGet]
        [Route("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            ResetPasswordModel model = new ResetPasswordModel
            {
                UserId = uid,
                Token = token
            };
            return View(model);
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                model.Token = model.Token.Replace(" ", "+");
                var result = await _accountRepository.ResetNewPassword(model);
                if (result.Succeeded)
                {
                    model.IsSuccess = true;
                    ModelState.Clear();
                    return View(model);
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

            }
            return View(model);
        }



    }
}
