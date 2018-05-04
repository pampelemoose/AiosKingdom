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
        private static AiosKingdomContext _context = new AiosKingdomContext();

        public static List<DataModels.GameServer> GetAll()
        {
            return _context.Servers.ToList();
        }

        public static DataModels.GameServer GetById(Guid id)
        {
            return _context.Servers.FirstOrDefault(u => u.Id.Equals(id));
        }

        public static bool Create(DataModels.GameServer server)
        {
            if (_context.Servers.FirstOrDefault(u => u.Name.Equals(server.Name)) != null)
                return false;

            if (server.Id.Equals(Guid.Empty))
                return false;

            _context.Servers.Add(server);
            try
            {
                _context.SaveChanges();
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

        public static bool Update()
        {
            try
            {
                _context.SaveChanges();
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
