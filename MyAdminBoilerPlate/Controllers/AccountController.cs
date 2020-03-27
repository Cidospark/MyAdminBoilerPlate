using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("ListOfUsers", "Home");
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
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("ListOfUsers", "Home");
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
                // use the instance to generate a hashed password for the user
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                // if result is successful the sign the user in
                if (result.Succeeded)
                {
                    // before redirecting check if return url is empty
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("ListOfUsers", "Home");
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
