using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public static class GameRepository
    {
        private static DataRepositories.GameDbContext _context = GetContext();

        private static DataRepositories.GameDbContext GetContext()
        {
            var id = Guid.Parse(ConfigurationManager.AppSettings.Get("ConfigId"));

            var config = DataRepositories.GameServerRepository.GetById(id);
            if (config != null)
            {
                return new DataRepositories.GameDbContext(config.DatabaseName);
            }

            return null;
        }

        public static List<DataModels.Soul> GetSoulsByUserId(Guid id)
        {
            return _context.Souls
                .Include(s => s.Equipment)
                .Include(s => s.Inventory)
                .Where(s => s.UserId.Equals(id)).ToList();
        }

        public static bool CreateSoul(DataModels.Soul soul)
        {
            var nameUsed = _context.Souls.FirstOrDefault(s => s.Name.Equals(soul.Name));
            if (nameUsed != null)
            {
                return false;
            }

            if (soul.Id.Equals(Guid.Empty))
            {
                return false;
            }



            return true;
        }
    }
}
