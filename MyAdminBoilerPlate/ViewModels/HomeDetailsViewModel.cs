﻿using MyAdminBoilerPlate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.ViewModels
{
    public class HomeDetailsViewModel
    {
        public ApplicationUser user { get; set; }
        public string pageTitle { get; set; }
    }
}
