namespace DataRepositories.Migrations
{
    using Newtonsoft.Json;
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
                context.Bookstores.RemoveRange(context.Bookstores);

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

                context.SaveChanges();

                _createBasicItems(context, version);
                _createBasicBooks(context, version);
                _createMonsters(context, version);
                _createAdventures(context, version);

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

        #region ITEMS
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
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect>
                {
                    new DataModels.Items.ItemEffect
                    {
                        Id = Guid.NewGuid(),
                        Type = DataModels.Items.EffectType.RestoreHealth,
                        Name = "Earth health regeneration",
                        Description = "Restore 10 H.P.",
                        AffectTime = 1,
                        AffectValue = 10
                    }
                }
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
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect>
                {
                    new DataModels.Items.ItemEffect
                    {
                        Id = Guid.NewGuid(),
                        Type = DataModels.Items.EffectType.RestoreHealth,
                        Name = "Heaven health regeneration",
                        Description = "Restore 50 H.P.",
                        AffectTime = 1,
                        AffectValue = 50
                    }
                }
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
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect>
                {
                    new DataModels.Items.ItemEffect
                    {
                        Id = Guid.NewGuid(),
                        Type = DataModels.Items.EffectType.RestoreHealth,
                        Name = "God health regeneration",
                        Description = "Restore 300 H.P.",
                        AffectTime = 1,
                        AffectValue = 300
                    }
                }
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
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect>
                {
                    new DataModels.Items.ItemEffect
                    {
                        Id = Guid.NewGuid(),
                        Type = DataModels.Items.EffectType.ResoreMana,
                        Name = "Earth mana regeneration",
                        Description = "Restore 5 M.P.",
                        AffectTime = 1,
                        AffectValue = 5
                    }
                }
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
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect>
                {
                    new DataModels.Items.ItemEffect
                    {
                        Id = Guid.NewGuid(),
                        Type = DataModels.Items.EffectType.ResoreMana,
                        Name = "Heaven mana regeneration",
                        Description = "Restore 20 M.P.",
                        AffectTime = 1,
                        AffectValue = 10
                    }
                }
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
                Effects = new System.Collections.Generic.List<DataModels.Items.ItemEffect>
                {
                    new DataModels.Items.ItemEffect
                    {
                        Id = Guid.NewGuid(),
                        Type = DataModels.Items.EffectType.ResoreMana,
                        Name = "God mana regeneration",
                        Description = "Restore 100 M.P.",
                        AffectTime = 1,
                        AffectValue = 100
                    }
                }
            });

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

            // WEAPONS
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Axe, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Axe",
                Description = "Small axe that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 2,
                MaxDamages = 4,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Strength, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Bow, DataModels.Items.ItemSlot.TwoHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Bow",
                Description = "Bow that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 4,
                MaxDamages = 8,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Crossbow, DataModels.Items.ItemSlot.TwoHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Crossbow",
                Description = "Crossbow that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 3,
                MaxDamages = 6,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Dagger, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Dagger",
                Description = "Small dagger that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 1,
                MaxDamages = 4,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Fist, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Handplate",
                Description = "Small handplate that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 1,
                MaxDamages = 3,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Strength, StatValue = 2 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Gun, DataModels.Items.ItemSlot.TwoHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Riffle",
                Description = "Riffle that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 3,
                MaxDamages = 8,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Wisdom, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Mace, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Mace",
                Description = "Small mace that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 2,
                MaxDamages = 4,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Strength, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Polearm, DataModels.Items.ItemSlot.TwoHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Polearm",
                Description = "Polearm that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 4,
                MaxDamages = 8,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Shield, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Shield",
                Description = "Small shield that can defend a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                ArmorValue = 4,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Stamina, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Staff, DataModels.Items.ItemSlot.TwoHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Magic Staff",
                Description = "Magic staff that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 1,
                MaxDamages = 3,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 2 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Staff, DataModels.Items.ItemSlot.TwoHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Staff",
                Description = "Staff that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 4,
                MaxDamages = 8,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 2 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Sword, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Sword",
                Description = "Small sword that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 2,
                MaxDamages = 4,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Strength, StatValue = 1 } }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Wand, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Wand",
                Description = "Small want that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 1,
                MaxDamages = 2,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat>
                {
                    new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Intelligence, StatValue = 1 },
                    new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Wisdom, StatValue = 1 }
                }
            });
            context.Items.Add(new DataModels.Items.Item(DataModels.Items.ItemType.Whip, DataModels.Items.ItemSlot.OneHand)
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Small Whip",
                Description = "Small whip that can deal a little amount of damages.",
                ItemLevel = 1,
                Quality = DataModels.Items.ItemQuality.Common,
                UseLevelRequired = 1,
                SellingPrice = 1,
                Space = 1,
                MinDamages = 2,
                MaxDamages = 6,
                Stats = new System.Collections.Generic.List<DataModels.Items.ItemStat> { new DataModels.Items.ItemStat { Type = DataModels.Soul.Stats.Agility, StatValue = 1 } }
            });

            context.SaveChanges();
        }
        #endregion

        #region BOOKS
        private void _createBasicBooks(AiosKingdomContext context, DataModels.Version version)
        {
            _createSimplePunchBook(context, version);
            _createEnergyBallBook(context, version);

            _createMonsterBooks(context, version);

            context.SaveChanges();
        }

        private void _createSimplePunchBook(AiosKingdomContext context, DataModels.Version version)
        {
            var punchPhysicalDamageStrInscription = Guid.NewGuid();
            var punchPhysicalDamageAgiInscription = Guid.NewGuid();
            context.Books.Add(new DataModels.Skills.Book
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Simple Punch",
                Description = "A simple punch that deal small physical damages.",
                Quality = DataModels.Skills.BookQuality.TierOne,
                Action = DataModels.Skills.BookAction.Hit,
                Repetition = 1,
                RequireWeapon = false,
                ManaCost = 5,
                Cooldown = 0,
                ExperienceCost = 0,
                Inscriptions = new System.Collections.Generic.List<DataModels.Skills.Inscription>
                {
                    new DataModels.Skills.Inscription
                    {
                        Id = punchPhysicalDamageStrInscription,
                        Type = DataModels.Skills.InscriptionType.PhysicDamages,
                        BaseValue = 1,
                        StatType = DataModels.Soul.Stats.Strength,
                        Ratio = 1,
                        Duration = 0,
                        IncludeWeaponDamages = false,
                        PreferredWeaponTypes = new System.Collections.Generic.List<DataModels.Items.ItemType>
                        {
                            DataModels.Items.ItemType.Fist
                        },
                        PreferredWeaponDamagesRatio = 2f
                    },
                    new DataModels.Skills.Inscription
                    {
                        Id = punchPhysicalDamageAgiInscription,
                        Type = DataModels.Skills.InscriptionType.PhysicDamages,
                        BaseValue = 1,
                        StatType = DataModels.Soul.Stats.Agility,
                        Ratio = 0.5f,
                        Duration = 0,
                        IncludeWeaponDamages = false,
                        PreferredWeaponTypes = new System.Collections.Generic.List<DataModels.Items.ItemType>
                        {
                            DataModels.Items.ItemType.Fist
                        },
                        PreferredWeaponDamagesRatio = 1f
                    }
                },
                Talents = new System.Collections.Generic.List<DataModels.Skills.Talent>
                {
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 0,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchPhysicalDamageStrInscription,
                        TalentPointsRequired = 1,
                        Type = DataModels.Skills.TalentType.BaseValue,
                        Value = 2
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 1,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchPhysicalDamageAgiInscription,
                        TalentPointsRequired = 1,
                        Type = DataModels.Skills.TalentType.StatValue,
                        Value = 2
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 2,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchPhysicalDamageAgiInscription,
                        TalentPointsRequired = 2,
                        Type = DataModels.Skills.TalentType.StatValue,
                        Value = 5
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 3,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchPhysicalDamageStrInscription,
                        TalentPointsRequired = 3,
                        Type = DataModels.Skills.TalentType.Ratio,
                        Value = 2
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 4,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.None },
                        TargetInscription = punchPhysicalDamageStrInscription,
                        TalentPointsRequired = 5,
                        Type = DataModels.Skills.TalentType.StatValue,
                        Value = 10
                    },
                }
            });
        }

        private void _createEnergyBallBook(AiosKingdomContext context, DataModels.Version version)
        {
            var punchMagicDamageIntInscription = Guid.NewGuid();
            var punchMagicDamageWisInscription = Guid.NewGuid();
            context.Books.Add(new DataModels.Skills.Book
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Energy Ball",
                Description = "An energy ball that deal small magic damages.",
                Quality = DataModels.Skills.BookQuality.TierOne,
                Action = DataModels.Skills.BookAction.Cast,
                Repetition = 1,
                RequireWeapon = false,
                ManaCost = 5,
                Cooldown = 0,
                ExperienceCost = 0,
                Inscriptions = new System.Collections.Generic.List<DataModels.Skills.Inscription>
                {
                    new DataModels.Skills.Inscription
                    {
                        Id = punchMagicDamageIntInscription,
                        Type = DataModels.Skills.InscriptionType.MagicDamages,
                        BaseValue = 1,
                        StatType = DataModels.Soul.Stats.Intelligence,
                        Ratio = 2,
                        Duration = 0,
                        IncludeWeaponDamages = false,
                        PreferredWeaponTypes = new System.Collections.Generic.List<DataModels.Items.ItemType>
                        {
                            DataModels.Items.ItemType.Book,
                        },
                        PreferredWeaponDamagesRatio = 2f
                    },
                    new DataModels.Skills.Inscription
                    {
                        Id = punchMagicDamageWisInscription,
                        Type = DataModels.Skills.InscriptionType.MagicDamages,
                        BaseValue = 1,
                        StatType = DataModels.Soul.Stats.Wisdom,
                        Ratio = 0.5f,
                        Duration = 0,
                        IncludeWeaponDamages = false,
                        PreferredWeaponTypes = new System.Collections.Generic.List<DataModels.Items.ItemType>
                        {
                            DataModels.Items.ItemType.Book
                        },
                        PreferredWeaponDamagesRatio = 1f
                    }
                },
                Talents = new System.Collections.Generic.List<DataModels.Skills.Talent>
                {
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 0,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchMagicDamageIntInscription,
                        TalentPointsRequired = 1,
                        Type = DataModels.Skills.TalentType.BaseValue,
                        Value = 2
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 1,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchMagicDamageWisInscription,
                        TalentPointsRequired = 1,
                        Type = DataModels.Skills.TalentType.StatValue,
                        Value = 2
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 2,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchMagicDamageWisInscription,
                        TalentPointsRequired = 2,
                        Type = DataModels.Skills.TalentType.StatValue,
                        Value = 5
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 3,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.Next },
                        TargetInscription = punchMagicDamageIntInscription,
                        TalentPointsRequired = 3,
                        Type = DataModels.Skills.TalentType.Ratio,
                        Value = 2
                    },
                    new DataModels.Skills.Talent
                    {
                        Id = Guid.NewGuid(),
                        Branch = 0,
                        Leaf = 4,
                        Unlocks = new System.Collections.Generic.List<DataModels.Skills.TalentUnlock> { DataModels.Skills.TalentUnlock.None },
                        TargetInscription = punchMagicDamageIntInscription,
                        TalentPointsRequired = 5,
                        Type = DataModels.Skills.TalentType.StatValue,
                        Value = 10
                    },
                }
            });
        }

        // MONSTER SKILLS
        private void _createMonsterBooks(AiosKingdomContext context, DataModels.Version version)
        {
            _createScratchBook(context, version);
        }

        private void _createScratchBook(AiosKingdomContext context, DataModels.Version version)
        {
            var scratchStrInscription = Guid.NewGuid();
            context.Books.Add(new DataModels.Skills.Book
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Scratch",
                Description = "Scratch the enemy, inflicting a small amount of physical damages.",
                Quality = DataModels.Skills.BookQuality.TierOne,
                Action = DataModels.Skills.BookAction.Hit,
                Repetition = 1,
                RequireWeapon = false,
                ManaCost = 1,
                Cooldown = 0,
                ExperienceCost = 0,
                Inscriptions = new System.Collections.Generic.List<DataModels.Skills.Inscription>
                {
                    new DataModels.Skills.Inscription
                    {
                        Id = scratchStrInscription,
                        Type = DataModels.Skills.InscriptionType.PhysicDamages,
                        BaseValue = 1,
                        StatType = DataModels.Soul.Stats.Strength,
                        Ratio = 2,
                        Duration = 0
                    }
                }
            });
        }
        #endregion

        #region MONSTERS
        private void _createMonsters(AiosKingdomContext context, DataModels.Version version)
        {
            _createWolfMonster(context, version);

            context.SaveChanges();
        }

        private void _createWolfMonster(AiosKingdomContext context, DataModels.Version version)
        {
            var scratchSkill = context.Books.FirstOrDefault(s => s.Name == "Scratch");
            context.Monsters.Add(new DataModels.Monsters.Monster
            {
                Id = Guid.NewGuid(),
                Types = new System.Collections.Generic.List<DataModels.Monsters.MonsterType> { DataModels.Monsters.MonsterType.Animal },
                Name = "Wolf",
                Description = "Lonely wolf trying to survive. He is aggressive and fight for his life.",
                Story = "After a fight in the pack, he was forced to live on his own. Fighting for food everyday, he became ferocious towards all living beings.",
                BaseHealth = 5,
                HealthPerLevel = 2,
                BaseExperience = 10,
                ExperiencePerLevelRatio = 1.8f,
                StaminaPerLevel = 2,
                EnergyPerLevel = 1,
                StrengthPerLevel = 2,
                AgilityPerLevel = 1,
                IntelligencePerLevel = 0,
                WisdomPerLevel = 0,
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Phases = new System.Collections.Generic.List<DataModels.Monsters.Phase> { new DataModels.Monsters.Phase { Id = Guid.NewGuid(), BookVid = scratchSkill.Vid } },
                Loots = new System.Collections.Generic.List<DataModels.Monsters.Loot> { }
            });
        }
        #endregion

        #region ADVENTURES
        private void _createAdventures(AiosKingdomContext context, DataModels.Version version)
        {
            _createBookstores(context, version);
            _createTaverns(context, version);
            _createNpcs(context, version);

            context.SaveChanges();

            _createClassChoiceAdventure(context, version);

            context.SaveChanges();
        }

        private void _createBookstores(AiosKingdomContext context, DataModels.Version version)
        {
            context.Bookstores.Add(new DataModels.Adventures.Bookstore
            {
                Id = Guid.NewGuid(),
                Name = "Beginner's Bookstore",
                VersionId = version.Id,
                Vid = Guid.Parse("A1CDC7A7-D86B-4829-A115-4219C709492C"),
                Books = new System.Collections.Generic.List<DataModels.Adventures.BookstoreItem>()
            });
        }

        private void _createTaverns(AiosKingdomContext context, DataModels.Version version)
        {
            context.Taverns.Add(new DataModels.Adventures.Tavern
            {
                Id = Guid.NewGuid(),
                Name = "Beginner's Tavern",
                VersionId = version.Id,
                Vid = Guid.Parse("E6C52ADC-7408-4CB6-BCEC-0D1D4B71C461"),
                FoodCost = 1,
                FoodHealth = 20,
                RestShardCost = 1,
                RestStamina = 10,
                ShopItems = new System.Collections.Generic.List<DataModels.Adventures.ShopItem>()
            });
        }

        private void _createNpcs(AiosKingdomContext context, DataModels.Version version)
        {
            context.Npcs.Add(new DataModels.Adventures.Npc
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Nice Guy",
                Dialogues = new System.Collections.Generic.List<DataModels.Adventures.NpcDialogue>
                {
                    new DataModels.Adventures.NpcDialogue
                    {
                        Id = Guid.NewGuid(),
                        VersionId = version.Id,
                        Vid = Guid.NewGuid(),
                        Content = "Hello traveler, how is it going ?",
                        NextDialogues = new System.Collections.Generic.List<DataModels.Adventures.NpcDialogue>
                        {
                            new DataModels.Adventures.NpcDialogue
                            {
                                Id = Guid.NewGuid(),
                                VersionId = version.Id,
                                Vid = Guid.NewGuid(),
                                Content = "Doing fine, thanks."
                            },
                            new DataModels.Adventures.NpcDialogue
                            {
                                Id = Guid.NewGuid(),
                                VersionId = version.Id,
                                Vid = Guid.NewGuid(),
                                Content = "Not so good.."
                            },
                        }
                    }
                }
            });
        }

        private void _createClassChoiceAdventure(AiosKingdomContext context, DataModels.Version version)
        {
            //var wolf = context.Monsters.FirstOrDefault(m => m.Name == "Wolf");
            //var wolfEnemy = context.Enemies.Add(new DataModels.Adventures.Enemy
            //{
            //    Id = Guid.NewGuid(),
            //    VersionId = version.Id,
            //    Vid = Guid.NewGuid(),
            //    EnemyType = DataModels.Adventures.EnemyType.Normal,
            //    MonsterVid = wolf.Vid,
            //    Level = 1,
            //    ShardReward = 2
            //});

            var simplePunch = context.Books.FirstOrDefault(b => b.Name == "Simple Punch");
            var energyBall = context.Books.FirstOrDefault(b => b.Name == "Energy Ball");

            context.Adventures.Add(new DataModels.Adventures.Adventure
            {
                Id = Guid.NewGuid(),
                VersionId = version.Id,
                Vid = Guid.NewGuid(),
                Name = "Welcome to Aios Kingdom.",
                RequiredLevel = 1,
                MaxLevelAuthorized = 5,
                Repeatable = false,
                MapIdentifier = Guid.Parse("A17CB0E6-1D3C-42CD-AEC2-693B5FA0F0BE"),
                ExperienceReward = 100,
                ShardReward = 10,
                SpawnCoordinateX = 0,
                SpawnCoordinateY = 0,
                Quests = new System.Collections.Generic.List<DataModels.Adventures.Quest>
                {
                    new DataModels.Adventures.Quest
                    {
                        Id = Guid.NewGuid(),
                        VersionId = version.Id,
                        Vid = Guid.NewGuid(),
                        Name = "Choose your first skill.",
                        Description = "There are different path you can choose. Talk to people around and select your first skill.",
                        Objectives = new System.Collections.Generic.List<DataModels.Adventures.QuestObjective>
                        {
                            new DataModels.Adventures.QuestObjective
                            {
                                Id = Guid.NewGuid(),
                                VersionId = version.Id,
                                Vid = Guid.NewGuid(),
                                Title = "Learn a skill.",
                                Type = DataModels.Adventures.QuestObjective.ObjectiveType.LearnBook,
                                ObjectiveDataJson = JsonConvert.SerializeObject(new DataModels.Adventures.QuestObjectiveDataLearnBook
                                {
                                    NeedToLearnCount = 1,
                                    Books = new System.Collections.Generic.List<Guid>
                                    {
                                        simplePunch.Vid,
                                        energyBall.Vid
                                    }
                                })
                            }
                        }
                    }
                }
            });
        }
        #endregion
    }
}
