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

        public Adventure(DataModels.Dungeons.Dungeon dungeon, int roomNumber = 0)
        {
            _dungeon = dungeon;
            _roomNumber = roomNumber;

            SetState();
        }

        public Guid DungeonId => _dungeon.DungeonId;
        public int RoomNumber => _roomNumber;

        public bool IsCleared => false;

        public void OpenNextRoom()
        {
            if (IsCleared)
            {
                ++_roomNumber;
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

            _state.IsRestingArea = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Rest;
            _state.IsFightArea = _dungeon.Rooms[_roomNumber].Type == DataModels.Dungeons.RoomType.Fight;
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
                    Stamina = (int)(enemy.Level * monster.StaminaPerLevel),
                    Energy = (int)(enemy.Level * monster.EnergyPerLevel),
                    Strength = (int)(enemy.Level * monster.StrengthPerLevel),
                    Agility = (int)(enemy.Level * monster.AgilityPerLevel),
                    Intelligence = (int)(enemy.Level * monster.IntelligencePerLevel),
                    Wisdom = (int)(enemy.Level * monster.WisdomPerLevel)
                });
            }
        }
    }
}
