using Microsoft.AspNetCore.Http;
using MyAdminBoilerPlate.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.ViewModels
{
    public class CreateUserViewModel
    {
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Display(Name="Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and confirmation password does not match.")]
        public string ConfirmPassword { get; set; }
        public IFormFile Pix { get; set; }
    }
}
