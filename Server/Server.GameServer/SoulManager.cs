using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private SoulManager()
        {
            _souls = new Dictionary<Guid, DataModels.Soul>();

            _ids = new Dictionary<Guid, Guid>();
            _soulTime = new Dictionary<Guid, DateTime>();
            _soulDatas = new Dictionary<Guid, Network.SoulDatas>();

            _components = new Dictionary<Guid, SoulComponents>();
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
                    Knowledge = new List<Network.Knowledge>()
                };

                foreach (var inventorySlot in soul.Inventory)
                {
                    var slot = new Network.InventorySlot
                    {
                        Id = inventorySlot.Id,
                        ItemId = inventorySlot.ItemId,
                        Quantity = inventorySlot.Quantity,
                        LootedAt = inventorySlot.LootedAt
                    };

                    switch (inventorySlot.Type)
                    {
                        case DataModels.Items.ItemType.Armor:
                            slot.Type = Network.Items.ItemType.Armor;
                            break;
                        case DataModels.Items.ItemType.Bag:
                            slot.Type = Network.Items.ItemType.Bag;
                            break;
                        case DataModels.Items.ItemType.Consumable:
                            slot.Type = Network.Items.ItemType.Consumable;
                            break;
                        case DataModels.Items.ItemType.Jewelry:
                            slot.Type = Network.Items.ItemType.Jewelry;
                            break;
                        case DataModels.Items.ItemType.Weapon:
                            slot.Type = Network.Items.ItemType.Weapon;
                            break;
                    }

                    component.Inventory.Add(slot);
                }

                foreach (var knowledgeSlot in soul.Knowledge)
                {
                    var knowledge = new Network.Knowledge
                    {
                        Id = knowledgeSlot.Id,
                        BookId = knowledgeSlot.BookId,
                        Rank = knowledgeSlot.Rank
                    };

                    component.Knowledge.Add(knowledge);
                }

                _components.Add(token, component);
                return true;
            }

            return false;
        }

        public bool DisconnectSoul(Guid token)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().DisconnectSoul({token})");
            if (_ids.ContainsKey(token))
            {
                var soul = DataRepositories.SoulRepository.GetById(_ids[token]);
                _ids.Remove(token);

                var timePlayed = DateTime.Now - _soulTime[token];
                _soulTime.Remove(token);

                _soulDatas.Remove(token);

                soul.TimePlayed += (float)timePlayed.TotalSeconds;
                DataRepositories.SoulRepository.Update(soul);

                return true;
            }

            return false;
        }

        public void UpdateCurrentDatas(Guid token, DataModels.Config config)
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

        [Obsolete("Do not use Soul anymore, only update db when disconnected or every 5 mins", true)]
        public DataModels.Soul GetSoul(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _souls[token];
            }

            return null;
        }

        public Guid GetSoulId(Guid token)
        {
            if (_ids.ContainsKey(token))
            {
                return _ids[token];
            }

            return Guid.Empty;
        }

        [Obsolete("Do not use Soul anymore, only update db when disconnected or every 5 mins", true)]
        public void UpdateSoul(Guid token, DataModels.Soul soul)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().UpdateSoul({token}, {soul})");
            if (_ids.ContainsKey(token))
            {
                _souls[token] = soul;
            }
        }

        public void UpdateBaseDatas(Guid token, SoulBaseDatas datas)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Datas = datas;

                _components[token] = component;
            }
        }

        public void UpdateCurrencies(Guid token, Network.Currencies currencies)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Currency = currencies;

                _components[token] = component;
            }
        }

        public void UpdateEquipment(Guid token, Network.Equipment equipment)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Equipment = equipment;

                _components[token] = component;
            }
        }

        public void UpdateInventory(Guid token, List<Network.InventorySlot> inventory)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Inventory = inventory;

                _components[token] = component;
            }
        }

        public void UpdateKnowledge(Guid token, List<Network.Knowledge> knowledges)
        {
            if (_ids.ContainsKey(token))
            {
                var component = _components[token];

                component.Knowledge = knowledges;

                _components[token] = component;
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

            if (!Guid.Empty.Equals(equipment.Head))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Head));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Shoulder))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Shoulder));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Torso))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Torso));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Belt))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Belt));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Hand))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Hand));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Leg))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Leg));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Pants))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Pants));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Feet))
            {
                var item = DataManager.Instance.Armors.FirstOrDefault(a => a.Id.Equals(equipment.Feet));

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Bag))
            {
                var item = DataManager.Instance.Bags.FirstOrDefault(a => a.Id.Equals(equipment.Bag));

                data.ItemLevel += item.ItemLevel;
                data.BagSpace = item.SlotCount;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.WeaponRight))
            {
                var item = DataManager.Instance.Weapons.FirstOrDefault(a => a.Id.Equals(equipment.WeaponRight));

                data.ItemLevel += item.ItemLevel;
                data.MinDamages += item.MinDamages;
                data.MaxDamages += item.MaxDamages;
                data.WeaponTypes.Add(item.WeaponType.ToString());
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.WeaponLeft))
            {
                var item = DataManager.Instance.Weapons.FirstOrDefault(a => a.Id.Equals(equipment.WeaponLeft));

                data.ItemLevel += item.ItemLevel;
                data.MinDamages += item.MinDamages;
                data.MaxDamages += item.MaxDamages;
                data.WeaponTypes.Add(item.WeaponType.ToString());
                AddStatValue(data, item.Stats);
            }
        }

        private int GetRequiredExperienceToLevelUp(Guid token, DataModels.Config config)
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

        private int GetMaxHealth(Guid token, DataModels.Config config)
        {
            var soul = _components[token];
            var datas = _soulDatas[token];
            int health = config.BaseHealth;

            health += (int)((soul.Datas.Level - 1) * config.HealthPerLevelRatio);
            health += (int)(datas.TotalStamina * config.HealthPerStaminaRatio);
            return health;
        }

        private int GetMaxMana(Guid token, DataModels.Config config)
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
