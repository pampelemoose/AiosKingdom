namespace DataRepositories.Migrations.Game
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class GameConfiguration : DbMigrationsConfiguration<DataRepositories.GameDbContext>
    {
        public GameConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations\Game";
        }

        protected override void Seed(DataRepositories.GameDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
