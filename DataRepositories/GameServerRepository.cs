using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class GameServerRepository
    {
        public static List<DataModels.GameServer> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Servers.ToList();
            }
        }

        public static DataModels.GameServer GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Servers.FirstOrDefault(u => u.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.GameServer server)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Servers.FirstOrDefault(u => u.Name.Equals(server.Name)) != null)
                    return false;

                if (server.Id.Equals(Guid.Empty))
                    return false;

                context.Servers.Add(server);
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

        public static bool Update(DataModels.GameServer server)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Servers.FirstOrDefault(s => s.Id.Equals(server.Id));
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
