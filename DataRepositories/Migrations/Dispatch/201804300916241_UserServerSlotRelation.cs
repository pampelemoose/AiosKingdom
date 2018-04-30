namespace DataRepositories.Migrations.Dispatch
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserServerSlotRelation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserServerSlots",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ServerIdentifier = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Users", "SoulSlots", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserServerSlots", "UserId", "dbo.Users");
            DropIndex("dbo.UserServerSlots", new[] { "UserId" });
            DropColumn("dbo.Users", "SoulSlots");
            DropTable("dbo.UserServerSlots");
        }
    }
}
