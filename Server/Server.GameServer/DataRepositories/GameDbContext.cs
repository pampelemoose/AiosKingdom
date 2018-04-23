using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Text;

namespace Server.GameServer.DataRepositories
{
    public class GameDbContext : DbContext
    {
        // User ID=sa;Password=Test1337
        public GameDbContext()
            : base($"Data Source=.;Initial Catalog={ConfigurationManager.AppSettings.Get("DbName2")};Trusted_Connection=True;")
        {
        }

        public DbSet<DataModels.Soul> Souls { get; set; }
        public DbSet<DataModels.Equipment> Equipments { get; set; }
        public DbSet<DataModels.InventorySlot> Inventories { get; set; }
    }
}
