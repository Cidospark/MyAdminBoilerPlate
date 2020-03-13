﻿using Microsoft.AspNetCore.Mvc;
using MyAdminBoilerPlate.Models;
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
            return userRepository.GetUser(1).LastName;
        }
    }
}
