using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAdminBoilerPlate.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                // assign the values from the viewmodel to a new instance 
                // of the app user
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    LastName = model.LastName,
                    FirstName = model.FirstName,
                    Email = model.Email
                };
                // use the instance to generate a hashed password for the user
                var result = await userManager.CreateAsync(user, model.Password);

                // if result is successful the sign the user in
                if (result.Succeeded)
                {
                    // on successful registration generate email confirmation token and the confirmation link
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", 
                        new { userId = user.Id, token = token }, Request.Scheme);
                    
                    // log the confirmation link to a file
                    logger.Log(LogLevel.Warning, confirmationLink);

                    if(signInManager.IsSignedIn(User) && User.IsInRole("Super Admin"))
                    {
                        return RedirectToAction("ListOfUsers", "Administration");
                    }
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("ListOfUsers", "Administration");
                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Before you can Login, please confirm ypur email, by clicking on the confirmation link we have emailed you";
                    return View("Error");
                }

                // if not successful add errors to modelstate
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                UserId = user.Id,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Email = user.Email,
                ExistingPhotoPath = user.Photo,
                Claims = userClaims.Where(x => x.Value == "true").Select(c => c.Type+" => "+c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.UserId} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.LastName = model.LastName;
                user.FirstName = model.FirstName;
                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListOfUsers", "Administration");
                }

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Code);
                }

                return View(model);
            }

        }

        //---- confirm email starts ----//
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }


            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }
        //---- confirm email ends ----//



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                // Get the user's details by email and check if the user's email is confirmed and password match
                var user = await userManager.FindByEmailAsync(model.Email);
                if(user != null && !user.EmailConfirmed && 
                                    await userManager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(String.Empty, "Email not confirmed yet");
                    return View(model);
                }
                
                // use the instance to generate a hashed password for the user
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                // if result is successful then sign-in the user
                if (result.Succeeded)
                {
                    // before redirecting check if return url is empty
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("ListOfUsers", "Administration");
                    }
                }

                // if not successful add errors to modelstate
                ModelState.AddModelError("", "Invalid Login Attempt");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
