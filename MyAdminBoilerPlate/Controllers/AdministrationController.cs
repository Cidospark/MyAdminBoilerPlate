﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAdminBoilerPlate.Controllers
{
    [Authorize(Policy = "AdminRolePloicy")]
    //[Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;
        private readonly IUserRepository userRepository;

        // constructor inject role manager
        public AdministrationController(IUserRepository userRepository, RoleManager<IdentityRole> roleManager, 
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger)
        {
            this.userRepository = userRepository;
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        // dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }

        // manage user claims action method
        [HttpGet]
        [Authorize(Policy = "CustomPolicy")]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            // get user by Id
            var user = await userManager.FindByIdAsync(userId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {userId} cannot be found";
                return View("NotFound");
            }

            var existingUserClaim = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId
            };
            
            foreach(Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                if(existingUserClaim.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }

                model.Claims.Add(userClaim);
            };
            ViewBag.Fullname = $"{user.LastName} {user.FirstName}";
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "CustomPolicy")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // if we have found the user
            // retrieve all the user claims and delete those claims
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);
            // if there is any problem removing the claims then
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // retrieve all selected claims
            result = await userManager.AddClaimsAsync(user, model.Claims
                                      //.Where(c => c.IsSelected)
                                      //.Select(c => new Claim(c.ClaimType, c.ClaimType)));
                                      .Select(c => new Claim(c.ClaimType, c.IsSelected? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("Edit", "Account", new { id = model.UserId });
        }

        [HttpGet]
        [Authorize(Policy = "CustomPolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach(var role in roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                ViewBag.Fullname = $"{user.LastName} {user.FirstName}";
                userRolesViewModel.IsSelected = await userManager.IsInRoleAsync(user, role.Name) ? true : false;
                model.Add(userRolesViewModel);
            }

            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "CustomPolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("","Cannot remove existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(x => x.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove existing roles");
                return View(model);
            }
            return RedirectToAction("Edit", "Account", new { Id = userId });

        }

        // list identity users
        [HttpGet]
        public IActionResult ListOfUsers()
        {
            var users = userManager.Users;
            TempData["numOfUsers"] = users.Count();
            return View(users);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!User.IsInRole("Super Admin"))
            {
                ViewBag.AccessErr = "Access denied!";
                return RedirectToAction("ListOfUsers", "Administration");
            }

            var user = await userManager.FindByIdAsync(id);
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await userManager.DeleteAsync(user);
                    if(result.Succeeded)
                    {
                        return RedirectToAction("ListOfUsers", "Administration");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListOfUsers"); // this code will look for a view named ListOfUsers and redirect to it
                    //return View(ListOfUsers); // this will return an object named 'ListOfUsers' to the view with thesame name as this action method
                    //return "ListOfUsers"; // this will return 'ListOfUsers' as a string value to an empty browser
                }catch(DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"{user.LastName} {user.FirstName} is attached to a role.";
                    ViewBag.ErrorMessage = $"Please first detach the user from the role, then try again.";
                    return View("Error");
                }
            }
        }

        // User Details
        [HttpGet]
        public IActionResult Details(string Id)
        {
            string pg = "User's Details";
            ApplicationUser user = userRepository.GetUser(Id);

            // to handle 404 errors for un-known id
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id = {Id} cannot be found";
                return View("NotFound");
            }

            if (userManager.GetUserId(User).Equals(Id))
            {
                pg = "Your Profile";
            }


            HomeDetailsViewModel hdvm = new HomeDetailsViewModel()
            {
                pageTitle = pg,
                user = user
            };

            return View(hdvm);
        }


        // goto role view
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        // add roles
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }

                // in an event of error
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // List all roles
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);

        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {model.Id} cannot be found";
                return View("Notfound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Listroles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);

        }

        [HttpPost]
        [Authorize(Policy = "ManageRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return View("Notfound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Listroles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"Error deleting role {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are " +
                        $"users in this role.<br/> Please first remove users in this role and try again.";
                    return View("Error");
                }

            }


        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRoleUsers(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found!";
                return View("NotFound");
            }

            var model = new List<RoleUsersViewModel>();

            foreach(var user in userManager.Users)
            {
                var RoleUsersViewModel = new RoleUsersViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                RoleUsersViewModel.IsSelected = await userManager.IsInRoleAsync(user, role.Name) ? true : false;
                
                model.Add(RoleUsersViewModel);
            }

            ViewBag.rolename = role.Name;
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRoleUsers(List<RoleUsersViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for(int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                // selected but not in role then add
                if(model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }else if(!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("ListRoles", new { Id = roleId });
                }
            }

            return RedirectToAction("ListRoles", new { Id = roleId });
        }

    }
}
