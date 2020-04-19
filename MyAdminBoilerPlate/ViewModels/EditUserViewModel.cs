using Microsoft.AspNetCore.Http;
using MyAdminBoilerPlate.Models;
using MyAdminBoilerPlate.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.ViewModels
{
    public class EditUserViewModel : CreateUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        public string UserId { get; set; }
        public string ExistingPhotoPath { get; set; }

        [Required(ErrorMessage = "Email required")]
        [EmailAddress]
        [ValidEmailDomain(allowedDomain: "sample.com", ErrorMessage = "Email domain must be 'sample.com'")]
        public new string Email { get; set; }

        public string Nationality { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }

        public List<string> Claims { get; set; }
        public IList<string> Roles { get; set; }

        [Display(Name="Photo")]
        public IFormFile formPhoto { get; set; }
    }
}
