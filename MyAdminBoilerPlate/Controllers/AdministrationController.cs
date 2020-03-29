using System;
using System.Collections.Generic;
using System.Linq;
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
    [Authorize(Roles="Super Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdministrationController> logger;

        // constructor inject role manager
        public AdministrationController(RoleManager<IdentityRole> roleManager, 
                                        UserManager<ApplicationUser> userManager,
                                        ILogger<AdministrationController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [HttpGet]
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
                userRolesViewModel.IsSelected = await userManager.IsInRoleAsync(user, role.Name) ? true : false;
                model.Add(userRolesViewModel);
            }
            
            return View(model);
        }
        [HttpPost]
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

            //for (int i = 0; i < model.Count; i++)
            //{
            //    var role = await roleManager.FindByIdAsync(model[i].RoleId);
            //    IdentityResult result = null;

            //    // selected but not in role then add
            //    if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
            //    {
            //        result = await userManager.AddToRoleAsync(user, role.Name);
            //    }
            //    else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
            //    {
            //        result = await userManager.RemoveFromRoleAsync(user, role.Name);
            //    }
            //    else
            //    {
            //        continue;
            //    }
            //    if (result.Succeeded)
            //    {
            //        if (i < (model.Count - 1))
            //            continue;
            //        else
            //            return RedirectToAction("Edit", "Account", new { Id = userId });
            //    }
            //}

            //return RedirectToAction("Edit", "Account", new { Id = userId });
        }

        // list identity users
        [HttpGet]
        public IActionResult ListOfUsers()
        {
            var users = userManager.Users;
            TempData["numOfUsers"] = users.Count();
            return View(users);
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
