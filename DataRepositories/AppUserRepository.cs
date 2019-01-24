using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace DataRepositories
{
    public static class AppUserRepository
    {
        public static List<DataModels.AppUser> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.AppUsers.ToList();
            }
        }

        public static DataModels.AppUser GetByIdentifier(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.AppUsers
                    .Include(s => s.Souls)
                    .FirstOrDefault(u => u.Identifier.Equals(id));
            }
        }

        public static DataModels.AppUser GetBySafeKey(Guid safeKey)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.AppUsers
                    .FirstOrDefault(u => u.SafeKey.Equals(safeKey));
            }
        }

        public static DataModels.AppUser GetByPublicId(Guid publicId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.AppUsers
                    .FirstOrDefault(u => u.PublicKey.Equals(publicId));
            }
        }

        public static bool Create(DataModels.AppUser AppUser)
        {
            using (var context = new AiosKingdomContext())
            {
                if (AppUser.Id.Equals(Guid.Empty))
                {
                    AppUser.Id = Guid.NewGuid();
                }

                context.AppUsers.Add(AppUser);
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
