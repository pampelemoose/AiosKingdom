﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameServer
{
    public class Adventure
    {
        private struct EnemyStats
        {
            public int Level { get; set; }
            public int ShardReward { get; set; }
            public int Stamina { get; set; }
            public int Energy { get; set; }
            public int Strength { get; set; }
            public int Agility { get; set; }
            public int Intelligence { get; set; }
            public int Wisdom { get; set; }
        }

        private DataModels.Dungeons.Dungeon _dungeon;
        private int _roomNumber;

        private Network.AdventureState _state;
        private Dictionary<Guid, EnemyStats> _enemiesStats;
        private Dictionary<Guid, Network.LootItem> _loots;
        private List<DataModels.Skills.Inscription> _marks;
        private List<DataModels.Items.ConsumableEffect> _effects;
        private Dictionary<Guid, List<DataModels.Skills.Inscription>> _enemyMarks;

        public Adventure(DataModels.Dungeons.Dungeon dungeon, int roomNumber = 0)
        {
            _dungeon = dungeon;
            _roomNumber = roomNumber;

            SetState();
        }

        public Guid DungeonId => _dungeon.DungeonId;
        public int RoomNumber => _roomNumber;

        private bool _isCleared;
        public bool IsCleared => _isCleared;

        public void OpenNextRoom()
        {
            if (IsCleared)
            {
                ++_roomNumber;
            }
        }

        public bool UseSkill(DataModels.Skills.Page skill, Network.SoulDatas datas, Guid enemyId = new Guid())
        {
            // check if skill is in cooldown list
            if (_state.Cooldowns.FirstOrDefault(c => c.SkillId.Equals(skill.Id)) != null)
            {
                // In Cooldown
                return false;
            }

            TickTurn(datas); // TODO : Maybe create a step in between to execute turn related logic

            foreach (var inscription in skill.Inscriptions)
            {
                ExecutePlayerInscription(inscription, datas, enemyId);

                if (inscription.Duration > 0)
                {
                    _state.Marks.Add(new Network.AdventureState.ModifierApplied
                    {
                        Id = inscription.Id,
                        Left = inscription.Duration - 1,
                        EnemyId = enemyId
                    });
                    _marks.Add(inscription);
                }

                if (skill.Cooldown > 0)
                {
                    _state.Cooldowns.Add(new Network.AdventureState.SkillCooldowns
                    {
                        SkillId = skill.Id,
                        Left = skill.Cooldown - 1
                    });
                }
            }

            _state.CurrentMana -= skill.ManaCost;
            Console.WriteLine($"Consumed {skill.ManaCost} mana.");

            var enemies = _enemiesStats.Keys.ToList();
            foreach (var enemyKeys in enemies)
            {
                var enemy = _state.Enemies[enemyKeys];
                var enemyStats = _enemiesStats[enemyKeys];

                if (enemy.CurrentHealth <= 0)
                {
                    var monster = DataRepositories.MonsterRepository.GetById(enemy.MonsterId);

                    _state.StackedExperience += monster.BaseExperience + (int)(enemyStats.Level * monster.ExperiencePerLevelRatio);
                    _state.StackedShards += enemyStats.ShardReward;

                    foreach (var loot in monster.Loots)
                    {
                        var id = Guid.NewGuid();
                        _loots.Add(id, new Network.LootItem
                        {
                            LootId = id,
                            Type = loot.Type.ToString(),
                            ItemId = loot.ItemId,
                            Quantity = loot.Quantity
                        });
                    }

                    _enemiesStats.Remove(enemyKeys);
                    _state.Enemies.Remove(enemyKeys);
                }
                else
                {
                    _enemiesStats[enemyKeys] = enemyStats;
                    _state.Enemies[enemyKeys] = enemy;
                }
            }

            if (_enemiesStats.Count == 0)
            {
                _isCleared = true;
            }

            return true;
        }

        public bool UseConsumable(DataModels.Items.Consumable consumable, Network.SoulDatas datas, Guid enemyId = new Guid())
        {
            TickTurn(datas); // TODO : Maybe create a step in between to execute turn related logic

            /*if (_enemiesStats.ContainsKey(enemyId))
            {
                var enemy = _state.Enemies[enemyId];
                var enemyStats = _enemiesStats[enemyId];*/

            foreach (var effect in consumable.Effects)
            {
                ExecutePlayerEffects(effect, datas);

                if (effect.AffectTime > 0)
                {
                    _state.Effects.Add(new Network.AdventureState.ModifierApplied
                    {
                        Id = effect.Id,
                        Left = effect.AffectTime - 1,
                        EnemyId = enemyId
                    });
                    _effects.Add(effect);
                }
            }

            return true;
        }

        public bool EnemyTurn(Network.SoulDatas datas)
        {
            TickTurn(datas); // TODO : Maybe create a step in between to execute turn related logic

            var enemies = _enemiesStats.Keys.ToList();
            foreach (var enemyKeys in enemies)
            {
                var enemy = _state.Enemies[enemyKeys];
                var enemyStats = _enemiesStats[enemyKeys];

                var phase = DataRepositories.MonsterRepository.GetById(enemy.MonsterId).Phases.ElementAt(enemy.NextPhase);

                if (phase == null) return false;

                var skill = DataRepositories.BookRepository.GetAll().SelectMany(b => b.Pages).FirstOrDefault(p => p.Id.Equals(phase.SkillId));

                if (skill == null) return false;

                foreach (var inscription in skill.Inscriptions)
                {
                    ExecuteEnemyInscription(inscription, enemyKeys);
                }
            }

            return true;
        }

        public bool DoNothingTurn(Network.SoulDatas datas)
        {
            TickTurn(datas); // TODO : Maybe create a step in between to execute turn related logic

            return true;
        }

        public bool BuyShopItem(Guid tempId, int quantity, DataModels.Soul soul)
        {
            if (_state.Shops.ContainsKey(tempId))
            {
                var shopItem = _state.Shops[tempId];

                if (shopItem.Quantity >= quantity && (soul.Shards >= (shopItem.ShardPrice * quantity)))
                {
                    soul.Shards -= (shopItem.ShardPrice * quantity);

                    var type = (DataModels.Items.ItemType)Enum.Parse(typeof(DataModels.Items.ItemType), shopItem.Type);

                    switch (type)
                    {
                        case DataModels.Items.ItemType.Armor:
                        case DataModels.Items.ItemType.Bag:
                        case DataModels.Items.ItemType.Weapon:
                        case DataModels.Items.ItemType.Jewelry:
                            {
                                soul.Inventory.Add(new DataModels.InventorySlot
                                {
                                    ItemId = shopItem.ItemId,
                                    Type = type,
                                    Quantity = quantity,
                                    SoulId = soul.Id,
                                    LootedAt = DateTime.Now
                                });
                                DataRepositories.SoulRepository.Update(soul);
                            }
                            break;
                        case DataModels.Items.ItemType.Consumable:
                            {
                                var exists = soul.Inventory.FirstOrDefault(i => i.ItemId.Equals(shopItem.ItemId));
                                if (exists != null)
                                {
                                    soul.Inventory.Remove(exists);
                                    exists.Quantity += quantity;
                                    soul.Inventory.Add(exists);
                                    DataRepositories.SoulRepository.Update(soul);
                                }
                                else
                                {
                                    soul.Inventory.Add(new DataModels.InventorySlot
                                    {
                                        ItemId = shopItem.ItemId,
                                        Type = type,
                                        Quantity = quantity,
                                        SoulId = soul.Id,
                                        LootedAt = DateTime.Now
                                    });
                                    DataRepositories.SoulRepository.Update(soul);
                                }
                            }
                            break;
                    }

                    if (shopItem.Quantity > 0)
                    {
                        shopItem.Quantity -= quantity;

                        if (shopItem.Quantity == 0)
                        {
                            _state.Shops.Remove(tempId);
                        }
                        else
                        {
                            _state.Shops[tempId] = shopItem;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public List<Network.LootItem> LootRoom()
        {
            return _loots.Values.ToList();
        }

        private int GetStatValue(DataModels.Soul.Stats stat, Network.SoulDatas datas)
        {
            switch (stat)
            {
                case DataModels.Soul.Stats.Stamina:
                    return datas.TotalStamina;
                case DataModels.Soul.Stats.Energy:
                    return datas.TotalEnergy;
                case DataModels.Soul.Stats.Strength:
                    return datas.TotalStrength;
                case DataModels.Soul.Stats.Agility:
                    return datas.TotalAgility;
                case DataModels.Soul.Stats.Intelligence:
                    return datas.TotalIntelligence;
                case DataModels.Soul.Stats.Wisdom:
                    return datas.TotalWisdom;
                default:
                    return 0;
            }
        }

        private int GetEnemyStatValue(DataModels.Soul.Stats stat, EnemyStats stats)
        {
            switch (stat)
            {
                case DataModels.Soul.Stats.Stamina:
                    return stats.Stamina;
                case DataModels.Soul.Stats.Energy:
                    return stats.Energy;
                case DataModels.Soul.Stats.Strength:
                    return stats.Strength;
                case DataModels.Soul.Stats.Agility:
                    return stats.Agility;
                case DataModels.Soul.Stats.Intelligence:
                    return stats.Intelligence;
                case DataModels.Soul.Stats.Wisdom:
                    return stats.Wisdom;
                default:
                    return 0;
            }
        }

        public Network.AdventureState GetActualState()
        {
            return _state;
        }

        public void SetPlayerState(Network.SoulDatas datas)
        {
            _state.CurrentHealth = datas.MaxHealth;
            _state.CurrentMana = datas.MaxMana;
            _state.StackedExperience = 0;
        }

        private void TickTurn(Network.SoulDatas datas)
        {
            TickMarks(datas); // TODO : is this the right way ?...

            var rand = new Random();

            foreach (var key in _state.Enemies.Keys.ToList())
            {
                var enemy = _state.Enemies[key];
                var monsterPhaseCount = DataRepositories.MonsterRepository.GetById(enemy.MonsterId).Phases.Count;

                enemy.NextPhase = rand.Next(monsterPhaseCount);

                _state.Enemies[key] = enemy;
            }

            var newCooldowns = new List<Network.AdventureState.SkillCooldowns>();
            for (int i = 0; i < _state.Cooldowns.Count; ++i)
            {
                var cooldown = _state.Cooldowns.FirstOrDefault();
                _state.Cooldowns.Remove(cooldown);

                --cooldown.Left;
                if (cooldown.Left > 0)
                {
                    newCooldowns.Add(cooldown);
                }
            }
            _state.Cooldowns = newCooldowns;

            var newMarks = new List<Network.AdventureState.ModifierApplied>();
            for (int i = 0; i < _state.Marks.Count; ++i)
            {
                var buff = _state.Marks.FirstOrDefault();
                _state.Marks.Remove(buff);

                --buff.Left;
                if (buff.Left > 0)
                {
                    newMarks.Add(buff);
                }
                else
                {
                    _marks.Remove(_marks.FirstOrDefault(m => m.Id.Equals(buff.Id)));
                }
            }
            _state.Marks = newMarks;

            var newEffects = new List<Network.AdventureState.ModifierApplied>();
            for (int i = 0; i < _state.Effects.Count; ++i)
            {
                var buff = _state.Effects.FirstOrDefault();
                _state.Effects.Remove(buff);

                --buff.Left;
                if (buff.Left > 0)
                {
                    newEffects.Add(buff);
                }
                else
                {
                    _effects.Remove(_effects.FirstOrDefault(m => m.Id.Equals(buff.Id)));
                }
            }
            _state.Effects = newEffects;
        }

        private void TickMarks(Network.SoulDatas datas)
        {
            foreach (var buff in _state.Marks)
            {
                var insc = _marks.FirstOrDefault(i => i.Id.Equals(buff.Id));

                ExecutePlayerInscription(insc, datas, buff.EnemyId);
            }

            foreach (var mod in _state.Effects)
            {
                var effect = _effects.FirstOrDefault(i => i.Id.Equals(mod.Id));

                ExecutePlayerEffects(effect, datas, mod.EnemyId);
            }

            foreach (var enemyMarks in _enemyMarks)
            {
                foreach (var buff in enemyMarks.Value)
                {
                    ExecuteEnemyInscription(buff, enemyMarks.Key);
                }
            }
        }

        private void ExecutePlayerInscription(DataModels.Skills.Inscription inscription, Network.SoulDatas datas, Guid enemyId = new Guid())
        {
            switch (inscription.Type)
            {
                case DataModels.Skills.InscriptionType.Damages:
                    {
                        if (_enemiesStats.ContainsKey(enemyId))
                        {
                            var enemy = _state.Enemies[enemyId];

                            enemy.CurrentHealth -= inscription.BaseValue + (inscription.Ratio * GetStatValue(inscription.StatType, datas));
                            Console.WriteLine($"Using skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetStatValue(inscription.StatType, datas)})*{inscription.Ratio}) on {enemy.MonsterId}.({enemy.CurrentHealth}/{enemy.MaxHealth}) .");

                            _state.Enemies[enemyId] = enemy;
                        }
                        else
                        {
                            throw new Exception($"Could not execute player skill because {enemyId} id doesn't match an enemy.");
                        }
                    }
                    break;
                case DataModels.Skills.InscriptionType.Heal:
                    {
                        _state.CurrentHealth += inscription.BaseValue + (inscription.Ratio * GetStatValue(inscription.StatType, datas));
                        if (_state.CurrentHealth > datas.MaxHealth)
                        {
                            _state.CurrentHealth = datas.MaxHealth;
                        }
                        Console.WriteLine($"Using skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetStatValue(inscription.StatType, datas)})*{inscription.Ratio}) on yourself .");
                    }
                    break;
            }
        }

        private void ExecuteEnemyInscription(DataModels.Skills.Inscription inscription, Guid enemyKeys)
        {
            var enemy = _state.Enemies[enemyKeys];
            var enemyStats = _enemiesStats[enemyKeys];

            switch (inscription.Type)
            {
                case DataModels.Skills.InscriptionType.Damages:
                    {
                        _state.CurrentHealth -= inscription.BaseValue + (inscription.Ratio * GetEnemyStatValue(inscription.StatType, enemyStats));
                        Console.WriteLine($"Enemy {enemyKeys} using skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetEnemyStatValue(inscription.StatType, enemyStats)})*{inscription.Ratio}) on yourself.");
                    }
                    break;
                case DataModels.Skills.InscriptionType.Heal:
                    {
                        enemy.CurrentHealth += inscription.BaseValue + (inscription.Ratio * GetEnemyStatValue(inscription.StatType, enemyStats));
                        if (enemy.CurrentHealth > enemy.MaxHealth)
                        {
                            enemy.CurrentHealth = enemy.MaxHealth;
                        }
                        Console.WriteLine($"Enemy {enemyKeys} using skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetEnemyStatValue(inscription.StatType, enemyStats)})*{inscription.Ratio}) on himself .");

                        _enemiesStats[enemyKeys] = enemyStats;
                        _state.Enemies[enemyKeys] = enemy;
                    }
                    break;
            }
        }

        private void ExecutePlayerEffects(DataModels.Items.ConsumableEffect effect, Network.SoulDatas datas, Guid enemyId = new Guid())
        {
            switch (effect.Type)
            {
                case DataModels.Items.EffectType.RestoreHealth:
                    {
                        _state.CurrentHealth += effect.AffectValue;
                        if (_state.CurrentHealth > datas.MaxHealth)
                        {
                            _state.CurrentHealth = datas.MaxHealth;
                        }
                        Console.WriteLine($"Using consumable doing ({effect.Type}).({effect.AffectValue}) on yourself .");
                    }
                    break;
            }
        }

        private void SetState()
        {
            _state = new Network.AdventureState();
            _enemiesStats = new Dictionary<Guid, EnemyStats>();

            _state.Enemies = new Dictionary<Guid, Network.AdventureState.EnemyState>();
            _state.Shops = new Dictionary<Guid, Network.AdventureState.ShopState>();

            _state.Cooldowns = new List<Network.AdventureState.SkillCooldowns>();

            _state.Marks = new List<Network.AdventureState.ModifierApplied>();
            _state.Effects = new List<Network.AdventureState.ModifierApplied>();

            _loots = new Dictionary<Guid, Network.LootItem>();
            _marks = new List<DataModels.Skills.Inscription>();
            _effects = new List<DataModels.Items.ConsumableEffect>();
            _enemyMarks = new Dictionary<Guid, List<DataModels.Skills.Inscription>>();

            _state.IsRestingArea = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Rest;
            _state.IsFightArea = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Fight
                || _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Elite
                || _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Boss;
            _state.IsShopArea = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Shop;
            _state.IsEliteArea = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Elite;
            _state.IsBossFight = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Boss;

            foreach (var enemy in _dungeon.Rooms[_roomNumber].Ennemies)
            {
                var tempId = Guid.NewGuid();
                var monster = DataRepositories.MonsterRepository.GetById(enemy.MonsterId);

                _state.Enemies.Add(tempId, new Network.AdventureState.EnemyState
                {
                    MonsterId = enemy.MonsterId,
                    MaxHealth = enemy.Level * monster.HealthPerLevel,
                    CurrentHealth = enemy.Level * monster.HealthPerLevel
                });

                _enemiesStats.Add(tempId, new EnemyStats
                {
                    Level = enemy.Level,
                    ShardReward = enemy.ShardReward,
                    Stamina = (int)(enemy.Level * monster.StaminaPerLevel),
                    Energy = (int)(enemy.Level * monster.EnergyPerLevel),
                    Strength = (int)(enemy.Level * monster.StrengthPerLevel),
                    Agility = (int)(enemy.Level * monster.AgilityPerLevel),
                    Intelligence = (int)(enemy.Level * monster.IntelligencePerLevel),
                    Wisdom = (int)(enemy.Level * monster.WisdomPerLevel)
                });
            }

            foreach (var item in _dungeon.Rooms[_roomNumber].ShopItems)
            {
                var tempId = Guid.NewGuid();

                _state.Shops.Add(tempId, new Network.AdventureState.ShopState
                {
                    Type = item.Type.ToString(),
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    ShardPrice = item.ShardPrice
                });
            }

            _state.ExperienceReward = _dungeon.ExperienceReward;
            _state.ShardReward = _dungeon.ShardReward;
        }
    }
}
