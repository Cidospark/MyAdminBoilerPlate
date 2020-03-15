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
        public User AddUser(User user)
        {
            // use the instance of the DbContext and access users and add the new user to it
            // save changes and return the newly added user
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User DeleteUser(User model)
        {
            // use the instance of the DbContext and access users
            // find the user with the passed-in Id
            // if returned value is not null then remove user, save changes and return user
            var user = _context.Users.Find(model.UserId);
            if(user != null)
            {
                _context.Users.Remove(user);
            }
            return user;
        }

        public User EditUser(User userChanges)
        {
            // use the instance of the DbContext and access users
            // attach changes, save state as modified, save changes
            // return user changes
            var user = _context.Users.Attach(userChanges);
            if (user != null)
            {
                user.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            return userChanges;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users;
        }

        public User GetUser(int Id)
        {
            return _context.Users.Find(Id);
        }
    }
}
