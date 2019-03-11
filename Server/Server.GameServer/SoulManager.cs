using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Server.GameServer
{
    public class SoulManager
    {
        public class SoulBaseDatas
        {
            public string Name { get; set; }
            public int Level { get; set; }

            public int CurrentExperience { get; set; }

            public int Stamina { get; set; }
            public int Energy { get; set; }
            public int Strength { get; set; }
            public int Agility { get; set; }
            public int Intelligence { get; set; }
            public int Wisdom { get; set; }
        }

        private struct SoulComponents
        {
            public SoulBaseDatas Datas { get; set; }

            public Network.Currencies Currency { get; set; }
            public Network.Equipment Equipment { get; set; }

            public List<Network.InventorySlot> Inventory { get; set; }
            public List<Network.Knowledge> Knowledge { get; set; }

            public List<Network.AdventureUnlocked> AdventureLocks { get; set; }

            public Network.Job Job { get; set; }
        }

        private static SoulManager _instance;
        public static SoulManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoulManager();
                }

                return _instance;
            }
        }

        private Dictionary<Guid, DataModels.Soul> _souls;

        private Dictionary<Guid, Guid> _ids;
        private Dictionary<Guid, DateTime> _soulTime;
        private Dictionary<Guid, Network.SoulDatas> _soulDatas;

        private Dictionary<Guid, SoulComponents> _components;

        private Timer _dbSaverTimer;

        private Object _componentLock = new object();

        private SoulManager()
        {
            _souls = new Dictionary<Guid, DataModels.Soul>();

            _ids = new Dictionary<Guid, Guid>();
            _soulTime = new Dictionary<Guid, DateTime>();
            _soulDatas = new Dictionary<Guid, Network.SoulDatas>();

            _components = new Dictionary<Guid, SoulComponents>();

            _dbSaverTimer = new Timer(5 * 60 * 1000);
            _dbSaverTimer.AutoReset = true;
            _dbSaverTimer.Enabled = true;
            _dbSaverTimer.Elapsed += (sender, e) =>
            {
                Log.Instance.Write(Log.Level.Infos, "Updating DB from current game state.");
                lock (_componentLock)
                {
                    foreach (var id in _ids)
                    {
                        UpdateDatabase(id.Key);
                    }
                }
            };
        }

        //public bool ConnectSoul(Guid token, DataModels.Soul soul)
        public bool ConnectSoul(Guid token, Guid soulId)
        {
            var soul = DataRepositories.SoulRepository.GetById(soulId);

            Log.Instance.Write(Log.Level.Infos, $"SoulManager().ConnectSoul({token})");
            if (soul != null && !_ids.ContainsKey(token))
            {
                _souls.Add(token, soul);

                _ids.Add(token, soulId);
                _soulTime.Add(token, DateTime.Now);
                _soulDatas.Add(token, new Network.SoulDatas());

                var component = new SoulComponents
                {
                    Datas = new SoulBaseDatas
                    {
                        Name = soul.Name,
                        Level = soul.Level,

                        CurrentExperience = soul.CurrentExperience,

                        Stamina = soul.Stamina,
                        Energy = soul.Energy,
                        Strength = soul.Strength,
                        Agility = soul.Agility,
                        Intelligence = soul.Intelligence,
                        Wisdom = soul.Wisdom,
                    },

                    Currency = new Network.Currencies
                    {
                        Spirits = soul.Spirits,
                        Embers = soul.Embers,
                        Shards = soul.Shards,
                        Bits = soul.Bits
                    },

                    Equipment = new Network.Equipment
                    {
                        Bag = soul.Equipment.Bag,
                        Belt = soul.Equipment.Belt,
                        Feet = soul.Equipment.Feet,
                        Hand = soul.Equipment.Hand,
                        Head = soul.Equipment.Head,
                        Leg = soul.Equipment.Leg,
                        Pants = soul.Equipment.Pants,
                        Shoulder = soul.Equipment.Shoulder,
                        Torso = soul.Equipment.Torso,
                        WeaponLeft = soul.Equipment.WeaponLeft,
                        WeaponRight = soul.Equipment.WeaponRight
                    },

                    Inventory = new List<Network.InventorySlot>(),
                    Knowledge = new List<Network.Knowledge>(),

                    AdventureLocks = new List<Network.AdventureUnlocked>(),

                    Job = null
                };

                foreach (var inventorySlot in soul.Inventory)
                {
                    var slot = new Network.InventorySlot
                    {
                        Id = inventorySlot.Id,
                        IsNew = false,
                        ItemId = inventorySlot.ItemVid,
                        Quantity = inventorySlot.Quantity,
                        LootedAt = inventorySlot.LootedAt
                    };

                    component.Inventory.Add(slot);
                }

                foreach (var knowledgeSlot in soul.Knowledge)
                {
                    var knowledge = new Network.Knowledge
                    {
                        Id = knowledgeSlot.Id,
                        BookId = knowledgeSlot.BookVid,
                        IsNew = false,
                        TalentPoints = knowledgeSlot.TalentPoints,
                        Talents = new List<Network.TalentUnlocked>()
                    };

                    foreach (var talentSlot in knowledgeSlot.Talents)
                    {
                        var talent = new Network.TalentUnlocked
                        {
                            Id = talentSlot.Id,
                            KnowledgeId = talentSlot.KnowledgeId,
                            TalentId = talentSlot.TalentId,
                            IsNew = false,
                            UnlockedAt = talentSlot.UnlockedAt
                        };

                        knowledge.Talents.Add(talent);
                    }

                    component.Knowledge.Add(knowledge);
                }

                foreach (var adventureLock in soul.AdventureLocks)
                {
                    var advLock = new Network.AdventureUnlocked
                    {
                        Id = adventureLock.Id,
                        AdventureId = adventureLock.AdventureVid,
                        UnlockedAt = adventureLock.UnlockedAt
                    };

                    component.AdventureLocks.Add(advLock);
                }

                if (soul.Job != null)
                {
                    var job = new Network.Job
                    {
                        Id = soul.Job.Id,
                        Rank = DataManager.ConvertJobRank(soul.Job.Rank),
                        Type = DataManager.ConvertJobType(soul.Job.Type),
                        Points = soul.Job.Points,
                        Recipes = new List<Network.RecipeUnlocked>()
                    };

                    foreach (var recipeUnlocked in soul.Job.Recipes)
                    {
                        var recUn = new Network.RecipeUnlocked
                        {
                            Id = recipeUnlocked.Id,
                            RecipeId = recipeUnlocked.RecipeId,
                            SoulId = recipeUnlocked.SoulId,
                            UnlockedAt = recipeUnlocked.UnlockedAt,
                            IsNew = false
                        };

                        job.Recipes.Add(recUn);
                    }

                    component.Job = job;
                }

                lock (_componentLock)
                {
                    _components.Add(token, component);
                }
                return true;
            }

            return false;
        }

        public bool DisconnectSoul(Guid token)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().DisconnectSoul({token})");
            if (_ids.ContainsKey(token))
            {
                AdventureManager.Instance.ExitRoom(_ids[token]);

                UpdateDatabase(token);

                _ids.Remove(token);
                _soulTime.Remove(token);
                _soulDatas.Remove(token);
                lock (_componentLock)
                {
                    _components.Remove(token);
                }

                return true;
            }

            return false;
        }

        private void UpdateDatabase(Guid token)
        {
            var soul = DataRepositories.SoulRepository.GetById(_ids[token]);
            var components = _components[token];

            // GENERAL
            soul.Level = components.Datas.Level;
            soul.CurrentExperience = components.Datas.CurrentExperience;
            // STATS
            soul.Stamina = components.Datas.Stamina;
            soul.Energy = components.Datas.Energy;
            soul.Strength = components.Datas.Strength;
            soul.Agility = components.Datas.Agility;
            soul.Intelligence = components.Datas.Intelligence;
            soul.Wisdom = components.Datas.Wisdom;
            // EQUIPMENT
            soul.Equipment.Bag = components.Equipment.Bag;
            soul.Equipment.Belt = components.Equipment.Belt;
            soul.Equipment.Feet = components.Equipment.Feet;
            soul.Equipment.Hand = components.Equipment.Hand;
            soul.Equipment.Head = components.Equipment.Head;
            soul.Equipment.Leg = components.Equipment.Leg;
            soul.Equipment.Pants = components.Equipment.Pants;
            soul.Equipment.Shoulder = components.Equipment.Shoulder;
            soul.Equipment.Torso = components.Equipment.Torso;
            soul.Equipment.WeaponLeft = components.Equipment.WeaponLeft;
            soul.Equipment.WeaponRight = components.Equipment.WeaponRight;
            // INVENTORY
            soul.Inventory = new List<DataModels.InventorySlot>();
            foreach (var inventorySlot in components.Inventory)
            {
                var slot = new DataModels.InventorySlot
                {
                    Id = inventorySlot.IsNew ? Guid.Empty : inventorySlot.Id,
                    ItemVid = inventorySlot.ItemId,
                    Quantity = inventorySlot.Quantity,
                    LootedAt = inventorySlot.LootedAt,
                    SoulId = soul.Id
                };

                soul.Inventory.Add(slot);
            }
            // KNOWLEDGE
            soul.Knowledge = new List<DataModels.Knowledge>();
            foreach (var knowledgeSlot in components.Knowledge)
            {
                var knowledge = new DataModels.Knowledge
                {
                    Id = knowledgeSlot.IsNew ? Guid.Empty : knowledgeSlot.Id,
                    BookVid = knowledgeSlot.BookId,
                    SoulId = soul.Id,
                    TalentPoints = knowledgeSlot.TalentPoints,
                    Talents = new List<DataModels.TalentUnlocked>()
                };

                foreach (var talentSlot in knowledgeSlot.Talents)
                {
                    var talent = new DataModels.TalentUnlocked
                    {
                        Id = talentSlot.IsNew ? Guid.Empty : talentSlot.Id,
                        SoulId = soul.Id,
                        TalentId = talentSlot.TalentId,
                        UnlockedAt = talentSlot.UnlockedAt
                    };

                    knowledge.Talents.Add(talent);
                }

                soul.Knowledge.Add(knowledge);
            }
            // CURRENCIES
            soul.Spirits = components.Currency.Spirits;
            soul.Embers = components.Currency.Embers;
            soul.Shards = components.Currency.Shards;
            soul.Bits = components.Currency.Bits;
            // ADVENTURELOCKS
            soul.AdventureLocks = new List<DataModels.AdventureUnlocked>();
            foreach (var advLockSlot in components.AdventureLocks)
            {
                var advLock = new DataModels.AdventureUnlocked
                {
                    Id = advLockSlot.Id,
                    SoulId = soul.Id,
                    AdventureVid = advLockSlot.AdventureId,
                    UnlockedAt = advLockSlot.UnlockedAt
                };

                soul.AdventureLocks.Add(advLock);
            }

            var timePlayed = DateTime.Now - _soulTime[token];
            soul.TimePlayed += (float)timePlayed.TotalSeconds;

            DataRepositories.SoulRepository.Update(soul);
        }

        public void UpdateCurrentDatas(Guid token, DataModels.Town config)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().UpdateCurrentDatas({token})");
            if (_ids.ContainsKey(token))
            {
                var datas = _soulDatas[token];

                SetStats(datas, token);

                datas.CurrentExperience = _components[token].Datas.CurrentExperience;
                datas.RequiredExperience = GetRequiredExperienceToLevelUp(token, config);
                datas.MaxHealth = GetMaxHealth(token, config);
                datas.MaxMana = GetMaxMana(token, config);

                _soulDatas[token] = datas;
            }
        }

        public Guid GetSoulId(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _ids[token];
            }

            return Guid.Empty;
        }

        public void UpdateBaseDatas(Guid token, SoulBaseDatas datas)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Datas = datas;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public void UpdateCurrencies(Guid token, Network.Currencies currencies)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Currency = currencies;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public void UpdateEquipment(Guid token, Network.Equipment equipment)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Equipment = equipment;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public void UpdateInventory(Guid token, List<Network.InventorySlot> inventory)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Inventory = inventory;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public void UpdateKnowledge(Guid token, List<Network.Knowledge> knowledges)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Knowledge = knowledges;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public void UpdateAdventureLocks(Guid token, List<Network.AdventureUnlocked> locks)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.AdventureLocks = locks;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public void UpdateJob(Guid token, Network.Job job)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Job = job;

                lock (_componentLock)
                {
                    _components[token] = component;
                }
            }
        }

        public Network.SoulDatas GetDatas(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _soulDatas[token];
            }

            return null;
        }

        public SoulBaseDatas GetBaseDatas(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].Datas;
            }

            return null;
        }

        public Network.Currencies GetCurrencies(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].Currency;
            }

            return null;
        }

        public Network.Equipment GetEquipment(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].Equipment;
            }

            return null;
        }

        public List<Network.InventorySlot> GetInventory(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].Inventory;
            }

            return null;
        }

        public List<Network.Knowledge> GetKnowledges(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].Knowledge;
            }

            return null;
        }

        public List<Network.AdventureUnlocked> GetAdventureLocks(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].AdventureLocks;
            }

            return null;
        }

        public Network.Job GetJob(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _components[token].Job;
            }

            return null;
        }

        private void SetStats(Network.SoulDatas data, Guid token)
        {
            var soul = _components[token];

            data.Name = soul.Datas.Name;
            data.Level = soul.Datas.Level;
            data.ItemLevel = 0;
            data.Armor = 0;

            data.TotalStamina = soul.Datas.Stamina;
            data.TotalEnergy = soul.Datas.Energy;
            data.TotalStrength = soul.Datas.Strength;
            data.TotalAgility = soul.Datas.Agility;
            data.TotalIntelligence = soul.Datas.Intelligence;
            data.TotalWisdom = soul.Datas.Wisdom;

            data.MinDamages = 0;
            data.MaxDamages = 0;
            data.WeaponTypes = new List<string>();
            data.BagSpace = 0;

            var equipment = soul.Equipment;
            var equipmentSlots = new List<Network.Items.ItemSlot>
            {
                Network.Items.ItemSlot.Belt,
                Network.Items.ItemSlot.Feet,
                Network.Items.ItemSlot.Hand,
                Network.Items.ItemSlot.Head,
                Network.Items.ItemSlot.Leg,
                Network.Items.ItemSlot.Pants,
                Network.Items.ItemSlot.Torso,
            };

            foreach (var slot in equipmentSlots)
            {
                var armorId = equipment.GetArmorBySlot(slot);
                if (!Guid.Empty.Equals(armorId))
                {
                    var item = DataManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(armorId));

                    data.ItemLevel += item.ItemLevel;
                    data.Armor += item.ArmorValue != null ? (int)item.ArmorValue : 0;
                    AddStatValue(data, item.Stats);
                }
            }

            if (!Guid.Empty.Equals(equipment.Bag))
            {
                var item = DataManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(equipment.Bag));

                data.ItemLevel += item.ItemLevel;
                data.BagSpace = item.SlotCount != null ? (int)item.SlotCount : 0;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.WeaponRight))
            {
                var item = DataManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(equipment.WeaponRight));

                data.ItemLevel += item.ItemLevel;
                data.MinDamages += item.MinDamages != null ? (int)item.MinDamages : 0;
                data.MaxDamages += item.MaxDamages != null ? (int)item.MaxDamages : 0;
                data.WeaponTypes.Add(item.Slot.ToString());
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.WeaponLeft))
            {
                var item = DataManager.Instance.Items.FirstOrDefault(a => a.Id.Equals(equipment.WeaponLeft));

                data.ItemLevel += item.ItemLevel;
                data.MinDamages += item.MinDamages != null ? (int)item.MinDamages : 0;
                data.MaxDamages += item.MaxDamages != null ? (int)item.MaxDamages : 0;
                data.WeaponTypes.Add(item.Slot.ToString());
                AddStatValue(data, item.Stats);
            }
        }

        private int GetRequiredExperienceToLevelUp(Guid token, DataModels.Town config)
        {
            var soul = _components[token];
            var datas = _soulDatas[token];

            int calculated = config.BaseExperience;

            for (int i = 0; i < soul.Datas.Level - 1; ++i)
            {
                calculated += (int)Math.Round(calculated / config.ExperiencePerLevelRatio);
            }

            return calculated;
        }

        private int GetMaxHealth(Guid token, DataModels.Town config)
        {
            var soul = _components[token];
            var datas = _soulDatas[token];
            int health = config.BaseHealth;

            health += (int)((soul.Datas.Level - 1) * config.HealthPerLevelRatio);
            health += (int)(datas.TotalStamina * config.HealthPerStaminaRatio);
            return health;
        }

        private int GetMaxMana(Guid token, DataModels.Town config)
        {
            var soul = _components[token];
            var datas = _soulDatas[token];
            int mana = config.BaseMana;

            mana += (int)((soul.Datas.Level - 1) * config.ManaPerLevelRatio);
            mana += (int)(datas.TotalEnergy * config.ManaPerEnergyRatio);
            return mana;
        }

        private void AddStatValue(Network.SoulDatas data, List<Network.Items.ItemStat> stats)
        {
            foreach (var stat in stats)
            {
                switch (stat.Type)
                {
                    case Network.Stats.Stamina:
                        data.TotalStamina += stat.StatValue;
                        break;
                    case Network.Stats.Energy:
                        data.TotalEnergy += stat.StatValue;
                        break;
                    case Network.Stats.Strength:
                        data.TotalStrength += stat.StatValue;
                        break;
                    case Network.Stats.Agility:
                        data.TotalAgility += stat.StatValue;
                        break;
                    case Network.Stats.Intelligence:
                        data.TotalIntelligence += stat.StatValue;
                        break;
                    case Network.Stats.Wisdom:
                        data.TotalWisdom += stat.StatValue;
                        break;
                }
            }
        }
    }
}
