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
            var uniquePass = Guid.NewGuid().ToString();
            var uniqueSecurity = Guid.NewGuid().ToString();
            var uniqueCuncurrency = Guid.NewGuid().ToString();
            var email = "sample@sample.com";
            var emailNormalized = "SAMPLE@SAMPLE.COM";
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    UserName = email,
                    NormalizedUserName = emailNormalized,
                    Email = email,
                    NormalizedEmail = emailNormalized,
                    SecurityStamp = uniqueSecurity,
                    ConcurrencyStamp = uniqueCuncurrency,
                    FirstName = "John",
                    LastName = "Doe",
                    Gender = Gender.Male,
                    PasswordHash = uniquePass
                }
            );
        }
    }
}
