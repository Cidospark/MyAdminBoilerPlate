using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string Email { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public string Nationality { get; set; }
        public int DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Photo { get; set; }
    }
}
