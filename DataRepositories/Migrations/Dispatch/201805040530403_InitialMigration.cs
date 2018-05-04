namespace DataRepositories.Migrations.Dispatch
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AEquipableItems",
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
                .ForeignKey("dbo.AEquipableItems", t => t.ItemId, cascadeDelete: true)
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
                .ForeignKey("dbo.Consumables", t => t.ConsumableId, cascadeDelete: true)
                .Index(t => t.ConsumableId);
            
            CreateTable(
                "dbo.Consumables",
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
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 25),
                        Email = c.String(nullable: false),
                        ActivationCode = c.Guid(nullable: false),
                        IsActivated = c.Boolean(nullable: false),
                        SoulSlots = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserServerSlots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ServerId = c.Guid(nullable: false),
                        SoulId = c.Guid(nullable: false),
                        User_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.GameServers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        DatabaseName = c.String(nullable: false),
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
                "dbo.UserRoles",
                c => new
                    {
                        User_Id = c.Guid(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.Role_Id })
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.Role_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.Role_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserServerSlots", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "Role_Id", "dbo.Roles");
            DropForeignKey("dbo.UserRoles", "User_Id", "dbo.Users");
            DropForeignKey("dbo.ConsumableEffects", "ConsumableId", "dbo.Consumables");
            DropForeignKey("dbo.Inscriptions", "PageId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "BookId", "dbo.Books");
            DropForeignKey("dbo.ItemStats", "ItemId", "dbo.AEquipableItems");
            DropIndex("dbo.UserRoles", new[] { "Role_Id" });
            DropIndex("dbo.UserRoles", new[] { "User_Id" });
            DropIndex("dbo.UserServerSlots", new[] { "User_Id" });
            DropIndex("dbo.ConsumableEffects", new[] { "ConsumableId" });
            DropIndex("dbo.Inscriptions", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "BookId" });
            DropIndex("dbo.ItemStats", new[] { "ItemId" });
            DropTable("dbo.UserRoles");
            DropTable("dbo.GameServerTokens");
            DropTable("dbo.GameServers");
            DropTable("dbo.UserServerSlots");
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.Consumables");
            DropTable("dbo.ConsumableEffects");
            DropTable("dbo.Inscriptions");
            DropTable("dbo.Pages");
            DropTable("dbo.Books");
            DropTable("dbo.ItemStats");
            DropTable("dbo.AEquipableItems");
        }
    }
}
