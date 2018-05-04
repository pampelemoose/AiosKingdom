namespace DataRepositories.Migrations.Dispatch
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PasswordLenghtRemoved : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Password", c => c.String(nullable: false, maxLength: 25));
        }
    }
}
