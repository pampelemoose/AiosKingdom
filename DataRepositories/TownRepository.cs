using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class TownRepository
    {
        public static List<DataModels.Town> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Towns.ToList();
            }
        }

        public static DataModels.Town GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Towns.FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.Town server)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Towns.FirstOrDefault(u => u.Name.Equals(server.Name)) != null)
                    return false;

                if (server.Id.Equals(Guid.Empty))
                    return false;

                context.Towns.Add(server);
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

        public static bool Update(DataModels.Town server)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Towns.FirstOrDefault(s => s.Id.Equals(server.Id));
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
