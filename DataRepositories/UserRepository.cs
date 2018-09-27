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
        public static List<DataModels.Role> Roles
        {
            get
            {
                using (var context = new AiosKingdomContext())
                {
                    return context.Roles.ToList();
                }
            }
        }

        public static List<DataModels.User> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Users.ToList();
            }
        }

        public static DataModels.User GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Users
                    .FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public static DataModels.User GetByCredentials(string username, string password)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Users
                    .FirstOrDefault(u => u.Username.Equals(username) && u.Password.Equals(password));
            }
        }

        public static DataModels.User GetByUsername(string username)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Users
                    .FirstOrDefault(u => u.Username.Equals(username));
            }
        }

        public static bool Create(DataModels.User user)
        {
            using (var context = new AiosKingdomContext())
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

        public static bool Update(DataModels.User user)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Users.FirstOrDefault(u => u.Id.Equals(user.Id));

                if (online == null)
                    return false;

                online.Username = user.Username;
                online.Email = user.Email;
                online.Roles = user.Roles;

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
