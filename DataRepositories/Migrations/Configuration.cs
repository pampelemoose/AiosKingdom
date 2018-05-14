namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DataRepositories.AiosKingdomContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataRepositories.AiosKingdomContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            foreach (var entity in context.Roles)
                context.Roles.Remove(entity);

            foreach (var entity in context.Users)
                context.Users.Remove(entity);

            foreach (var entity in context.Servers)
                context.Servers.Remove(entity);

            foreach (var entity in context.Market)
                context.Market.Remove(entity);

            context.SaveChanges();

            context.Roles.Add(new DataModels.Role { Name = "User" });
            context.Roles.Add(new DataModels.Role { Name = "Admin" });
            context.Roles.Add(new DataModels.Role { Name = "SuperAdmin" });

            context.Roles.Add(new DataModels.Role { Name = "Backer" });
            context.Roles.Add(new DataModels.Role { Name = "Parent" });
            context.Roles.Add(new DataModels.Role { Name = "Child" });

            context.SaveChanges();

            context.Users.Add(new DataModels.User
            {
                Id = Guid.NewGuid(),
                Email = "pampe@lemoosecorp.com",
                Username = "pampe",
                Password = DataModels.User.EncryptPassword("pampe123"),
                ActivationCode = Guid.NewGuid(),
                IsActivated = true,
                Roles = context.Roles.ToList(),
                SoulSlots = 1
            });

            context.Servers.Add(new DataModels.GameServer
            {
                Id = Guid.NewGuid(),
                Host = "127.0.0.1",
                Port = 4242,
                Name = "Server 1",
                Difficulty = DataModels.ServerDifficulty.Easy,
                Online = false,
                SlotAvailable = 0,
                SlotLimit = 100,
                BaseExperience = 400,
                ExperiencePerLevelRatio = 1.2f,
                BaseHealth = 100,
                HealthPerLevelRatio = 10f,
                HealthPerStaminaRatio = 10f,
                BaseMana = 50,
                ManaPerLevelRatio = 5f,
                ManaPerEnergyRatio = 5f,
                SpiritsPerLevelUp = 10,
                EmbersPerLevelUp = 1
            });

            context.SaveChanges();
        }
    }
}
