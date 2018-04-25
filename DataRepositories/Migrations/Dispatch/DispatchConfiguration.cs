namespace DataRepositories.Migrations.Dispatch
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class DispatchConfiguration : DbMigrationsConfiguration<DataRepositories.DispatchDbContext>
    {
        public DispatchConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\Dispatch";
        }

        protected override void Seed(DataRepositories.DispatchDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Roles.Add(new DataModels.Role { Name = "User" });
            context.Roles.Add(new DataModels.Role { Name = "Admin" });
            context.Roles.Add(new DataModels.Role { Name = "SuperAdmin" });

            context.Roles.Add(new DataModels.Role { Name = "Backer" });

            context.Roles.Add(new DataModels.Role { Name = "Parent" });
            context.Roles.Add(new DataModels.Role { Name = "Child" });
        }
    }
}
