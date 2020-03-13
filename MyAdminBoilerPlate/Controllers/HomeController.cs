﻿using Microsoft.AspNetCore.Mvc;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserRepository userRepository;

        public HomeController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public string Index()
        {
            return userRepository.GetUser(1).LastName + " " + userRepository.GetUser(1).FirstName;
        }

        public IActionResult Details()
        {
            HomeDetailsViewModel hdvm = new HomeDetailsViewModel()
            {
                pageTitle = "Users Details",
                user = userRepository.GetUser(1)
            };

            //var model = userRepository.GetUser(1);
            //ViewData["Title"] = "Users Details";
            return View(hdvm);
        }

        public IActionResult ListOfUsers()
        {
            var users = userRepository.GetAllUsers();
            return View(users);
        }
    }
}
