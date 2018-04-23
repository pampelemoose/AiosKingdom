namespace Server.GameServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SoulId = c.Guid(nullable: false),
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Souls", t => t.SoulId, cascadeDelete: true)
                .Index(t => t.SoulId);
            
            CreateTable(
                "dbo.Souls",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SoulId = c.Guid(nullable: false),
                        Name = c.String(),
                        TimePlayed = c.Single(nullable: false),
                        Level = c.Int(nullable: false),
                        CurrentExperience = c.Int(nullable: false),
                        Stamina = c.Int(nullable: false),
                        Mana = c.Int(nullable: false),
                        Strength = c.Int(nullable: false),
                        Agility = c.Int(nullable: false),
                        Intelligence = c.Int(nullable: false),
                        Wisdom = c.Int(nullable: false),
                        Spirits = c.Int(nullable: false),
                        Embers = c.Int(nullable: false),
                        Shards = c.Int(nullable: false),
                        Bits = c.Int(nullable: false),
                        Equipment_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Equipments", t => t.Equipment_Id)
                .Index(t => t.Equipment_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventorySlots", "SoulId", "dbo.Souls");
            DropForeignKey("dbo.Souls", "Equipment_Id", "dbo.Equipments");
            DropIndex("dbo.Souls", new[] { "Equipment_Id" });
            DropIndex("dbo.InventorySlots", new[] { "SoulId" });
            DropTable("dbo.Souls");
            DropTable("dbo.InventorySlots");
            DropTable("dbo.Equipments");
        }
    }
}
