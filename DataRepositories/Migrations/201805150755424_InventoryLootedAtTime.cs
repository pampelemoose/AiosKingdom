namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryLootedAtTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventorySlots", "LootedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventorySlots", "LootedAt");
        }
    }
}
