using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IUserRepository userRepository, IHostingEnvironment hostingEnvironment)
        {
            this.userRepository = userRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult ListOfUsers()
        {
            var users = userRepository.GetAllUsers();
            TempData["numOfUsers"] = userRepository.GetAllUsers().Count();
            return View(users);
        }

        [HttpGet]
                public IActionResult Details(string Id)
                {
                    // un-comment the line beloww to test the nlog functionality
                    //throw new Exception("Error in Details");
                    ApplicationUser user = userRepository.GetUser(Id);
                    
                    // to handle 404 errors for un-known id
                    if(user == null)
                    {
                        Response.StatusCode = 404;
                        return View("IdNotFound");
                    }

                    HomeDetailsViewModel hdvm = new HomeDetailsViewModel()
                    {
                        pageTitle = "Users Details",
                        user = user
                    };

                    //var model = userRepository.GetUser(1);
                    //ViewData["Title"] = "Users Details";
                    return View(hdvm);
                }

    }
}
