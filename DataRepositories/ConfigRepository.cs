using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class ConfigRepository
    {
        public static List<DataModels.Config> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Configs.ToList();
            }
        }

        public static DataModels.Config GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Configs.FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.Config server)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Configs.FirstOrDefault(u => u.Name.Equals(server.Name)) != null)
                    return false;

                if (server.Id.Equals(Guid.Empty))
                    return false;

                context.Configs.Add(server);
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

        public static bool Update(DataModels.Config server)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Configs.FirstOrDefault(s => s.Id.Equals(server.Id));
                if (online == null) return false;

                online.Online = server.Online;

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
