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

            try
            {
                context.Versions.RemoveRange(context.Versions);
                context.Kingdoms.RemoveRange(context.Kingdoms);
                context.Towns.RemoveRange(context.Towns);
                context.Tokens.RemoveRange(context.Tokens);


                context.Users.RemoveRange(context.Users);
                context.Roles.RemoveRange(context.Roles);

                context.AppUsers.RemoveRange(context.AppUsers);

                context.Items.RemoveRange(context.Items);
                context.ItemStats.RemoveRange(context.ItemStats);
                context.ItemEffects.RemoveRange(context.ItemEffects);

                context.Books.RemoveRange(context.Books);
                context.Talents.RemoveRange(context.Talents);
                context.Inscriptions.RemoveRange(context.Inscriptions);

                context.Souls.RemoveRange(context.Souls);
                context.Equipments.RemoveRange(context.Equipments);
                context.Inventories.RemoveRange(context.Inventories);
                context.Knowledges.RemoveRange(context.Knowledges);

                context.Market.RemoveRange(context.Market);
                context.MarketHistory.RemoveRange(context.MarketHistory);

                context.Monsters.RemoveRange(context.Monsters);
                context.Loots.RemoveRange(context.Loots);
                context.Phases.RemoveRange(context.Phases);

                context.Adventures.RemoveRange(context.Adventures);
                context.Taverns.RemoveRange(context.Taverns);
                context.QuestObjectives.RemoveRange(context.QuestObjectives);
                context.Quests.RemoveRange(context.Quests);
                context.ShopItems.RemoveRange(context.ShopItems);
                context.Enemies.RemoveRange(context.Enemies);
                context.NpcDialogues.RemoveRange(context.NpcDialogues);
                context.Npcs.RemoveRange(context.Npcs);

                context.AdventureUnlocked.RemoveRange(context.AdventureUnlocked);

                context.SaveChanges();

                var version = context.Versions.Add(new DataModels.Version { Id = Guid.NewGuid(), Low = 1, Mid = 0, High = 0 });

                context.Roles.Add(new DataModels.Role { Name = "User" });
                context.Roles.Add(new DataModels.Role { Name = "Admin" });
                context.Roles.Add(new DataModels.Role { Name = "SuperAdmin" });

                context.Roles.Add(new DataModels.Role { Name = "Backer" });
                context.Roles.Add(new DataModels.Role { Name = "Parent" });
                context.Roles.Add(new DataModels.Role { Name = "Child" });

                context.Roles.Add(new DataModels.Role { Name = "ItemSmith" });

                context.Roles.Add(new DataModels.Role { Name = "BookWriter" });
                context.Roles.Add(new DataModels.Role { Name = "AdventureCreator" });
                context.Roles.Add(new DataModels.Role { Name = "MonsterCreator" });
                context.Roles.Add(new DataModels.Role { Name = "MarketRegulator" });

                context.Roles.Add(new DataModels.Role { Name = "TicketMaster" });
                context.Roles.Add(new DataModels.Role { Name = "Ticketer" });
                context.Roles.Add(new DataModels.Role { Name = "ForumAdmin" });

                var kingdom = context.Kingdoms.Add(new DataModels.Kingdom
                {
                    Id = Guid.NewGuid(),

                    Name = "Aios",

                    LevelGap = 5,
                    CurrentMaxLevel = 10,
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
                    Roles = context.Roles.Select(r => r.Name).ToList()
                });

                var bag = context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Bag, null)
                {
                    Id = Guid.NewGuid(),
                    VersionId = version.Id,
                    Vid = Guid.NewGuid(),
                    Name = "Small Pouch",
                    Description = "This is the default bag.",
                    ItemLevel = 1,
                    Quality = DataModels.Items.ItemQuality.Common,
                    UseLevelRequired = 1,
                    SellingPrice = 1,
                    SlotCount = 10,
                    Space = 1,
                });

                context.Towns.Add(new DataModels.Town
                {
                    Id = Guid.Parse("22c85f7f-79fe-47dc-b116-fcf09bc507dd"),
                    VersionId = version.Id,

                    Host = "127.0.0.1",
                    Port = 4242,

                    KingdomId = kingdom.Id,
                    Name = "Churros",
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

                    DefaultBagId = bag.Vid
                });

                _createBasicItems(context, version);

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

        private void _createBasicItems(AiosKingdomContext context, DataModels.Version version)
        {
            // CONSUMABLE ITEMS
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Consumable, null)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Earth Health Restoration Pill",
                Description = "Low grade pill that restore a small amount of health.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect> { new DataModels.Items.ItemEffect
            {
                Type = DataModels.Items.EffectType.RestoreHealth,
                Name = "Earth health regeneration",
                Description = "Restore 10 H.P.",
                AffectTime = 1,
                AffectValue = 10
            } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Consumable, null)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Heaven Health Restoration Pill",
                Description = "Middle grade pill that restore a medium amount of health.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Rare,
                UseLevelRequired = 5,
                SellingPrice = 10,
                Space = 1,
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect> { new DataModels.Items.ItemEffect
            {
                Type = DataModels.Items.EffectType.RestoreHealth,
                Name = "Heaven health regeneration",
                Description = "Restore 50 H.P.",
                AffectTime = 1,
                AffectValue = 50
            } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Consumable, null)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "God Health Restoration Pill",
                Description = "High grade pill that restore a big amount of health.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Epic,
                UseLevelRequired = 10,
                SellingPrice = 100,
                Space = 1,
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect> { new DataModels.Items.ItemEffect
            {
                Type = DataModels.Items.EffectType.RestoreHealth,
                Name = "God health regeneration",
                Description = "Restore 300 H.P.",
                AffectTime = 1,
                AffectValue = 300
            } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Consumable, null)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Earth Mana Restoration Pill",
                Description = "Low grade pill that restore a small amount of mana.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect> { new DataModels.Items.ItemEffect
            {
                Type = DataModels.Items.EffectType.ResoreMana,
                Name = "Earth mana regeneration",
                Description = "Restore 5 M.P.",
                AffectTime = 1,
                AffectValue = 5
            } }
            });

            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Consumable, null)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Heaven Mana Restoration Pill",
                Description = "Low grade pill that restore a small amount of mana.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Rare,
                UseLevelRequired = 5,
                SellingPrice = 10,
                Space = 1,
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect> { new DataModels.Items.ItemEffect
            {
                Type = DataModels.Items.EffectType.ResoreMana,
                Name = "Heaven mana regeneration",
                Description = "Restore 20 M.P.",
                AffectTime = 1,
                AffectValue = 10
            } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Consumable, null)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "God Mana Restoration Pill",
                Description = "Low grade pill that restore a small amount of mana.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Epic,
                UseLevelRequired = 10,
                SellingPrice = 100,
                Space = 1,
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect> { new DataModels.Items.ItemEffect
            {
                Type = DataModels.Items.EffectType.ResoreMana,
                Name = "God mana regeneration",
                Description = "Restore 100 M.P.",
                AffectTime = 1,
                AffectValue = 100
            } }
            });

            // STATS
            //var stamina1 = context.ItemStats.Add(new DataModels.Items.ItemStat { ItemId = Guid.NewGuid(), Type = DataModels.Soul.Stats.Stamina, StatValue = 1 });
            //var energy1 = context.ItemStats.Add(new DataModels.Items.ItemStat { ItemId = Guid.NewGuid(), Type = DataModels.Soul.Stats.Energy, StatValue = 1 });
            //var strength1 = context.ItemStats.Add(new DataModels.Items.ItemStat { ItemId = Guid.NewGuid(), Type = DataModels.Soul.Stats.Strength, StatValue = 1 });
            //var agility1 = context.ItemStats.Add(new DataModels.Items.ItemStat { ItemId = Guid.NewGuid(), Type = DataModels.Soul.Stats.Agility, StatValue = 1 });
            //var intelligence1 = context.ItemStats.Add(new DataModels.Items.ItemStat { ItemId = Guid.NewGuid(), Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 });
            //var wisdom1 = context.ItemStats.Add(new DataModels.Items.ItemStat { ItemId = Guid.NewGuid(), Type = DataModels.Soul.Stats.Wisdom, StatValue = 1 });

            // ARMOR ITEMS
            // LEATHER SET
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Pants)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Pants",
                Description = "Leather pants that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Torso)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Torso",
                Description = "Leather torso that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Belt)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Belt",
                Description = "Leather belt that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Feet)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Shoes",
                Description = "Leather shoes that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Hand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Gloves",
                Description = "Leather gloves that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Head)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Hood",
                Description = "Leather hood that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Leg)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Leather Lower Pants",
                Description = "Leather lower pants that offers below average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 1,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });

            // CLOTH SET
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Pants)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Pants",
                Description = "Cloth pants that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Torso)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Torso",
                Description = "Cloth torso that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Belt)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Belt",
                Description = "Cloth belt that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Feet)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Shoes",
                Description = "Cloth shoes that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Hand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Gloves",
                Description = "Cloth gloves that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Head)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Hood",
                Description = "Cloth hood that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Leg)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Cloth Lower Pants",
                Description = "Cloth lower pants that offers below average magic protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 0,
                MagicArmorValue = 1,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 } }
            });

            // PLATE SET
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Pants)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Pants",
                Description = "Plate pants that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Torso)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Torso",
                Description = "Plate torso that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Belt)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Belt",
                Description = "Plate belt that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Feet)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Shoes",
                Description = "Plate shoes that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Hand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Gloves",
                Description = "Plate gloves that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Head)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Hood",
                Description = "Plate hood that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Armor, DataModels.Items.ItemSlot.Leg)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Plate Lower Pants",
                Description = "Plate lower pants that offers average protection.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 3,
                MagicArmorValue = 0,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
        }
    }
}
