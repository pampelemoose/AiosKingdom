using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class SoulManager
    {
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
        private Dictionary<Guid, DateTime> _soulTime;
        private Dictionary<Guid, Network.SoulDatas> _soulDatas;

        private SoulManager()
        {
            _souls = new Dictionary<Guid, DataModels.Soul>();
            _soulTime = new Dictionary<Guid, DateTime>();
            _soulDatas = new Dictionary<Guid, Network.SoulDatas>();
        }

        public bool ConnectSoul(Guid token, DataModels.Soul soul)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().ConnectSoul({token}, {soul})");
            if (!_souls.ContainsKey(token))
            {
                _souls.Add(token, soul);
                _soulTime.Add(token, DateTime.Now);
                _soulDatas.Add(token, new Network.SoulDatas());
                return true;
            }

            return false;
        }

        public bool DisconnectSoul(Guid token)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().DisconnectSoul({token})");
            if (_souls.ContainsKey(token))
            {
                var soul = _souls[token];
                _souls.Remove(token);

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
            if (_souls.ContainsKey(token))
            {
                var datas = _soulDatas[token];

                SetStats(datas, token);

                datas.CurrentExperience = _souls[token].CurrentExperience;
                datas.RequiredExperience = GetRequiredExperienceToLevelUp(token, config);
                datas.MaxHealth = GetMaxHealth(token, config);
                datas.MaxMana = GetMaxMana(token, config);

                _soulDatas[token] = datas;
            }
        }

        public DataModels.Soul GetSoul(Guid token)
        {
            if (_souls.ContainsKey(token))
            {
                return _souls[token];
            }

            return null;
        }

        public void UpdateSoul(Guid token, DataModels.Soul soul)
        {
            Log.Instance.Write(Log.Level.Infos, $"SoulManager().UpdateSoul({token}, {soul})");
            if (_souls.ContainsKey(token))
            {
                _souls[token] = soul;
            }
        }

        public Network.SoulDatas GetDatas(Guid token)
        {
            if (_soulDatas.ContainsKey(token))
            {
                return _soulDatas[token];
            }

            return null;
        }

        public Network.Currencies GetCurrencies(Guid token)
        {
            if (_souls.ContainsKey(token))
            {
                var soul = _souls[token];
                return new Network.Currencies
                {
                    Spirits = soul.Spirits,
                    Embers = soul.Embers,
                    Shards = soul.Shards,
                    Bits = soul.Bits
                };
            }

            return null;
        }

        private void SetStats(Network.SoulDatas data, Guid token)
        {
            var soul = _souls[token];

            data.Name = soul.Name;
            data.Level = soul.Level;
            data.ItemLevel = 0;
            data.Armor = 0;

            data.TotalStamina = soul.Stamina;
            data.TotalEnergy = soul.Energy;
            data.TotalStrength = soul.Strength;
            data.TotalAgility = soul.Agility;
            data.TotalIntelligence = soul.Intelligence;
            data.TotalWisdom = soul.Wisdom;

            data.MinDamages = 0;
            data.MaxDamages = 0;
            data.WeaponTypes = new List<string>();
            data.BagSpace = 0;

            var equipment = soul.Equipment;

            if (!Guid.Empty.Equals(equipment.Head))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Head);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Shoulder))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Shoulder);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Torso))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Torso);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Belt))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Belt);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Hand))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Hand);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Leg))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Leg);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Pants))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Pants);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Feet))
            {
                var item = DataRepositories.ArmorRepository.GetById(equipment.Feet);

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.Bag))
            {
                var item = DataRepositories.BagRepository.GetById(equipment.Bag);

                data.ItemLevel += item.ItemLevel;
                data.BagSpace = item.SlotCount;
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.WeaponRight))
            {
                var item = DataRepositories.WeaponRepository.GetById(equipment.WeaponRight);

                data.ItemLevel += item.ItemLevel;
                data.MinDamages += item.MinDamages;
                data.MaxDamages += item.MaxDamages;
                data.WeaponTypes.Add(item.WeaponType.ToString());
                AddStatValue(data, item.Stats);
            }

            if (!Guid.Empty.Equals(equipment.WeaponLeft))
            {
                var item = DataRepositories.WeaponRepository.GetById(equipment.WeaponLeft);

                data.ItemLevel += item.ItemLevel;
                data.MinDamages += item.MinDamages;
                data.MaxDamages += item.MaxDamages;
                data.WeaponTypes.Add(item.WeaponType.ToString());
                AddStatValue(data, item.Stats);
            }
        }

        private int GetRequiredExperienceToLevelUp(Guid token, DataModels.Config config)
        {
            var soul = _souls[token];
            var datas = _soulDatas[token];

            int calculated = config.BaseExperience;

            for (int i = 0; i < soul.Level - 1; ++i)
            {
                calculated += (int)Math.Round(calculated / config.ExperiencePerLevelRatio);
            }

            return calculated;
        }

        private int GetMaxHealth(Guid token, DataModels.Config config)
        {
            var soul = _souls[token];
            var datas = _soulDatas[token];
            int health = config.BaseHealth;

            health += (int)((soul.Level - 1) * config.HealthPerLevelRatio);
            health += (int)(datas.TotalStamina * config.HealthPerStaminaRatio);
            return health;
        }

        private int GetMaxMana(Guid token, DataModels.Config config)
        {
            var soul = _souls[token];
            var datas = _soulDatas[token];
            int mana = config.BaseMana;

            mana += (int)((soul.Level - 1) * config.ManaPerLevelRatio);
            mana += (int)(datas.TotalEnergy * config.ManaPerEnergyRatio);
            return mana;
        }

        private void AddStatValue(Network.SoulDatas data, List<DataModels.Items.ItemStat> stats)
        {
            foreach (var stat in stats)
            {
                switch (stat.Type)
                {
                    case DataModels.Soul.Stats.Stamina:
                        data.TotalStamina += stat.StatValue;
                        break;
                    case DataModels.Soul.Stats.Energy:
                        data.TotalEnergy += stat.StatValue;
                        break;
                    case DataModels.Soul.Stats.Strength:
                        data.TotalStrength += stat.StatValue;
                        break;
                    case DataModels.Soul.Stats.Agility:
                        data.TotalAgility += stat.StatValue;
                        break;
                    case DataModels.Soul.Stats.Intelligence:
                        data.TotalIntelligence += stat.StatValue;
                        break;
                    case DataModels.Soul.Stats.Wisdom:
                        data.TotalWisdom += stat.StatValue;
                        break;
                }
            }
        }
    }
}
