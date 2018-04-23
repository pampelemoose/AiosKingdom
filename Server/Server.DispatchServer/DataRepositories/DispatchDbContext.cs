using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace Server.DispatchServer.DataRepositories
{
    public class DispatchDbContext : DbContext
    {
        // User ID=sa;Password=Test1337
        public DispatchDbContext()
            : base("Data Source=.;Initial Catalog=DispatchDb;Trusted_Connection=True;")
        {
        }

        public DbSet<DataModels.User> Users { get; set; }
        public DbSet<DataModels.Items.Bag> Bags { get; set; }
        public DbSet<DataModels.Items.Armor> Armors { get; set; }
        public DbSet<DataModels.Items.ItemStat> ItemStats { get; set; }
        public DbSet<DataModels.Items.Consumable> Consumables { get; set; }
        public DbSet<DataModels.Items.ConsumableEffect> ConsumableEffects { get; set; }

        public DbSet<DataModels.Skills.Book> Books { get; set; }
        public DbSet<DataModels.Skills.Page> Pages { get; set; }
        public DbSet<DataModels.Skills.Inscription> Inscriptions { get; set; }
    }
}
