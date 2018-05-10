namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignKeyFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Equipments", "Id", "dbo.Souls");
            DropIndex("dbo.Equipments", new[] { "Id" });
            AddColumn("dbo.Souls", "EquipmentId", c => c.Guid(nullable: false));
            CreateIndex("dbo.Souls", "EquipmentId");
            AddForeignKey("dbo.Souls", "EquipmentId", "dbo.Equipments", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Souls", "EquipmentId", "dbo.Equipments");
            DropIndex("dbo.Souls", new[] { "EquipmentId" });
            DropColumn("dbo.Souls", "EquipmentId");
            CreateIndex("dbo.Equipments", "Id");
            AddForeignKey("dbo.Equipments", "Id", "dbo.Souls", "Id");
        }
    }
}
