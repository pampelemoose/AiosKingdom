namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AItems",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false, maxLength: 200),
                        Image = c.String(),
                        Type = c.Int(nullable: false),
                        Quality = c.Int(nullable: false),
                        ItemLevel = c.Int(nullable: false),
                        UseLevelRequired = c.Int(nullable: false),
                        Part = c.Int(),
                        ArmorValue = c.Int(),
                        SlotCount = c.Int(),
                        WeaponType = c.Int(),
                        MinDamages = c.Int(),
                        MaxDamages = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AItems", t => t.ItemId, cascadeDelete: true)
                .Index(t => t.ItemId);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Guid(nullable: false),
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
                        StatCost = c.Int(nullable: false),
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
                .ForeignKey("dbo.AItems", t => t.ConsumableId, cascadeDelete: true)
                .Index(t => t.ConsumableId);
            
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Bag_Id = c.Guid(),
                        Belt_Id = c.Guid(),
                        Feet_Id = c.Guid(),
                        Hand_Id = c.Guid(),
                        Head_Id = c.Guid(),
                        Leg_Id = c.Guid(),
                        Pants_Id = c.Guid(),
                        Shoulder_Id = c.Guid(),
                        Torso_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AItems", t => t.Bag_Id)
                .ForeignKey("dbo.AItems", t => t.Belt_Id)
                .ForeignKey("dbo.AItems", t => t.Feet_Id)
                .ForeignKey("dbo.AItems", t => t.Hand_Id)
                .ForeignKey("dbo.AItems", t => t.Head_Id)
                .ForeignKey("dbo.AItems", t => t.Leg_Id)
                .ForeignKey("dbo.AItems", t => t.Pants_Id)
                .ForeignKey("dbo.AItems", t => t.Shoulder_Id)
                .ForeignKey("dbo.AItems", t => t.Torso_Id)
                .Index(t => t.Bag_Id)
                .Index(t => t.Belt_Id)
                .Index(t => t.Feet_Id)
                .Index(t => t.Hand_Id)
                .Index(t => t.Head_Id)
                .Index(t => t.Leg_Id)
                .Index(t => t.Pants_Id)
                .Index(t => t.Shoulder_Id)
                .Index(t => t.Torso_Id);
            
            CreateTable(
                "dbo.InventorySlots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SoulId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        ItemId = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AItems", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Souls", t => t.SoulId, cascadeDelete: true)
                .Index(t => t.SoulId)
                .Index(t => t.ItemId);
            
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
                .ForeignKey("dbo.GameServers", t => t.ServerId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ServerId)
                .Index(t => t.UserId)
                .Index(t => t.EquipmentId);
            
            CreateTable(
                "dbo.GameServers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Host = c.String(nullable: false),
                        Port = c.Int(nullable: false),
                        Online = c.Boolean(nullable: false),
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
                .PrimaryKey(t => t.Id);
            
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
                .ForeignKey("dbo.AItems", t => t.ItemId, cascadeDelete: true)
                .ForeignKey("dbo.Souls", t => t.SellerId)
                .Index(t => t.ItemId)
                .Index(t => t.SellerId);
            
            CreateTable(
                "dbo.GameServerTokens",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Token = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("dbo.MarketSlots", "ItemId", "dbo.AItems");
            DropForeignKey("dbo.Souls", "UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.Souls", "ServerId", "dbo.GameServers");
            DropForeignKey("dbo.InventorySlots", "SoulId", "dbo.Souls");
            DropForeignKey("dbo.Souls", "EquipmentId", "dbo.Equipments");
            DropForeignKey("dbo.InventorySlots", "ItemId", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Torso_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Shoulder_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Pants_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Leg_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Head_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Hand_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Feet_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Belt_Id", "dbo.AItems");
            DropForeignKey("dbo.Equipments", "Bag_Id", "dbo.AItems");
            DropForeignKey("dbo.ConsumableEffects", "ConsumableId", "dbo.AItems");
            DropForeignKey("dbo.Inscriptions", "PageId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "BookId", "dbo.Books");
            DropForeignKey("dbo.ItemStats", "ItemId", "dbo.AItems");
            DropIndex("dbo.RoleUsers", new[] { "User_Id" });
            DropIndex("dbo.RoleUsers", new[] { "Role_Id" });
            DropIndex("dbo.MarketSlots", new[] { "SellerId" });
            DropIndex("dbo.MarketSlots", new[] { "ItemId" });
            DropIndex("dbo.Souls", new[] { "EquipmentId" });
            DropIndex("dbo.Souls", new[] { "UserId" });
            DropIndex("dbo.Souls", new[] { "ServerId" });
            DropIndex("dbo.InventorySlots", new[] { "ItemId" });
            DropIndex("dbo.InventorySlots", new[] { "SoulId" });
            DropIndex("dbo.Equipments", new[] { "Torso_Id" });
            DropIndex("dbo.Equipments", new[] { "Shoulder_Id" });
            DropIndex("dbo.Equipments", new[] { "Pants_Id" });
            DropIndex("dbo.Equipments", new[] { "Leg_Id" });
            DropIndex("dbo.Equipments", new[] { "Head_Id" });
            DropIndex("dbo.Equipments", new[] { "Hand_Id" });
            DropIndex("dbo.Equipments", new[] { "Feet_Id" });
            DropIndex("dbo.Equipments", new[] { "Belt_Id" });
            DropIndex("dbo.Equipments", new[] { "Bag_Id" });
            DropIndex("dbo.ConsumableEffects", new[] { "ConsumableId" });
            DropIndex("dbo.Inscriptions", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "BookId" });
            DropIndex("dbo.ItemStats", new[] { "ItemId" });
            DropTable("dbo.RoleUsers");
            DropTable("dbo.GameServerTokens");
            DropTable("dbo.MarketSlots");
            DropTable("dbo.Roles");
            DropTable("dbo.Users");
            DropTable("dbo.GameServers");
            DropTable("dbo.Souls");
            DropTable("dbo.InventorySlots");
            DropTable("dbo.Equipments");
            DropTable("dbo.ConsumableEffects");
            DropTable("dbo.Inscriptions");
            DropTable("dbo.Pages");
            DropTable("dbo.Books");
            DropTable("dbo.ItemStats");
            DropTable("dbo.AItems");
        }
    }
}
