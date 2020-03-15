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
        User AddUser(User user);
        User DeleteUser(int Id);
        User EditUser(User userChanges);
    }
}
