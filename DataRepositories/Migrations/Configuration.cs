namespace DataRepositories.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Text;

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

            return;

            try
            {
                context.Versions.RemoveRange(context.Versions);
                context.Kingdoms.RemoveRange(context.Kingdoms);
                context.Configs.RemoveRange(context.Configs);
                context.Tokens.RemoveRange(context.Tokens);


                context.Users.RemoveRange(context.Users);
                context.Roles.RemoveRange(context.Roles);

                context.AppUsers.RemoveRange(context.AppUsers);

                context.Bags.RemoveRange(context.Bags);
                context.Armors.RemoveRange(context.Armors);
                context.Weapons.RemoveRange(context.Weapons);
                context.ItemStats.RemoveRange(context.ItemStats);
                context.Consumables.RemoveRange(context.Consumables);
                context.ConsumableEffects.RemoveRange(context.ConsumableEffects);

                context.Books.RemoveRange(context.Books);
                context.Pages.RemoveRange(context.Pages);
                context.Inscriptions.RemoveRange(context.Inscriptions);

                context.Souls.RemoveRange(context.Souls);
                context.Equipments.RemoveRange(context.Equipments);
                context.Inventories.RemoveRange(context.Inventories);
                context.Knowledges.RemoveRange(context.Knowledges);

                context.Market.RemoveRange(context.Market);

                context.Monsters.RemoveRange(context.Monsters);
                context.Loots.RemoveRange(context.Loots);
                context.Phases.RemoveRange(context.Phases);

                context.Dungeons.RemoveRange(context.Dungeons);
                context.Rooms.RemoveRange(context.Rooms);
                context.ShopItems.RemoveRange(context.ShopItems);
                context.Enemies.RemoveRange(context.Enemies);

                context.DungeonProgresses.RemoveRange(context.DungeonProgresses);

                context.SaveChanges();

                var version = context.Versions.Add(new DataModels.Version { Id = Guid.NewGuid(), Low = 1, Mid = 0, High = 0 });

                context.Roles.Add(new DataModels.Role { Name = "User" });
                context.Roles.Add(new DataModels.Role { Name = "Admin" });
                context.Roles.Add(new DataModels.Role { Name = "SuperAdmin" });

                context.Roles.Add(new DataModels.Role { Name = "Backer" });
                context.Roles.Add(new DataModels.Role { Name = "Parent" });
                context.Roles.Add(new DataModels.Role { Name = "Child" });

                var kingdom = context.Kingdoms.Add(new DataModels.Kingdom
                {
                    Id = Guid.NewGuid(),

                    Name = "Aios",

                    LevelGap = 5,
                    MaxLevelCountForGap = 100
                });

                context.SaveChanges();

                context.Users.Add(new DataModels.User
                {
                    Id = Guid.NewGuid(),
                    Email = "pampe@lemoosecorp.com",
                    Username = "pampe",
                    Password = DataModels.User.EncryptPassword("pampe123"),
                    ActivationCode = Guid.NewGuid(),
                    IsActivated = true,
                    Roles = context.Roles.ToList()
                });

                var bag = context.Bags.Add(new DataModels.Items.Bag
                {
                    Id = Guid.NewGuid(),
                    VersionId = version.Id,
                    ItemId = Guid.NewGuid(),
                    Name = "Small Pouch",
                    Description = "This is the default bag.",
                    ItemLevel = 1,
                    Quality = DataModels.Items.ItemQuality.Common,
                    UseLevelRequired = 1,
                    SlotCount = 10,
                    Space = 1,
                    Image = ""
                });

                context.Configs.Add(new DataModels.Config
                {
                    Id = Guid.NewGuid(),
                    VersionId = version.Id,

                    Host = "127.0.0.1",
                    Port = 4242,

                    KingdomId = kingdom.Id,
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
                    EmbersPerLevelUp = 1,

                    DefaultBagId = bag.ItemId
                });

                context.SaveChanges();


            }
            catch (DbEntityValidationException e)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        sb.AppendFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), e
                );
            }
        }
    }
}
