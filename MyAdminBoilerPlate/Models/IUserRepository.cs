using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Models
{
    public interface IUserRepository
    {
        User GetUser(int Id);
        IEnumerable<User> GetAllUsers();
        void AddUser(User user);
        int DeleteUser(int Id);
    }
}
