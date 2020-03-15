using Microsoft.AspNetCore.Http;
using MyAdminBoilerPlate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.ViewModels
{
    public class EditUserViewModel : CreateUserViewModel
    {
        public int UserId { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
