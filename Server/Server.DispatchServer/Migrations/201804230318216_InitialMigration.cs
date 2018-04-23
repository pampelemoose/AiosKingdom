namespace Server.DispatchServer.Migrations
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
                        Name = c.String(),
                        Description = c.String(),
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
                        ArmorId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        StatValue = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Armors", t => t.ArmorId, cascadeDelete: true)
                .Index(t => t.ArmorId);
            
            CreateTable(
                "dbo.Bags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SlotCount = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
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
                        Name = c.String(),
                        Quality = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pages",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BookId = c.Guid(nullable: false),
                        Description = c.String(),
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
                        Description = c.String(),
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
                        Name = c.String(),
                        Description = c.String(),
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
                        Name = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        Type = c.Int(nullable: false),
                        Quality = c.Int(nullable: false),
                        ItemLevel = c.Int(nullable: false),
                        UseLevelRequired = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ConsumableEffects", "ConsumableId", "dbo.Consumables");
            DropForeignKey("dbo.Inscriptions", "PageId", "dbo.Pages");
            DropForeignKey("dbo.Pages", "BookId", "dbo.Books");
            DropForeignKey("dbo.ItemStats", "ArmorId", "dbo.Armors");
            DropIndex("dbo.ConsumableEffects", new[] { "ConsumableId" });
            DropIndex("dbo.Inscriptions", new[] { "PageId" });
            DropIndex("dbo.Pages", new[] { "BookId" });
            DropIndex("dbo.ItemStats", new[] { "ArmorId" });
            DropTable("dbo.Users");
            DropTable("dbo.Consumables");
            DropTable("dbo.ConsumableEffects");
            DropTable("dbo.Inscriptions");
            DropTable("dbo.Pages");
            DropTable("dbo.Books");
            DropTable("dbo.Bags");
            DropTable("dbo.ItemStats");
            DropTable("dbo.Armors");
        }
    }
}
