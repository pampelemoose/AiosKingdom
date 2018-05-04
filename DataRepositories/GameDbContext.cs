using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Text;

namespace DataRepositories
{
    public class GameDbContext : DbContext
    {
        // User ID=sa;Password=Test1337
        public GameDbContext(string dbName)
            : base($"Data Source=.;Initial Catalog={/*ConfigurationManager.AppSettings.Get("DbName")*/dbName};Trusted_Connection=True;")
        {
        }

        public DbSet<DataModels.Soul> Souls { get; set; }
        public DbSet<DataModels.Equipment> Equipments { get; set; }
        public DbSet<DataModels.InventorySlot> Inventories { get; set; }
    }
}
