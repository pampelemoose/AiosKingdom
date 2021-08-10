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
            : base()
        {
        }

        public DbSet<DataModels.Version> Versions { get; set; }

        public DbSet<DataModels.Kingdom> Kingdoms { get; set; }
        public DbSet<DataModels.Town> Towns { get; set; }
        public DbSet<DataModels.AuthToken> Tokens { get; set; }

        public DbSet<DataModels.User> Users { get; set; }
        public DbSet<DataModels.Role> Roles { get; set; }

        public DbSet<DataModels.AppUser> AppUsers { get; set; }

        public DbSet<DataModels.Items.Item> Items { get; set; }
        public DbSet<DataModels.Items.ItemStat> ItemStats { get; set; }
        public DbSet<DataModels.Items.ItemEffect> ItemEffects { get; set; }

        public DbSet<DataModels.Skills.Book> Books { get; set; }
        public DbSet<DataModels.Skills.Talent> Talents { get; set; }
        public DbSet<DataModels.Skills.Inscription> Inscriptions { get; set; }

        public DbSet<DataModels.Soul> Souls { get; set; }
        public DbSet<DataModels.Equipment> Equipments { get; set; }
        public DbSet<DataModels.InventorySlot> Inventories { get; set; }
        public DbSet<DataModels.Knowledge> Knowledges { get; set; }

        public DbSet<DataModels.MarketSlot> Market { get; set; }
        public DbSet<DataModels.MarketHistory> MarketHistory { get; set; }

        public DbSet<DataModels.Monsters.Monster> Monsters { get; set; }
        public DbSet<DataModels.Monsters.Loot> Loots { get; set; }
        public DbSet<DataModels.Monsters.Phase> Phases { get; set; }

        public DbSet<DataModels.Adventures.Adventure> Adventures { get; set; }
        public DbSet<DataModels.Adventures.Bookstore> Bookstores { get; set; }
        public DbSet<DataModels.Adventures.Tavern> Taverns { get; set; }
        public DbSet<DataModels.Adventures.Quest> Quests { get; set; }
        public DbSet<DataModels.Adventures.QuestObjective> QuestObjectives { get; set; }
        public DbSet<DataModels.Adventures.Lock> Locks { get; set; }
        public DbSet<DataModels.Adventures.ShopItem> ShopItems { get; set; }
        public DbSet<DataModels.Adventures.Enemy> Enemies { get; set; }
        public DbSet<DataModels.Adventures.Npc> Npcs { get; set; }
        public DbSet<DataModels.Adventures.NpcDialogue> NpcDialogues { get; set; }

        public DbSet<DataModels.AdventureUnlocked> AdventureUnlocked { get; set; }
        public DbSet<DataModels.TalentUnlocked> TalentUnlocked { get; set; }

        public DbSet<DataModels.Website.Section> Sections { get; set; }
        public DbSet<DataModels.Website.Banner> Contents { get; set; }

        public DbSet<DataModels.Website.Category> Categories { get; set; }
        public DbSet<DataModels.Website.Thread> Threads { get; set; }
        public DbSet<DataModels.Website.Ticket> Tickets { get; set; }
        public DbSet<DataModels.Website.Comment> Comments { get; set; }
    }
}
