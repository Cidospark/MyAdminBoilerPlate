using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Lastname required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Firstname require")]
        public string FirstName { get; set; }
        //[Required]
        //public Gender Gender { get; set; }
        [Required(ErrorMessage ="Email required")]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [ValidEmailDomain(allowedDomain: "sample.com", ErrorMessage ="Email domain must be 'sample.com'")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required field")]
        [Display(Name="Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and confirmation password does not match.")]
        public string ConfirmPassword { get; set; }
        ///public IFormFile Pix { get; set; }
    }
}
