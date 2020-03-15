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
        User DeleteUser(User user);
        User EditUser(User userChanges);
    }
}
