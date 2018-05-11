﻿using System;
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

        public void UpdateCurrentDatas(Guid token, DataModels.GameServer config)
        {
            if (_souls.ContainsKey(token))
            {
                var datas = _soulDatas[token];

                datas.RequiredExperience = GetRequiredExperienceToLevelUp(token, config);
                datas.MaxHealth = GetMaxHealth(token, config);
                datas.MaxMana = GetMaxMana(token, config);
                SetStats(datas, token);

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

        public Network.SoulDatas GetDatas(Guid token)
        {
            if (_soulDatas.ContainsKey(token))
            {
                return _soulDatas[token];
            }

            return null;
        }

        private void SetStats(Network.SoulDatas data, Guid token)
        {
            var soul = _souls[token];

            data.ItemLevel = 0;
            data.Armor = 0;

            data.TotalStamina = soul.Stamina;
            data.TotalEnergy = soul.Energy;
            data.TotalStrength = soul.Strength;
            data.TotalAgility = soul.Agility;
            data.TotalIntelligence = soul.Intelligence;
            data.TotalWisdom = soul.Wisdom;

            var equipment = soul.Equipment;

            if (equipment.Head != null)
            {
                var item = equipment.Head;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Shoulder != null)
            {
                var item = equipment.Shoulder;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Torso != null)
            {
                var item = equipment.Torso;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Belt != null)
            {
                var item = equipment.Belt;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Hand != null)
            {
                var item = equipment.Hand;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Leg != null)
            {
                var item = equipment.Leg;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Pants != null)
            {
                var item = equipment.Pants;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }

            if (equipment.Feet != null)
            {
                var item = equipment.Feet;

                data.ItemLevel += item.ItemLevel;
                data.Armor += item.ArmorValue;
                AddStatValue(data, item.Stats);
            }
        }

        private int GetRequiredExperienceToLevelUp(Guid token, DataModels.GameServer config)
        {
            var soul = _souls[token];
            var datas = _soulDatas[token];

            int calculated = (int)Math.Round((config.BaseExperience * (soul.Level - 1) * config.ExperiencePerLevelRatio));

            if (calculated == 0)
                calculated = config.BaseExperience;

            calculated -= soul.CurrentExperience;

            if (calculated > 0)
                return calculated;

            return 0;
        }

        private int GetMaxHealth(Guid token, DataModels.GameServer config)
        {
            var soul = _souls[token];
            var datas = _soulDatas[token];
            int health = config.BaseHealth;

            health += (int)((soul.Level - 1) * config.HealthPerLevelRatio);
            health += (int)(soul.Stamina * config.HealthPerStaminaRatio);
            return health;
        }

        private int GetMaxMana(Guid token, DataModels.GameServer config)
        {
            var soul = _souls[token];
            var datas = _soulDatas[token];
            int mana = config.BaseMana;

            mana += (int)((soul.Level - 1) * config.ManaPerLevelRatio);
            mana += (int)(soul.Energy * config.ManaPerEnergyRatio);
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
