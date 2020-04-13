using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using MyAdminBoilerPlate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Security
{
    public class CanEditOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if(authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            // get the id of the logged in user and the user to be editted
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];
            

            // if current loggedIn user is an Admin or Super Admin
            if (context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true") &&
                (adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
