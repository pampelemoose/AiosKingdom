namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GameServer_RemoveDatabaseName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.GameServers", "DatabaseName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GameServers", "DatabaseName", c => c.String(nullable: false));
        }
    }
}
