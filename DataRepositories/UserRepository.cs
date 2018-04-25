using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace DataRepositories
{
    public static class UserRepository
    {
        private static DispatchDbContext _context = new DispatchDbContext();

        public static List<DataModels.User> GetAll()
        {
            return _context.Users.ToList();
        }

        public static DataModels.User GetByCredentials(string username, string password)
        {
            return _context.Users.Include(r => r.Roles).FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
        }

        public static bool Create(DataModels.User user)
        {
            using (var context = new DispatchDbContext())
            {
                if (context.Users.FirstOrDefault(u => u.Username.Equals(user.Username)) != null)
                    return false;

                if (user.Id.Equals(Guid.Empty))
                {
                    user.Id = Guid.NewGuid();
                }

                context.Users.Add(user);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var error in e.EntityValidationErrors)
                    {
                        foreach (var mess in error.ValidationErrors)
                        {
                            Console.WriteLine(mess.ErrorMessage);
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
