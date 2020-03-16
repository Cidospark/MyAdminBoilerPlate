using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.Male
                }
            );
        }
    }
}
