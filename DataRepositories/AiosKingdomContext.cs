using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;

namespace DataRepositories
{
    public class AiosKingdomContext : DbContext
    {
        // User ID=sa;Password=Test1337
        public AiosKingdomContext()
            : base("Data Source=.;Initial Catalog=AiosKingdom;Trusted_Connection=True;")
        {
        }

        public DbSet<DataModels.Version> Versions { get; set; }

        public DbSet<DataModels.Kingdom> Kingdoms { get; set; }
        public DbSet<DataModels.Config> Configs { get; set; }
        public DbSet<DataModels.AuthToken> Tokens { get; set; }

        public DbSet<DataModels.User> Users { get; set; }
        public DbSet<DataModels.Role> Roles { get; set; }

        public DbSet<DataModels.AppUser> AppUsers { get; set; }

        public DbSet<DataModels.Items.Bag> Bags { get; set; }
        public DbSet<DataModels.Items.Armor> Armors { get; set; }
        public DbSet<DataModels.Items.Weapon> Weapons { get; set; }
        public DbSet<DataModels.Items.ItemStat> ItemStats { get; set; }
        public DbSet<DataModels.Items.Consumable> Consumables { get; set; }
        public DbSet<DataModels.Items.ConsumableEffect> ConsumableEffects { get; set; }

        public DbSet<DataModels.Skills.Book> Books { get; set; }
        public DbSet<DataModels.Skills.Page> Pages { get; set; }
        public DbSet<DataModels.Skills.Inscription> Inscriptions { get; set; }

        public DbSet<DataModels.Soul> Souls { get; set; }
        public DbSet<DataModels.Equipment> Equipments { get; set; }
        public DbSet<DataModels.InventorySlot> Inventories { get; set; }
        public DbSet<DataModels.Knowledge> Knowledges { get; set; }
        public DbSet<DataModels.DungeonProgress> DungeonProgresses { get; set; }

        public DbSet<DataModels.MarketSlot> Market { get; set; }

        public DbSet<DataModels.Monsters.Monster> Monsters { get; set; }
        public DbSet<DataModels.Monsters.Loot> Loots { get; set; }
        public DbSet<DataModels.Monsters.Phase> Phases { get; set; }
        public DbSet<DataModels.Dungeons.Dungeon> Dungeons { get; set; }

        public DbSet<DataModels.Website.Section> Sections { get; set; }

    }
}
