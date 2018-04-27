namespace DataRepositories.Migrations.Dispatch
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AEquipableItems", "WeaponType", c => c.Int());
            AddColumn("dbo.AEquipableItems", "MinDamages", c => c.Int());
            AddColumn("dbo.AEquipableItems", "MaxDamages", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AEquipableItems", "MaxDamages");
            DropColumn("dbo.AEquipableItems", "MinDamages");
            DropColumn("dbo.AEquipableItems", "WeaponType");
        }
    }
}
