using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Models
{
    public interface IUserRepository
    {
        ApplicationUser GetUser(string Id);
        IEnumerable<ApplicationUser> GetAllUsers();
        ApplicationUser AddUser(ApplicationUser user);
        ApplicationUser DeleteUser(int Id);
        ApplicationUser EditUser(ApplicationUser userChanges);
    }
}
