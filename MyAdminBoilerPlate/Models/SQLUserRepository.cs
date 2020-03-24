using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAdminBoilerPlate.Models
{
    public class SQLUserRepository : IUserRepository
    {
        // create a constructor, inject an instance of DbContext into it
        // create a privat global instance of the DbContext 
        private readonly AppDbContext _context;
        public SQLUserRepository(AppDbContext context)
        {
            _context = context;
        }
        public ApplicationUser AddUser(ApplicationUser user)
        {
            // use the instance of the DbContext and access users and add the new user to it
            // save changes and return the newly added user
            _context.applicationUsers.Add(user);
            _context.SaveChanges();
            return user;
        }

        public ApplicationUser DeleteUser(int Id)
        {
            // use the instance of the DbContext and access users
            // find the user with the passed-in Id
            // if returned value is not null then remove user, save changes and return user
            var user = _context.applicationUsers.Find(Id);
            if(user != null)
            {
                _context.applicationUsers.Remove(user);
            }
            return user;
        }

        public ApplicationUser EditUser(ApplicationUser userChanges)
        {
            // use the instance of the DbContext and access users
            // attach changes, save state as modified, save changes
            // return user changes
            var user = _context.applicationUsers.Attach(userChanges);
            if (user != null)
            {
                user.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            return userChanges;
        }

        public IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _context.applicationUsers;
        }

        public ApplicationUser GetUser(string Id)
        {
            return _context.applicationUsers.Find(Id);
        }
    }
}
