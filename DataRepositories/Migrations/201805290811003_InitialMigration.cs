namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Armors",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Part = c.Int(nullable: false),
                        ArmorValue = c.Int(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        Image = c.String(),
                        Type = c.Int(nullable: false),
                        Quality = c.Int(nullable: false),
                        ItemLevel = c.Int(nullable: false),
                        UseLevelRequired = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ItemStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ItemId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        StatValue = c.Int(nullable: false),
                        Armor_Id = c.Guid(),
                        Bag_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Armors", t => t.Armor_Id)
                .ForeignKey("dbo.Bags", t => t.Bag_Id)
                .Index(t => t.Armor_Id)
                .Index(t => t.Bag_Id);
            
            CreateTable(
                "dbo.Bags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SlotCount = c.Int(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        Image = c.String(),
                        Type = c.Int(nullable: false),
                        Quality = c.Int(nullable: false),
                        ItemLevel = c.Int(nullable: false),
                        UseLevelRequired = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Quality = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                        Image = c.String(),
                        EmberCost = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                        ManaCost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId);
            
            CreateTable(
                "dbo.Inscriptions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PageId = c.Guid(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50),
                        Type = c.Int(nullable: false),
                        BaseValue = c.Int(nullable: false),
                        StatType = c.Int(nullable: false),
                        Ratio = c.Single(nullable: false),
                        Duration = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pages", t => t.PageId, cascadeDelete: true)
                .Index(t => t.PageId);
            
            CreateTable(
                "dbo.Configs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        Host = c.String(nullable: false),
                        Port = c.Int(nullable: false),
                        Online = c.Boolean(nullable: false),
                        KingdomId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Difficulty = c.Int(nullable: false),
                        SlotLimit = c.Int(nullable: false),
                        SlotAvailable = c.Int(nullable: false),
                        BaseExperience = c.Int(nullable: false),
                        ExperiencePerLevelRatio = c.Single(nullable: false),
                        SpiritsPerLevelUp = c.Int(nullable: false),
                        EmbersPerLevelUp = c.Int(nullable: false),
                        BaseHealth = c.Int(nullable: false),
                        HealthPerLevelRatio = c.Single(nullable: false),
                        HealthPerStaminaRatio = c.Single(nullable: false),
                        BaseMana = c.Int(nullable: false),
                        ManaPerLevelRatio = c.Single(nullable: false),
                        ManaPerEnergyRatio = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kingdoms", t => t.KingdomId, cascadeDelete: true)
                .Index(t => t.KingdomId);
            
            CreateTable(
                "dbo.ConsumableEffects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ConsumableId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        Type = c.Int(nullable: false),
                        AffectValue = c.Single(nullable: false),
                        AffectTime = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Consumables", t => t.ConsumableId, cascadeDelete: true)
                .Index(t => t.ConsumableId);
            
            CreateTable(
                "dbo.Consumables",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        Image = c.String(),
                        Type = c.Int(nullable: false),
                        Quality = c.Int(nullable: false),
                        ItemLevel = c.Int(nullable: false),
                        UseLevelRequired = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dungeons",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        DungeonId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        RequiredLevel = c.Int(nullable: false),
                        MaxLevelAuthorized = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DungeonId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        RoomNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dungeons", t => t.DungeonId, cascadeDelete: true)
                .Index(t => t.DungeonId);
            
            CreateTable(
                "dbo.Monsters",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        MonsterId = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        Story = c.String(nullable: false),
                        Image = c.String(),
                        HealthPerLevel = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Loots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MonsterId = c.Guid(nullable: false),
                        DropRate = c.Double(nullable: false),
                        Type = c.Int(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Monsters", t => t.MonsterId, cascadeDelete: true)
                .Index(t => t.MonsterId);
            
            CreateTable(
                "dbo.Phases",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        MonsterId = c.Guid(nullable: false),
                        StaminaPerLevel = c.Double(nullable: false),
                        EnergyPerLevel = c.Double(nullable: false),
                        StrengthPerLevel = c.Double(nullable: false),
                        AgilityPerLevel = c.Double(nullable: false),
                        IntelligencePerLevel = c.Double(nullable: false),
                        WisdomPerLevel = c.Double(nullable: false),
                        SkillId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Monsters", t => t.MonsterId, cascadeDelete: true)
                .Index(t => t.MonsterId);
            
            CreateTable(
                "dbo.ShopItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        RoomId = c.Guid(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        ShardPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId);
            
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Bag = c.Guid(nullable: false),
                        Head = c.Guid(nullable: false),
                        Shoulder = c.Guid(nullable: false),
                        Torso = c.Guid(nullable: false),
                        Belt = c.Guid(nullable: false),
                        Pants = c.Guid(nullable: false),
                        Leg = c.Guid(nullable: false),
                        Feet = c.Guid(nullable: false),
                        Hand = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InventorySlots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SoulId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                        LootedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Souls", t => t.SoulId, cascadeDelete: true)
                .Index(t => t.SoulId);
            
            CreateTable(
                "dbo.Kingdoms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        LevelGap = c.Int(nullable: false),
                        MaxLevelCountForGap = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Knowledges",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SoulId = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Rank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Souls", t => t.SoulId, cascadeDelete: true)
                .Index(t => t.SoulId);
            
            CreateTable(
                "dbo.MarketSlots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ServerId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        SellerId = c.Guid(),
                        Quantity = c.Int(nullable: false),
                        ShardPrice = c.Int(nullable: false),
                        BitPrice = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Souls", t => t.SellerId)
                .Index(t => t.SellerId);
            
            CreateTable(
                "dbo.Souls",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ServerId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 25),
                        TimePlayed = c.Single(nullable: false),
                        Level = c.Int(nullable: false),
                        CurrentExperience = c.Int(nullable: false),
                        Stamina = c.Int(nullable: false),
                        Energy = c.Int(nullable: false),
                        Strength = c.Int(nullable: false),
                        Agility = c.Int(nullable: false),
                        Intelligence = c.Int(nullable: false),
                        Wisdom = c.Int(nullable: false),
                        Spirits = c.Int(nullable: false),
                        Embers = c.Int(nullable: false),
                        Shards = c.Int(nullable: false),
                        Bits = c.Int(nullable: false),
                        EquipmentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Equipments", t => t.EquipmentId, cascadeDelete: true)
                .ForeignKey("dbo.Configs", t => t.ServerId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ServerId)
                .Index(t => t.UserId)
                .Index(t => t.EquipmentId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        ActivationCode = c.Guid(nullable: false),
                        IsActivated = c.Boolean(nullable: false),
                        SoulSlots = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AuthTokens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Token = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Low = c.Int(nullable: false),
                        Mid = c.Int(nullable: false),
                        High = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MonsterRooms",
                c => new
                    {
                        Monster_Id = c.Guid(nullable: false),
                        Room_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Monster_Id, t.Room_Id })
                .ForeignKey("dbo.Monsters", t => t.Monster_Id, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.Room_Id, cascadeDelete: true)
                .Index(t => t.Monster_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.RoleUsers",
                c => new
                    {
                        Role_Id = c.Int(nullable: false),
                        User_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_Id, t.User_Id })
                .ForeignKey("dbo.Roles", t => t.Role_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Role_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MarketSlots", "SellerId", "dbo.Souls");
            DropForeignKey("dbo.Souls", "UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.Souls", "ServerId", "dbo.Configs");
            DropForeignKey("dbo.Knowledges", "SoulId", "dbo.Souls");
            DropForeignKey("dbo.InventorySlots", "SoulId", "dbo.Souls");
            DropForeignKey("dbo.Souls", "EquipmentId", "dbo.Equipments");
            DropForeignKey("dbo.Configs", "KingdomId", "dbo.Kingdoms");
            DropForeignKey("dbo.Rooms", "DungeonId", "dbo.Dungeons");
            DropForeignKey("dbo.ShopItems", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.MonsterRooms", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.MonsterRooms", "Monster_Id", "dbo.Monsters");
            DropForeignKey("dbo.Phases", "MonsterId", "dbo.Monsters");
            DropForeignKey("dbo.Loots", "MonsterId", "dbo.Monsters");
            DropForeignKey("dbo.ConsumableEffects", "ConsumableId", "dbo.Consumables");
            DropForeignKey("dbo.Pages", "BookId", "dbo.Books");
            DropForeignKey("dbo.Inscriptions", "PageId", "dbo.Pages");
            DropForeignKey("dbo.ItemStats", "Bag_Id", "dbo.Bags");
            DropForeignKey("dbo.ItemStats", "Armor_Id", "dbo.Armors");
            DropIndex("dbo.RoleUsers", new[] { "User_Id" });
            DropIndex("dbo.RoleUsers", new[] { "Role_Id" });
            DropIndex("dbo.MonsterRooms", new[] { "Room_Id" });
            DropIndex("dbo.MonsterRooms", new[] { "Monster_Id" });
            DropIndex("dbo.Souls", new[] { "EquipmentId" });
            DropIndex("dbo.Souls", new[] { "UserId" });
            DropIndex("dbo.Souls", new[] { "ServerId" });
            DropIndex("dbo.MarketSlots", new[] { "SellerId" });
            DropIndex("dbo.Knowledges", new[] { "SoulId" });
            DropIndex("dbo.InventorySlots", new[] { "SoulId" });
            DropIndex("dbo.ShopItems", new[] { "RoomId" });
            DropIndex("dbo.Phases", new[] { "MonsterId" });
            DropIndex("dbo.Loots", new[] { "MonsterId" });
            DropIndex("dbo.Rooms", new[] { "DungeonId" });
            DropIndex("dbo.ConsumableEffects", new[] { "ConsumableId" });
            DropIndex("dbo.Configs", new[] { "KingdomId" });
            DropIndex("dbo.Inscriptions", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "BookId" });
            DropIndex("dbo.ItemStats", new[] { "Bag_Id" });
            DropIndex("dbo.ItemStats", new[] { "Armor_Id" });
            DropTable("dbo.RoleUsers");
            DropTable("dbo.MonsterRooms");
            DropTable("dbo.Versions");
            DropTable("dbo.AuthTokens");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.Souls");
            DropTable("dbo.MarketSlots");
            DropTable("dbo.Knowledges");
            DropTable("dbo.Kingdoms");
            DropTable("dbo.InventorySlots");
            DropTable("dbo.Equipments");
            DropTable("dbo.ShopItems");
            DropTable("dbo.Phases");
            DropTable("dbo.Loots");
            DropTable("dbo.Monsters");
            DropTable("dbo.Rooms");
            DropTable("dbo.Dungeons");
            DropTable("dbo.Consumables");
            DropTable("dbo.ConsumableEffects");
            DropTable("dbo.Configs");
            DropTable("dbo.Inscriptions");
            DropTable("dbo.Pages");
            DropTable("dbo.Books");
            DropTable("dbo.Bags");
            DropTable("dbo.ItemStats");
            DropTable("dbo.Armors");
        }
    }
}
