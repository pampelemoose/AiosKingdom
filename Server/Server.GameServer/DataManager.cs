using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class DataManager
    {
        private static DataManager _instance;
        public static DataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataManager();

                return _instance;
            }
        }

        private DataManager()
        {
        }

        private DataModels.Config _config;

        public void Initialize(DataModels.Config config)
        {
            _config = config;

            LoadArmors();
            LoadBags();
            LoadConsumables();
            LoadWeapons();

            LoadBooks();

            LoadDungeons();

            LoadMonsters();
        }

        public List<Network.Items.Armor> Armors { get; private set; }
        public List<Network.Items.Bag> Bags { get; private set; }
        public List<Network.Items.Consumable> Consumables { get; private set; }
        public List<Network.Items.Weapon> Weapons { get; private set; }

        public List<Network.Skills.Book> Books { get; private set; }

        public List<Network.Adventures.Dungeon> Dungeons { get; private set; }

        public List<Network.Monsters.Monster> Monsters { get; private set; }

        public List<Network.MarketSlot> Market { get; private set; }

        private void LoadArmors()
        {
            var armors = DataRepositories.ArmorRepository.GetAllForVersion(_config.VersionId);
            var armorList = new List<Network.Items.Armor>();
            foreach (var armor in armors)
            {
                var arm = new Network.Items.Armor
                {
                    Id = armor.ItemId,
                    Name = armor.Name,
                    Description = armor.Description,
                    Image = armor.Image,
                    ItemLevel = armor.ItemLevel,
                    UseLevelRequired = armor.UseLevelRequired,
                    SellingPrice = armor.SellingPrice,
                    Space = armor.Space,
                    ArmorValue = armor.ArmorValue,
                };

                arm.Type = Network.Items.ItemType.Armor;

                switch (armor.Quality)
                {
                    case DataModels.Items.ItemQuality.Common:
                        arm.Quality = Network.Items.ItemQuality.Common;
                        break;
                    case DataModels.Items.ItemQuality.Uncommon:
                        arm.Quality = Network.Items.ItemQuality.Uncommon;
                        break;
                    case DataModels.Items.ItemQuality.Rare:
                        arm.Quality = Network.Items.ItemQuality.Rare;
                        break;
                    case DataModels.Items.ItemQuality.Epic:
                        arm.Quality = Network.Items.ItemQuality.Epic;
                        break;
                    case DataModels.Items.ItemQuality.Legendary:
                        arm.Quality = Network.Items.ItemQuality.Legendary;
                        break;
                }

                switch (armor.Part)
                {
                    case DataModels.Items.ArmorPart.Belt:
                        arm.Part = Network.Items.ArmorPart.Belt;
                        break;
                    case DataModels.Items.ArmorPart.Feet:
                        arm.Part = Network.Items.ArmorPart.Feet;
                        break;
                    case DataModels.Items.ArmorPart.Hand:
                        arm.Part = Network.Items.ArmorPart.Hand;
                        break;
                    case DataModels.Items.ArmorPart.Head:
                        arm.Part = Network.Items.ArmorPart.Head;
                        break;
                    case DataModels.Items.ArmorPart.Leg:
                        arm.Part = Network.Items.ArmorPart.Leg;
                        break;
                    case DataModels.Items.ArmorPart.Pants:
                        arm.Part = Network.Items.ArmorPart.Pants;
                        break;
                    case DataModels.Items.ArmorPart.Shoulder:
                        arm.Part = Network.Items.ArmorPart.Shoulder;
                        break;
                    case DataModels.Items.ArmorPart.Torso:
                        arm.Part = Network.Items.ArmorPart.Torso;
                        break;
                }

                arm.Stats = new List<Network.Items.ItemStat>();
                foreach (var stat in armor.Stats)
                {
                    var st = new Network.Items.ItemStat
                    {
                        StatValue = stat.StatValue
                    };

                    switch (stat.Type)
                    {
                        case DataModels.Soul.Stats.Agility:
                            st.Type = Network.Stats.Agility;
                            break;
                        case DataModels.Soul.Stats.Energy:
                            st.Type = Network.Stats.Energy;
                            break;
                        case DataModels.Soul.Stats.Intelligence:
                            st.Type = Network.Stats.Intelligence;
                            break;
                        case DataModels.Soul.Stats.Stamina:
                            st.Type = Network.Stats.Stamina;
                            break;
                        case DataModels.Soul.Stats.Strength:
                            st.Type = Network.Stats.Strength;
                            break;
                        case DataModels.Soul.Stats.Wisdom:
                            st.Type = Network.Stats.Wisdom;
                            break;
                    }

                    arm.Stats.Add(st);
                }

                armorList.Add(arm);
            }

            Armors = armorList;
        }

        private void LoadBags()
        {
            var bags = DataRepositories.BagRepository.GetAllForVersion(_config.VersionId);
            var bagList = new List<Network.Items.Bag>();
            foreach (var bag in bags)
            {
                var ba = new Network.Items.Bag
                {
                    Id = bag.ItemId,
                    Name = bag.Name,
                    Description = bag.Description,
                    Image = bag.Image,
                    ItemLevel = bag.ItemLevel,
                    UseLevelRequired = bag.UseLevelRequired,
                    SellingPrice = bag.SellingPrice,
                    Space = bag.Space,
                    SlotCount = bag.SlotCount
                };

                ba.Type = Network.Items.ItemType.Bag;

                switch (bag.Quality)
                {
                    case DataModels.Items.ItemQuality.Common:
                        ba.Quality = Network.Items.ItemQuality.Common;
                        break;
                    case DataModels.Items.ItemQuality.Uncommon:
                        ba.Quality = Network.Items.ItemQuality.Uncommon;
                        break;
                    case DataModels.Items.ItemQuality.Rare:
                        ba.Quality = Network.Items.ItemQuality.Rare;
                        break;
                    case DataModels.Items.ItemQuality.Epic:
                        ba.Quality = Network.Items.ItemQuality.Epic;
                        break;
                    case DataModels.Items.ItemQuality.Legendary:
                        ba.Quality = Network.Items.ItemQuality.Legendary;
                        break;
                }

                ba.Stats = new List<Network.Items.ItemStat>();
                foreach (var stat in bag.Stats)
                {
                    var st = new Network.Items.ItemStat
                    {
                        StatValue = stat.StatValue
                    };

                    switch (stat.Type)
                    {
                        case DataModels.Soul.Stats.Agility:
                            st.Type = Network.Stats.Agility;
                            break;
                        case DataModels.Soul.Stats.Energy:
                            st.Type = Network.Stats.Energy;
                            break;
                        case DataModels.Soul.Stats.Intelligence:
                            st.Type = Network.Stats.Intelligence;
                            break;
                        case DataModels.Soul.Stats.Stamina:
                            st.Type = Network.Stats.Stamina;
                            break;
                        case DataModels.Soul.Stats.Strength:
                            st.Type = Network.Stats.Strength;
                            break;
                        case DataModels.Soul.Stats.Wisdom:
                            st.Type = Network.Stats.Wisdom;
                            break;
                    }

                    ba.Stats.Add(st);
                }

                bagList.Add(ba);
            }

            Bags = bagList;
        }

        private void LoadConsumables()
        {
            var consumables = DataRepositories.ConsumableRepository.GetAllForVersion(_config.VersionId);
            var consumableList = new List<Network.Items.Consumable>();
            foreach (var consumable in consumables)
            {
                var cons = new Network.Items.Consumable
                {
                    Id = consumable.ItemId,
                    Name = consumable.Name,
                    Description = consumable.Description,
                    Image = consumable.Image,
                    ItemLevel = consumable.ItemLevel,
                    UseLevelRequired = consumable.UseLevelRequired,
                    SellingPrice = consumable.SellingPrice,
                    Space = consumable.Space
                };

                cons.Type = Network.Items.ItemType.Consumable;

                switch (consumable.Quality)
                {
                    case DataModels.Items.ItemQuality.Common:
                        cons.Quality = Network.Items.ItemQuality.Common;
                        break;
                    case DataModels.Items.ItemQuality.Uncommon:
                        cons.Quality = Network.Items.ItemQuality.Uncommon;
                        break;
                    case DataModels.Items.ItemQuality.Rare:
                        cons.Quality = Network.Items.ItemQuality.Rare;
                        break;
                    case DataModels.Items.ItemQuality.Epic:
                        cons.Quality = Network.Items.ItemQuality.Epic;
                        break;
                    case DataModels.Items.ItemQuality.Legendary:
                        cons.Quality = Network.Items.ItemQuality.Legendary;
                        break;
                }

                cons.Effects = new List<Network.Items.ConsumableEffect>();
                foreach (var effect in consumable.Effects)
                {
                    var eff = new Network.Items.ConsumableEffect
                    {
                        Id = effect.Id,
                        ConsumableId = effect.ConsumableId,
                        Name = effect.Name,
                        Description = effect.Description,
                        AffectTime = effect.AffectTime,
                        AffectValue = effect.AffectValue
                    };

                    switch (effect.Type)
                    {
                        case DataModels.Items.EffectType.IncreaseAgility:
                            eff.Type = Network.Items.EffectType.IncreaseAgility;
                            break;
                        case DataModels.Items.EffectType.IncreaseEnergy:
                            eff.Type = Network.Items.EffectType.IncreaseEnergy;
                            break;
                        case DataModels.Items.EffectType.IncreaseIntelligence:
                            eff.Type = Network.Items.EffectType.IncreaseIntelligence;
                            break;
                        case DataModels.Items.EffectType.IncreaseStamina:
                            eff.Type = Network.Items.EffectType.IncreaseStamina;
                            break;
                        case DataModels.Items.EffectType.IncreaseStrength:
                            eff.Type = Network.Items.EffectType.IncreaseStrength;
                            break;
                        case DataModels.Items.EffectType.IncreaseWisdom:
                            eff.Type = Network.Items.EffectType.IncreaseWisdom;
                            break;
                        case DataModels.Items.EffectType.ResoreMana:
                            eff.Type = Network.Items.EffectType.ResoreMana;
                            break;
                        case DataModels.Items.EffectType.RestoreHealth:
                            eff.Type = Network.Items.EffectType.RestoreHealth;
                            break;
                    }

                    cons.Effects.Add(eff);
                }

                consumableList.Add(cons);
            }

            Consumables = consumableList;
        }

        private void LoadWeapons()
        {
            var weapons = DataRepositories.WeaponRepository.GetAllForVersion(_config.VersionId);
            var weaponList = new List<Network.Items.Weapon>();
            foreach (var weapon in weapons)
            {
                var weap = new Network.Items.Weapon
                {
                    Id = weapon.ItemId,
                    Name = weapon.Name,
                    Description = weapon.Description,
                    Image = weapon.Image,
                    ItemLevel = weapon.ItemLevel,
                    UseLevelRequired = weapon.UseLevelRequired,
                    SellingPrice = weapon.SellingPrice,
                    Space = weapon.Space,
                    MinDamages = weapon.MinDamages,
                    MaxDamages = weapon.MaxDamages
                };

                weap.Type = Network.Items.ItemType.Weapon;

                switch (weapon.Quality)
                {
                    case DataModels.Items.ItemQuality.Common:
                        weap.Quality = Network.Items.ItemQuality.Common;
                        break;
                    case DataModels.Items.ItemQuality.Uncommon:
                        weap.Quality = Network.Items.ItemQuality.Uncommon;
                        break;
                    case DataModels.Items.ItemQuality.Rare:
                        weap.Quality = Network.Items.ItemQuality.Rare;
                        break;
                    case DataModels.Items.ItemQuality.Epic:
                        weap.Quality = Network.Items.ItemQuality.Epic;
                        break;
                    case DataModels.Items.ItemQuality.Legendary:
                        weap.Quality = Network.Items.ItemQuality.Legendary;
                        break;
                }

                switch (weapon.HandlingType)
                {
                    case DataModels.Items.HandlingType.OneHand:
                        weap.HandlingType = Network.Items.HandlingType.OneHand;
                        break;
                    case DataModels.Items.HandlingType.TwoHand:
                        weap.HandlingType = Network.Items.HandlingType.TwoHand;
                        break;
                }

                switch (weapon.WeaponType)
                {
                    case DataModels.Items.WeaponType.Axe:
                        weap.WeaponType = Network.Items.WeaponType.Axe;
                        break;
                    case DataModels.Items.WeaponType.Book:
                        weap.WeaponType = Network.Items.WeaponType.Book;
                        break;
                    case DataModels.Items.WeaponType.Bow:
                        weap.WeaponType = Network.Items.WeaponType.Bow;
                        break;
                    case DataModels.Items.WeaponType.Crossbow:
                        weap.WeaponType = Network.Items.WeaponType.Crossbow;
                        break;
                    case DataModels.Items.WeaponType.Dagger:
                        weap.WeaponType = Network.Items.WeaponType.Dagger;
                        break;
                    case DataModels.Items.WeaponType.Fist:
                        weap.WeaponType = Network.Items.WeaponType.Fist;
                        break;
                    case DataModels.Items.WeaponType.Gun:
                        weap.WeaponType = Network.Items.WeaponType.Gun;
                        break;
                    case DataModels.Items.WeaponType.Mace:
                        weap.WeaponType = Network.Items.WeaponType.Mace;
                        break;
                    case DataModels.Items.WeaponType.Polearm:
                        weap.WeaponType = Network.Items.WeaponType.Polearm;
                        break;
                    case DataModels.Items.WeaponType.Shield:
                        weap.WeaponType = Network.Items.WeaponType.Shield;
                        break;
                    case DataModels.Items.WeaponType.Staff:
                        weap.WeaponType = Network.Items.WeaponType.Staff;
                        break;
                    case DataModels.Items.WeaponType.Sword:
                        weap.WeaponType = Network.Items.WeaponType.Sword;
                        break;
                    case DataModels.Items.WeaponType.Wand:
                        weap.WeaponType = Network.Items.WeaponType.Wand;
                        break;
                    case DataModels.Items.WeaponType.Whip:
                        weap.WeaponType = Network.Items.WeaponType.Whip;
                        break;
                }

                weap.Stats = new List<Network.Items.ItemStat>();
                foreach (var stat in weapon.Stats)
                {
                    var st = new Network.Items.ItemStat
                    {
                        StatValue = stat.StatValue
                    };

                    switch (stat.Type)
                    {
                        case DataModels.Soul.Stats.Agility:
                            st.Type = Network.Stats.Agility;
                            break;
                        case DataModels.Soul.Stats.Energy:
                            st.Type = Network.Stats.Energy;
                            break;
                        case DataModels.Soul.Stats.Intelligence:
                            st.Type = Network.Stats.Intelligence;
                            break;
                        case DataModels.Soul.Stats.Stamina:
                            st.Type = Network.Stats.Stamina;
                            break;
                        case DataModels.Soul.Stats.Strength:
                            st.Type = Network.Stats.Strength;
                            break;
                        case DataModels.Soul.Stats.Wisdom:
                            st.Type = Network.Stats.Wisdom;
                            break;
                    }

                    weap.Stats.Add(st);
                }

                weaponList.Add(weap);
            }

            Weapons = weaponList;
        }

        private void LoadBooks()
        {
            var books = DataRepositories.BookRepository.GetAllForVersion(_config.VersionId);
            var bookList = new List<Network.Skills.Book>();
            foreach (var book in books)
            {
                var bok = new Network.Skills.Book
                {
                    Id = book.BookId,
                    Name = book.Name
                };

                switch (book.Quality)
                {
                    case DataModels.Skills.BookQuality.TierOne:
                        bok.Quality = Network.Skills.BookQuality.TierOne;
                        break;
                    case DataModels.Skills.BookQuality.TierTwo:
                        bok.Quality = Network.Skills.BookQuality.TierTwo;
                        break;
                    case DataModels.Skills.BookQuality.TierThree:
                        bok.Quality = Network.Skills.BookQuality.TierThree;
                        break;
                    case DataModels.Skills.BookQuality.TierFour:
                        bok.Quality = Network.Skills.BookQuality.TierFour;
                        break;
                    case DataModels.Skills.BookQuality.TierFive:
                        bok.Quality = Network.Skills.BookQuality.TierFive;
                        break;
                }

                bok.Pages = new List<Network.Skills.Page>();
                foreach (var page in book.Pages)
                {
                    var pag = new Network.Skills.Page
                    {
                        Id = page.Id,
                        Description = page.Description,
                        Image = page.Image,
                        Rank = page.Rank,
                        EmberCost = page.EmberCost,
                        ManaCost = page.ManaCost,
                        Cooldown = page.Cooldown
                    };

                    pag.Inscriptions = new List<Network.Skills.Inscription>();
                    foreach (var insc in page.Inscriptions)
                    {
                        var ins = new Network.Skills.Inscription
                        {
                            Id = insc.Id,
                            PageId = page.Id,
                            BaseValue = insc.BaseValue,
                            Ratio = insc.Ratio,
                            Duration = insc.Duration,
                            IncludeWeaponDamages = insc.IncludeWeaponDamages,
                            InternalWeaponTypes = insc.InternalWeaponTypes,
                            WeaponDamagesRatio = insc.WeaponDamagesRatio,
                            InternalPreferredWeaponTypes = insc.InternalPreferredWeaponTypes,
                            PreferredWeaponDamagesRatio = insc.PreferredWeaponDamagesRatio
                        };

                        switch (insc.Type)
                        {
                            case DataModels.Skills.InscriptionType.Burn:
                                ins.Type = Network.Skills.InscriptionType.Burn;
                                break;
                            case DataModels.Skills.InscriptionType.Charmed:
                                ins.Type = Network.Skills.InscriptionType.Charmed;
                                break;
                            case DataModels.Skills.InscriptionType.Confused:
                                ins.Type = Network.Skills.InscriptionType.Confused;
                                break;
                            case DataModels.Skills.InscriptionType.Damages:
                                ins.Type = Network.Skills.InscriptionType.Damages;
                                break;
                            case DataModels.Skills.InscriptionType.Frozen:
                                ins.Type = Network.Skills.InscriptionType.Frozen;
                                break;
                            case DataModels.Skills.InscriptionType.Heal:
                                ins.Type = Network.Skills.InscriptionType.Heal;
                                break;
                            case DataModels.Skills.InscriptionType.StatBuff:
                                ins.Type = Network.Skills.InscriptionType.StatBuff;
                                break;
                            case DataModels.Skills.InscriptionType.StatDebuff:
                                ins.Type = Network.Skills.InscriptionType.StatDebuff;
                                break;
                            case DataModels.Skills.InscriptionType.Stunned:
                                ins.Type = Network.Skills.InscriptionType.Stunned;
                                break;
                        }

                        switch (insc.StatType)
                        {
                            case DataModels.Soul.Stats.Agility:
                                ins.StatType = Network.Stats.Agility;
                                break;
                            case DataModels.Soul.Stats.Energy:
                                ins.StatType = Network.Stats.Energy;
                                break;
                            case DataModels.Soul.Stats.Intelligence:
                                ins.StatType = Network.Stats.Intelligence;
                                break;
                            case DataModels.Soul.Stats.Stamina:
                                ins.StatType = Network.Stats.Stamina;
                                break;
                            case DataModels.Soul.Stats.Strength:
                                ins.StatType = Network.Stats.Strength;
                                break;
                            case DataModels.Soul.Stats.Wisdom:
                                ins.StatType = Network.Stats.Wisdom;
                                break;
                        }

                        pag.Inscriptions.Add(ins);
                    }

                    bok.Pages.Add(pag);
                }

                bookList.Add(bok);
            }

            Books = bookList;
        }

        private void LoadDungeons()
        {
            var dungeons = DataRepositories.DungeonRepository.GetAllForVersion(_config.VersionId);
            var dungeonList = new List<Network.Adventures.Dungeon>();

            foreach (var dungeon in dungeons)
            {
                var dun = new Network.Adventures.Dungeon
                {
                    Id = dungeon.DungeonId,
                    Name = dungeon.Name,
                    RequiredLevel = dungeon.RequiredLevel,
                    MaxLevelAuthorized = dungeon.MaxLevelAuthorized,
                    ExperienceReward = dungeon.ExperienceReward,
                    ShardReward = dungeon.ShardReward
                };

                dun.Rooms = new List<Network.Adventures.Room>();
                foreach (var room in dungeon.Rooms)
                {
                    var roo = new Network.Adventures.Room
                    {
                        RoomNumber = room.RoomNumber
                    };

                    roo.ShopItems = new List<Network.Adventures.ShopItem>();
                    foreach (var shopItem in room.ShopItems)
                    {
                        var shopIt = new Network.Adventures.ShopItem
                        {
                            ItemId = shopItem.ItemId,
                            Quantity = shopItem.Quantity,
                            ShardPrice = shopItem.ShardPrice
                        };

                        switch (shopItem.Type)
                        {
                            case DataModels.Items.ItemType.Armor:
                                shopIt.Type = Network.Items.ItemType.Armor;
                                break;
                            case DataModels.Items.ItemType.Bag:
                                shopIt.Type = Network.Items.ItemType.Bag;
                                break;
                            case DataModels.Items.ItemType.Consumable:
                                shopIt.Type = Network.Items.ItemType.Consumable;
                                break;
                            case DataModels.Items.ItemType.Jewelry:
                                shopIt.Type = Network.Items.ItemType.Jewelry;
                                break;
                            case DataModels.Items.ItemType.Weapon:
                                shopIt.Type = Network.Items.ItemType.Weapon;
                                break;
                        }

                        roo.ShopItems.Add(shopIt);
                    }

                    roo.Ennemies = new List<Network.Adventures.Enemy>();
                    foreach (var enemy in room.Ennemies)
                    {
                        var enn = new Network.Adventures.Enemy
                        {
                            MonsterId = enemy.MonsterId,
                            Level = enemy.Level,
                            ShardReward = enemy.ShardReward
                        };

                        switch (enemy.EnemyType)
                        {
                            case DataModels.Dungeons.EnemyType.Normal:
                                enn.EnemyType = Network.Adventures.EnemyType.Normal;
                                break;
                            case DataModels.Dungeons.EnemyType.Elite:
                                enn.EnemyType = Network.Adventures.EnemyType.Elite;
                                break;
                            case DataModels.Dungeons.EnemyType.Boss:
                                enn.EnemyType = Network.Adventures.EnemyType.Boss;
                                break;
                        }

                        roo.Ennemies.Add(enn);
                    }

                    switch (room.Type)
                    {
                        case DataModels.Dungeons.RoomType.Boss:
                            roo.Type = Network.Adventures.RoomType.Boss;
                            break;
                        case DataModels.Dungeons.RoomType.Elite:
                            roo.Type = Network.Adventures.RoomType.Elite;
                            break;
                        case DataModels.Dungeons.RoomType.Exit:
                            roo.Type = Network.Adventures.RoomType.Exit;
                            break;
                        case DataModels.Dungeons.RoomType.Fight:
                            roo.Type = Network.Adventures.RoomType.Fight;
                            break;
                        case DataModels.Dungeons.RoomType.Rest:
                            roo.Type = Network.Adventures.RoomType.Rest;
                            break;
                        case DataModels.Dungeons.RoomType.Shop:
                            roo.Type = Network.Adventures.RoomType.Shop;
                            break;
                    }

                    dun.Rooms.Add(roo);
                }

                dungeonList.Add(dun);
            }

            Dungeons = dungeonList;
        }

        private void LoadMonsters()
        {
            var monsters = DataRepositories.MonsterRepository.GetAllForVersion(_config.VersionId);
            var monsterList = new List<Network.Monsters.Monster>();

            foreach (var monster in monsters)
            {
                var monst = new Network.Monsters.Monster
                {
                    Id = monster.MonsterId,
                    Name = monster.Name,
                    Description = monster.Description,
                    Story = monster.Story,
                    Image = monster.Image,
                    BaseHealth = monster.BaseHealth,
                    BaseExperience = monster.BaseExperience,
                    ExperiencePerLevelRatio = monster.ExperiencePerLevelRatio,
                    HealthPerLevel = monster.HealthPerLevel,
                    AgilityPerLevel = monster.AgilityPerLevel,
                    EnergyPerLevel = monster.EnergyPerLevel,
                    IntelligencePerLevel = monster.IntelligencePerLevel,
                    StaminaPerLevel = monster.StaminaPerLevel,
                    StrengthPerLevel = monster.StrengthPerLevel,
                    WisdomPerLevel = monster.WisdomPerLevel
                };

                var types = new List<Network.Monsters.MonsterType>();
                foreach (var type in monster.Types)
                {
                    switch (type)
                    {
                        case DataModels.Monsters.MonsterType.Animal:
                            types.Add(Network.Monsters.MonsterType.Animal);
                            break;
                        case DataModels.Monsters.MonsterType.Dragon:
                            types.Add(Network.Monsters.MonsterType.Dragon);
                            break;
                        case DataModels.Monsters.MonsterType.Elementary:
                            types.Add(Network.Monsters.MonsterType.Elementary);
                            break;
                        case DataModels.Monsters.MonsterType.Human:
                            types.Add(Network.Monsters.MonsterType.Human);
                            break;
                        case DataModels.Monsters.MonsterType.Undead:
                            types.Add(Network.Monsters.MonsterType.Undead);
                            break;
                    }
                }
                monst.Types = types;

                monst.Loots = new List<Network.Monsters.Loot>();
                foreach (var loot in monster.Loots)
                {
                    var loo = new Network.Monsters.Loot
                    {
                        Id = loot.Id,
                        ItemId = loot.ItemId,
                        DropRate = loot.DropRate,
                        Quantity = loot.Quantity
                    };

                    switch (loot.Type)
                    {
                        case DataModels.Items.ItemType.Armor:
                            loo.Type = Network.Items.ItemType.Armor;
                            break;
                        case DataModels.Items.ItemType.Bag:
                            loo.Type = Network.Items.ItemType.Bag;
                            break;
                        case DataModels.Items.ItemType.Consumable:
                            loo.Type = Network.Items.ItemType.Consumable;
                            break;
                        case DataModels.Items.ItemType.Jewelry:
                            loo.Type = Network.Items.ItemType.Jewelry;
                            break;
                        case DataModels.Items.ItemType.Weapon:
                            loo.Type = Network.Items.ItemType.Weapon;
                            break;
                    }

                    monst.Loots.Add(loo);
                }

                monst.Phases = new List<Network.Monsters.Phase>();
                foreach (var phase in monster.Phases)
                {
                    var pha = new Network.Monsters.Phase
                    {
                        Id = phase.Id,
                        SkillId = phase.SkillId
                    };

                    monst.Phases.Add(pha);
                }

                monsterList.Add(monst);
            }

            Monsters = monsterList;
        }
    }
}
