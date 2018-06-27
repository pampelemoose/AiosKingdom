using System;
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
            foreach (var inscription in skill.Inscriptions)
            {
                switch (inscription.Type)
                {
                    case DataModels.Skills.InscriptionType.Damages:
                        {
                            if (_enemiesStats.ContainsKey(enemyId))
                            {
                                var enemy = _state.Enemies[enemyId];

                                enemy.CurrentHealth -= inscription.BaseValue + (inscription.Ratio * GetStatValue(inscription.StatType, datas));
                                Console.WriteLine($"Using {skill.BookId}.{skill.Rank} skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetStatValue(inscription.StatType, datas)})*{inscription.Ratio}) on {enemy.MonsterId}.({enemy.CurrentHealth}/{enemy.MaxHealth}) .");

                                _state.Enemies[enemyId] = enemy;
                            }
                            else
                            {
                                return false;
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
                            Console.WriteLine($"Using {skill.BookId}.{skill.Rank} skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetStatValue(inscription.StatType, datas)})*{inscription.Ratio}) on yourself .");
                        }
                        break;
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

                    _enemiesStats.Remove(enemyId);
                    _state.Enemies.Remove(enemyId);
                }
                else
                {
                    _enemiesStats[enemyId] = enemyStats;
                    _state.Enemies[enemyId] = enemy;
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
            /*if (_enemiesStats.ContainsKey(enemyId))
            {
                var enemy = _state.Enemies[enemyId];
                var enemyStats = _enemiesStats[enemyId];*/

            foreach (var effect in consumable.Effects)
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
                            Console.WriteLine($"Using {consumable.ItemId} consumable doing ({effect.Type}).({effect.AffectValue}) on yourself .");
                        }
                        break;
                }
            }

            return true;
        }

        public bool EnemyTurn()
        {
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
                    switch (inscription.Type)
                    {
                        case DataModels.Skills.InscriptionType.Damages:
                            {
                                _state.CurrentHealth -= inscription.BaseValue + (inscription.Ratio * GetEnemyStatValue(inscription.StatType, enemyStats));
                                Console.WriteLine($"Enemy {enemyKeys} using {skill.BookId}.{skill.Rank} skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetEnemyStatValue(inscription.StatType, enemyStats)})*{inscription.Ratio}) on yourself.");
                            }
                            break;
                        case DataModels.Skills.InscriptionType.Heal:
                            {
                                enemy.CurrentHealth += inscription.BaseValue + (inscription.Ratio * GetEnemyStatValue(inscription.StatType, enemyStats));
                                if (enemy.CurrentHealth > enemy.MaxHealth)
                                {
                                    enemy.CurrentHealth = enemy.MaxHealth;
                                }
                                Console.WriteLine($"Enemy {enemyKeys} using {skill.BookId}.{skill.Rank} skill doing ({inscription.Type}).({inscription.BaseValue}+{inscription.StatType}({GetEnemyStatValue(inscription.StatType, enemyStats)})*{inscription.Ratio}) on himself .");

                                _enemiesStats[enemyKeys] = enemyStats;
                                _state.Enemies[enemyKeys] = enemy;
                            }
                            break;
                    }
                }
            }

            return true;
        }

        public bool DoNothingTurn()
        {
            // TODO : Turn time.
            return true;
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
            // TODO : Return networkInfos object containing the actual room datas (enemies hp and so on)
            var rand = new Random();

            foreach (var key in _state.Enemies.Keys.ToList())
            {
                var enemy = _state.Enemies[key];
                var monsterPhaseCount = DataRepositories.MonsterRepository.GetById(enemy.MonsterId).Phases.Count;

                enemy.NextPhase = rand.Next(monsterPhaseCount);

                _state.Enemies[key] = enemy;
            }

            return _state;
        }

        public void SetPlayerState(Network.SoulDatas datas)
        {
            _state.CurrentHealth = datas.MaxHealth;
            _state.CurrentMana = datas.MaxMana;
            _state.StackedExperience = 0;
        }

        private void SetState()
        {
            _state = new Network.AdventureState();
            _enemiesStats = new Dictionary<Guid, EnemyStats>();

            _state.Enemies = new Dictionary<Guid, Network.AdventureState.EnemyState>();
            _state.Shops = new Dictionary<Guid, Network.AdventureState.ShopState>();
            _loots = new Dictionary<Guid, Network.LootItem>();

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

            _state.ExperienceReward = _dungeon.ExperienceReward;
            _state.ShardReward = _dungeon.ShardReward;
        }
    }
}
