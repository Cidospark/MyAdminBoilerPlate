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
                    Gender = Gender.Male,
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
                    Gender = Gender.Male,
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
                    Gender = Gender.Female,
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

        public void AddUser(User user)
        {
            user.UserId = _userList.Max(u => u.UserId) + 1;
            _userList.Add(user);
            //return user;
        }

        public int DeleteUser(int Id)
        {
            var user = _userList.Find(u => u.UserId == Id);
            if (!_userList.Remove(user))
            {
                return 0;
            }
            return 1;
        }

        public int EditUser(User user)
        {
            var editedUser = _userList.Where(u => u.UserId == user.UserId).FirstOrDefault();
            if (editedUser == null)
            {
                return 0;
            }

            editedUser.LastName = user.LastName;
            editedUser.FirstName = user.FirstName;
            editedUser.Gender = user.Gender;
            return 1;

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
