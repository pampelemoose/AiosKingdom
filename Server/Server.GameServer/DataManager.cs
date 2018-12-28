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

        private DataModels.Town _config;

        public void Initialize(DataModels.Town config)
        {
            _config = config;

            LoadItems();
            LoadBooks();
            LoadAdventures();
            LoadMonsters();
        }

        public List<Network.Items.Item> Items { get; private set; }
        public List<Network.Skills.Book> Books { get; private set; }
        public List<Network.Adventures.Adventure> Dungeons { get; private set; }
        public List<Network.Monsters.Monster> Monsters { get; private set; }
        public List<Network.MarketSlot> Market { get; private set; }

        #region Loaders
        private void LoadItems()
        {
            var items = DataRepositories.ItemRepository.GetAllForVersion(_config.VersionId);
            var itemList = new List<Network.Items.Item>();
            foreach (var item in items)
            {
                var itm = new Network.Items.Item
                {
                    Id = item.Vid,
                    Name = item.Name,
                    Description = item.Description,
                    ItemLevel = item.ItemLevel,
                    UseLevelRequired = item.UseLevelRequired,
                    Space = item.Space,
                    SellingPrice = item.SellingPrice,

                    ArmorValue = item.ArmorValue,
                    MagicArmorValue = item.MagicArmorValue,
                    SlotCount = item.SlotCount,
                    MinDamages = item.MinDamages,
                    MaxDamages = item.MaxDamages,
                };

                switch (item.Quality)
                {
                    case DataModels.Items.ItemQuality.Common:
                        itm.Quality = Network.Items.ItemQuality.Common;
                        break;
                    case DataModels.Items.ItemQuality.Uncommon:
                        itm.Quality = Network.Items.ItemQuality.Uncommon;
                        break;
                    case DataModels.Items.ItemQuality.Rare:
                        itm.Quality = Network.Items.ItemQuality.Rare;
                        break;
                    case DataModels.Items.ItemQuality.Epic:
                        itm.Quality = Network.Items.ItemQuality.Epic;
                        break;
                    case DataModels.Items.ItemQuality.Legendary:
                        itm.Quality = Network.Items.ItemQuality.Legendary;
                        break;
                }

                itm.Type = ConvertItemType(item.Type);

                switch (item.Slot)
                {
                    case DataModels.Items.ItemSlot.Belt:
                        itm.Slot = Network.Items.ItemSlot.Belt;
                        break;
                    case DataModels.Items.ItemSlot.Feet:
                        itm.Slot = Network.Items.ItemSlot.Feet;
                        break;
                    case DataModels.Items.ItemSlot.Hand:
                        itm.Slot = Network.Items.ItemSlot.Hand;
                        break;
                    case DataModels.Items.ItemSlot.Head:
                        itm.Slot = Network.Items.ItemSlot.Head;
                        break;
                    case DataModels.Items.ItemSlot.Leg:
                        itm.Slot = Network.Items.ItemSlot.Leg;
                        break;
                    case DataModels.Items.ItemSlot.Pants:
                        itm.Slot = Network.Items.ItemSlot.Pants;
                        break;
                    case DataModels.Items.ItemSlot.Shoulder:
                        itm.Slot = Network.Items.ItemSlot.Shoulder;
                        break;
                    case DataModels.Items.ItemSlot.Torso:
                        itm.Slot = Network.Items.ItemSlot.Torso;
                        break;
                    case DataModels.Items.ItemSlot.OneHand:
                        itm.Slot = Network.Items.ItemSlot.OneHand;
                        break;
                    case DataModels.Items.ItemSlot.TwoHand:
                        itm.Slot = Network.Items.ItemSlot.TwoHand;
                        break;
                }

                itm.Stats = new List<Network.Items.ItemStat>();
                foreach (var stat in item.Stats)
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

                    itm.Stats.Add(st);
                }

                itm.Effects = new List<Network.Items.ItemEffect>();
                foreach (var effect in item.Effects)
                {
                    var eff = new Network.Items.ItemEffect
                    {
                        Id = effect.Id,
                        ItemId = effect.ItemId,
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

                    itm.Effects.Add(eff);
                }

                itemList.Add(itm);
            }

            Items = itemList;
        }

        private void LoadBooks()
        {
            var books = DataRepositories.BookRepository.GetAllForVersion(_config.VersionId);
            var bookList = new List<Network.Skills.Book>();
            foreach (var book in books)
            {
                var bok = new Network.Skills.Book
                {
                    Id = book.Vid,
                    Name = book.Name,
                    Description = book.Description,
                    EmberCost = book.EmberCost,
                    ManaCost = book.ManaCost,
                    Cooldown = book.Cooldown
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

                bok.Inscriptions = new List<Network.Skills.Inscription>();
                foreach (var insc in book.Inscriptions)
                {
                    var ins = new Network.Skills.Inscription
                    {
                        Id = insc.Id,
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
                        case DataModels.Skills.InscriptionType.Charm:
                            ins.Type = Network.Skills.InscriptionType.Charm;
                            break;
                        case DataModels.Skills.InscriptionType.Confuse:
                            ins.Type = Network.Skills.InscriptionType.Confuse;
                            break;
                        case DataModels.Skills.InscriptionType.PhysicDamages:
                            ins.Type = Network.Skills.InscriptionType.PhysicDamages;
                            break;
                        case DataModels.Skills.InscriptionType.MagicDamages:
                            ins.Type = Network.Skills.InscriptionType.MagicDamages;
                            break;
                        case DataModels.Skills.InscriptionType.Freeze:
                            ins.Type = Network.Skills.InscriptionType.Freeze;
                            break;
                        case DataModels.Skills.InscriptionType.SelfHeal:
                            ins.Type = Network.Skills.InscriptionType.SelfHeal;
                            break;
                        case DataModels.Skills.InscriptionType.TargetHeal:
                            ins.Type = Network.Skills.InscriptionType.TargetHeal;
                            break;
                        case DataModels.Skills.InscriptionType.StatBuff:
                            ins.Type = Network.Skills.InscriptionType.StatBuff;
                            break;
                        case DataModels.Skills.InscriptionType.StatDebuff:
                            ins.Type = Network.Skills.InscriptionType.StatDebuff;
                            break;
                        case DataModels.Skills.InscriptionType.Stunt:
                            ins.Type = Network.Skills.InscriptionType.Stunt;
                            break;
                        case DataModels.Skills.InscriptionType.Multistrike:
                            ins.Type = Network.Skills.InscriptionType.Multistrike;
                            break;
                        case DataModels.Skills.InscriptionType.Unburn:
                            ins.Type = Network.Skills.InscriptionType.Unburn;
                            break;
                        case DataModels.Skills.InscriptionType.Unfreeze:
                            ins.Type = Network.Skills.InscriptionType.Unfreeze;
                            break;
                        case DataModels.Skills.InscriptionType.Uncharm:
                            ins.Type = Network.Skills.InscriptionType.Uncharm;
                            break;
                        case DataModels.Skills.InscriptionType.Unconfuse:
                            ins.Type = Network.Skills.InscriptionType.Unconfuse;
                            break;
                        case DataModels.Skills.InscriptionType.Unstunt:
                            ins.Type = Network.Skills.InscriptionType.Unstunt;
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

                    bok.Inscriptions.Add(ins);
                }

                bok.Talents = new List<Network.Skills.Talent>();
                foreach (var talent in book.Talents)
                {
                    var tal = new Network.Skills.Talent
                    {
                        Id = talent.Id,
                        Branch = talent.Branch,
                        Leaf = talent.Leaf,
                        InternalUnlocks = talent.InternalUnlocks,
                        TargetInscription = talent.TargetInscription,
                        TalentPointsRequired = talent.TalentPointsRequired,
                        Value = talent.Value
                    };

                    switch (talent.Type)
                    {
                        case DataModels.Skills.TalentType.AddAgilityRatio:
                            tal.Type = Network.Skills.TalentType.AddAgilityRatio;
                            break;
                        case DataModels.Skills.TalentType.AddEnergyRatio:
                            tal.Type = Network.Skills.TalentType.AddEnergyRatio;
                            break;
                        case DataModels.Skills.TalentType.AddIntelligenceRatio:
                            tal.Type = Network.Skills.TalentType.AddIntelligenceRatio;
                            break;
                        case DataModels.Skills.TalentType.AddStaminaRatio:
                            tal.Type = Network.Skills.TalentType.AddStaminaRatio;
                            break;
                        case DataModels.Skills.TalentType.AddStrengthRatio:
                            tal.Type = Network.Skills.TalentType.AddStrengthRatio;
                            break;
                        case DataModels.Skills.TalentType.AddWisdomRatio:
                            tal.Type = Network.Skills.TalentType.AddWisdomRatio;
                            break;
                        case DataModels.Skills.TalentType.BaseValue:
                            tal.Type = Network.Skills.TalentType.BaseValue;
                            break;
                        case DataModels.Skills.TalentType.Cooldown:
                            tal.Type = Network.Skills.TalentType.Cooldown;
                            break;
                        case DataModels.Skills.TalentType.Duration:
                            tal.Type = Network.Skills.TalentType.Duration;
                            break;
                        case DataModels.Skills.TalentType.ManaCost:
                            tal.Type = Network.Skills.TalentType.ManaCost;
                            break;
                        case DataModels.Skills.TalentType.Ratio:
                            tal.Type = Network.Skills.TalentType.Ratio;
                            break;
                    }

                    bok.Talents.Add(tal);
                }

                bookList.Add(bok);
            }

            Books = bookList;
        }

        private void LoadAdventures()
        {
            var adventures = DataRepositories.AdventureRepository.GetAllForVersion(_config.VersionId);
            var adventureList = new List<Network.Adventures.Adventure>();

            foreach (var adventure in adventures)
            {
                var adv = new Network.Adventures.Adventure
                {
                    Id = adventure.Vid,
                    Name = adventure.Name,
                    RequiredLevel = adventure.RequiredLevel,
                    MaxLevelAuthorized = adventure.MaxLevelAuthorized,
                    ExperienceReward = adventure.ExperienceReward,
                    ShardReward = adventure.ShardReward
                };

                adv.Rooms = new List<Network.Adventures.Room>();
                foreach (var room in adventure.Rooms)
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
                            ItemId = shopItem.ItemVid,
                            Quantity = shopItem.Quantity,
                            ShardPrice = shopItem.Price
                        };

                        shopIt.Type = ConvertItemType(shopItem.Type);

                        roo.ShopItems.Add(shopIt);
                    }

                    roo.Ennemies = new List<Network.Adventures.Enemy>();
                    foreach (var enemy in room.Ennemies)
                    {
                        var enn = new Network.Adventures.Enemy
                        {
                            MonsterId = enemy.MonsterVid,
                            Level = enemy.Level,
                            ShardReward = enemy.ShardReward
                        };

                        enn.EnemyType = ConvertEnemyType(enemy.EnemyType);

                        roo.Ennemies.Add(enn);
                    }

                    roo.Type = ConvertRoomType(room.Type);

                    adv.Rooms.Add(roo);
                }

                adv.Locks = new List<Network.Adventures.Lock>();
                foreach (var unlock in adventure.Locks)
                {
                    adv.Locks.Add(new Network.Adventures.Lock
                    {
                        LockedId = unlock.LockedId
                    });
                }

                adventureList.Add(adv);
            }

            Dungeons = adventureList;
        }

        private void LoadMonsters()
        {
            var monsters = DataRepositories.MonsterRepository.GetAllForVersion(_config.VersionId);
            var monsterList = new List<Network.Monsters.Monster>();

            foreach (var monster in monsters)
            {
                var monst = new Network.Monsters.Monster
                {
                    Id = monster.Vid,
                    Name = monster.Name,
                    Description = monster.Description,
                    Story = monster.Story,
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
                        ItemId = loot.ItemVid,
                        DropRate = loot.DropRate,
                        Quantity = loot.Quantity
                    };

                    monst.Loots.Add(loo);
                }

                monst.Phases = new List<Network.Monsters.Phase>();
                foreach (var phase in monster.Phases)
                {
                    var pha = new Network.Monsters.Phase
                    {
                        Id = phase.Id,
                        SkillId = phase.BookVid
                    };

                    monst.Phases.Add(pha);
                }

                monsterList.Add(monst);
            }

            Monsters = monsterList;
        }

        private Network.Items.ItemType ConvertItemType(DataModels.Items.ItemType type)
        {
            switch (type)
            {
                case DataModels.Items.ItemType.Armor:
                    return Network.Items.ItemType.Armor;
                case DataModels.Items.ItemType.Axe:
                    return Network.Items.ItemType.Axe;
                case DataModels.Items.ItemType.Bag:
                    return Network.Items.ItemType.Bag;
                case DataModels.Items.ItemType.Book:
                    return Network.Items.ItemType.Book;
                case DataModels.Items.ItemType.Bow:
                    return Network.Items.ItemType.Bow;
                case DataModels.Items.ItemType.Consumable:
                    return Network.Items.ItemType.Consumable;
                case DataModels.Items.ItemType.Crossbow:
                    return Network.Items.ItemType.Crossbow;
                case DataModels.Items.ItemType.Dagger:
                    return Network.Items.ItemType.Dagger;
                case DataModels.Items.ItemType.Fist:
                    return Network.Items.ItemType.Fist;
                case DataModels.Items.ItemType.Gun:
                    return Network.Items.ItemType.Gun;
                case DataModels.Items.ItemType.Jewelry:
                    return Network.Items.ItemType.Jewelry;
                case DataModels.Items.ItemType.Junk:
                    return Network.Items.ItemType.Junk;
                case DataModels.Items.ItemType.Mace:
                    return Network.Items.ItemType.Mace;
                case DataModels.Items.ItemType.Polearm:
                    return Network.Items.ItemType.Polearm;
                case DataModels.Items.ItemType.Shield:
                    return Network.Items.ItemType.Shield;
                case DataModels.Items.ItemType.Staff:
                    return Network.Items.ItemType.Staff;
                case DataModels.Items.ItemType.Sword:
                    return Network.Items.ItemType.Sword;
                case DataModels.Items.ItemType.Wand:
                    return Network.Items.ItemType.Wand;
            }

            return Network.Items.ItemType.Whip;
        }

        private Network.Adventures.EnemyType ConvertEnemyType(DataModels.Adventures.EnemyType type)
        {
            switch (type)
            {
                case DataModels.Adventures.EnemyType.Normal:
                    return Network.Adventures.EnemyType.Normal;
                case DataModels.Adventures.EnemyType.Elite:
                    return Network.Adventures.EnemyType.Elite;
            }

            return Network.Adventures.EnemyType.Boss;
        }

        private Network.Adventures.RoomType ConvertRoomType(DataModels.Adventures.RoomType type)
        {
            switch (type)
            {
                case DataModels.Adventures.RoomType.Boss:
                    return Network.Adventures.RoomType.Boss;
                case DataModels.Adventures.RoomType.Elite:
                    return Network.Adventures.RoomType.Elite;
                case DataModels.Adventures.RoomType.Exit:
                    return Network.Adventures.RoomType.Exit;
                case DataModels.Adventures.RoomType.Fight:
                    return Network.Adventures.RoomType.Fight;
                case DataModels.Adventures.RoomType.Rest:
                    return Network.Adventures.RoomType.Rest;   
            }

            return Network.Adventures.RoomType.Shop;
        }
        #endregion
    }
}
