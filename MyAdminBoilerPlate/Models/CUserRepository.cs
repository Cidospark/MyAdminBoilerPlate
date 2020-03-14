using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Models
{
    public class CUserRepository : IUserRepository
    {
        private List<User> _userList;
        public CUserRepository()
        {
            _userList = new List<User>()
            {
                new User(){ 
                    UserId = 1, 
                    LastName = "Eze", 
                    FirstName = "Dave", 
                    DOB = 2020, 
                    PhoneNumber = "08034345567", 
                    Email = "daveeze@sample.com", 
                    Nationality = "Nigerian", 
                    Street = "10, Ijeoma Odika Street", 
                    City = "Lagos", 
                    Country="Nigeria" ,
                    Photo="pix1.jpg"
                },
                new User(){ 
                    UserId = 2, 
                    LastName = "Musa", 
                    FirstName = "Micheal", 
                    DOB = 1920, 
                    PhoneNumber = "08034300000", 
                    Email = "musa@sample.com", 
                    Nationality = "Nigerian", 
                    Street = "10, Ijeoma Odika Street", 
                    City = "Lagos", 
                    Country="Nigeria" ,
                    Photo="pix2.jpg"
                },
                new User(){ 
                    UserId = 3, 
                    LastName = "Gyang", 
                    FirstName = "Prince", 
                    DOB = 1980, 
                    PhoneNumber = "08034311117", 
                    Email = "gyang@sample.com", 
                    Nationality = "Ghana", 
                    Street = "10, Accra Street", 
                    City = "Accra",
                    Country="Ghana" ,
                    Photo="pix1.jpg"
                }
            };
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userList;
        }

        public User GetUser(int Id)
        {
            return _userList.FirstOrDefault(u => u.UserId == Id);
        }
    }
}
