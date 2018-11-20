using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        private Network.Adventures.Dungeon _dungeon;
        private int _roomNumber;

        private Network.AdventureState _state;
        private Dictionary<Guid, EnemyStats> _enemiesStats;
        private Dictionary<Guid, Network.LootItem> _loots;
        private List<Network.Skills.Inscription> _marks;
        private List<Network.Items.ItemEffect> _effects;
        private Dictionary<Guid, List<Network.Skills.Inscription>> _enemyMarks;

        public Adventure(Network.Adventures.Dungeon dungeon, Network.SoulDatas datas, List<Network.AdventureState.BagItem> bagItems, int roomNumber = 0)
        {
            _dungeon = dungeon;
            _roomNumber = roomNumber;

            _state = new Network.AdventureState();

            SetPlayerState(datas);

            _state.Bag = bagItems;

            SetState();
        }

        public Guid DungeonId => _dungeon.Id;
        public int RoomNumber => _roomNumber;

        private bool _isCleared;
        public bool IsCleared => _isCleared;

        public void OpenNextRoom()
        {
            if (IsCleared)
            {
                ++_roomNumber;
                SetState();
            }
        }

        public bool UseSkill(Network.Skills.Page skill, Guid enemyId, out List<Network.AdventureState.ActionResult> message)
        {
            // check if skill is in cooldown list
            if (_state.Cooldowns.FirstOrDefault(c => c.SkillId.Equals(skill.Id)) != null)
            {
                // In Cooldown
                message = new List<Network.AdventureState.ActionResult>();
                return false;
            }

            message = TickTurn(); // TODO : Maybe create a step in between to execute turn related logic

            foreach (var inscription in skill.Inscriptions)
            {
                var res = ExecutePlayerInscription(inscription, enemyId);
                var inside = message.FirstOrDefault(m => m.IsConsumable == false && m.Id.Equals(res.Id));
                if (inside != null)
                {
                    message.Remove(inside);
                    res.Amount += inside.Amount;
                }
                message.Add(res);

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

            _state.State.CurrentMana -= skill.ManaCost;
            Console.WriteLine($"Consumed {skill.ManaCost} mana.");
            message.Add(new Network.AdventureState.ActionResult
            {
                Id = skill.Id,
                ResultType = Network.AdventureState.ActionResult.Type.ConsumedMana,
                Amount = skill.ManaCost
            });

            ClearDeadEnemies();

            return true;
        }

        public bool UseConsumable(Network.AdventureState.BagItem bagItem, Network.Items.Item consumable, Guid enemyId, out List<Network.AdventureState.ActionResult> message)
        {
            message = TickTurn(); // TODO : Maybe create a step in between to execute turn related logic

            _state.Bag.Remove(bagItem);
            bagItem.Quantity--;

            if (bagItem.Quantity > 0)
            {
                _state.Bag.Add(bagItem);
            }

            foreach (var effect in consumable.Effects)
            {
                var res = ExecutePlayerEffects(effect);
                var inside = message.FirstOrDefault(m => m.IsConsumable == false && m.Id.Equals(res.Id));
                if (inside != null)
                {
                    message.Remove(inside);
                    res.Amount += inside.Amount;
                }
                message.Add(res);

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

            ClearDeadEnemies();

            return true;
        }

        public bool EnemyTurn(out List<Network.AdventureState.ActionResult> message)
        {
            message = TickTurn(); // TODO : Maybe create a step in between to execute turn related logic

            var enemies = _enemiesStats.Keys.ToList();
            foreach (var enemyKeys in enemies)
            {
                var enemy = _state.Enemies[enemyKeys];
                var enemyStats = _enemiesStats[enemyKeys];

                var phase = DataRepositories.MonsterRepository.GetById(enemy.MonsterId).Phases.ElementAt(enemy.NextPhase);

                if (phase == null) return false;

                var skill = DataManager.Instance.Books.SelectMany(b => b.Pages).FirstOrDefault(p => p.Id.Equals(phase.SkillId));

                if (skill == null) return false;

                foreach (var inscription in skill.Inscriptions)
                {
                    var res = ExecuteEnemyInscription(inscription, enemyKeys);
                    var inside = message.FirstOrDefault(m => m.IsConsumable == false && m.TargetId.Equals(res.TargetId) && m.Id.Equals(res.Id));
                    if (inside != null)
                    {
                        message.Remove(inside);
                        res.Amount += inside.Amount;
                    }
                    message.Add(res);
                }
            }

            ClearDeadEnemies();

            return true;
        }

        public bool DoNothingTurn(out List<Network.AdventureState.ActionResult> message)
        {
            message = TickTurn(); // TODO : Maybe create a step in between to execute turn related logic

            ClearDeadEnemies();

            return true;
        }

        private void ClearDeadEnemies()
        {
            var enemies = _enemiesStats.Keys.ToList();
            foreach (var enemyKeys in enemies)
            {
                var enemy = _state.Enemies[enemyKeys];
                var enemyStats = _enemiesStats[enemyKeys];

                if (enemy.State.CurrentHealth <= 0)
                {
                    var monster = DataRepositories.MonsterRepository.GetById(enemy.MonsterId);

                    _state.StackedExperience += monster.BaseExperience + (int)(enemyStats.Level * monster.ExperiencePerLevelRatio);
                    _state.StackedShards += enemyStats.ShardReward;

                    foreach (var loot in monster.Loots)
                    {
                        var lootExists = _loots.Values.FirstOrDefault(l => l.ItemId.Equals(loot.ItemId));
                        if (lootExists != null)
                        {
                            lootExists.Quantity += loot.Quantity;
                            _loots[lootExists.LootId] = lootExists;
                        }
                        else
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
                    }

                    _enemiesStats.Remove(enemyKeys);
                    _state.Enemies.Remove(enemyKeys);
                    _state.Marks.RemoveAll(m => m.EnemyId.Equals(enemyKeys));
                    _state.Effects.RemoveAll(m => m.EnemyId.Equals(enemyKeys));
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
        }

        public bool BuyShopItem(Guid tempId, int quantity, Guid soulId, Guid clientId)
        {
            var currencies = SoulManager.Instance.GetCurrencies(clientId);

            if (_state.Shops.ContainsKey(tempId))
            {
                var shopItem = _state.Shops[tempId];

                if (shopItem.Quantity >= quantity && (currencies.Shards >= (shopItem.ShardPrice * quantity)))
                {
                    currencies.Shards -= (shopItem.ShardPrice * quantity);

                    var type = (Network.Items.ItemType)Enum.Parse(typeof(Network.Items.ItemType), shopItem.Type);

                    switch (type)
                    {
                        case Network.Items.ItemType.Armor:
                        case Network.Items.ItemType.Bag:
                        case Network.Items.ItemType.Axe:
                        case Network.Items.ItemType.Book:
                        case Network.Items.ItemType.Bow:
                        case Network.Items.ItemType.Crossbow:
                        case Network.Items.ItemType.Dagger:
                        case Network.Items.ItemType.Fist:
                        case Network.Items.ItemType.Gun:
                        case Network.Items.ItemType.Mace:
                        case Network.Items.ItemType.Polearm:
                        case Network.Items.ItemType.Shield:
                        case Network.Items.ItemType.Staff:
                        case Network.Items.ItemType.Sword:
                        case Network.Items.ItemType.Wand:
                        case Network.Items.ItemType.Whip:
                        case Network.Items.ItemType.Jewelry:
                            {
                                _state.Bag.Add(new Network.AdventureState.BagItem
                                {
                                    ItemId = shopItem.ItemId,
                                    Type = shopItem.Type,
                                    Quantity = shopItem.Quantity
                                });
                            }
                            break;
                        case Network.Items.ItemType.Junk:
                        case Network.Items.ItemType.Consumable:
                            {
                                var exists = _state.Bag.FirstOrDefault(b => b.ItemId.Equals(shopItem.ItemId));
                                if (exists != null)
                                {
                                    _state.Bag.Remove(exists);
                                    exists.Quantity += shopItem.Quantity;
                                    _state.Bag.Add(exists);
                                }
                                else
                                {
                                    _state.Bag.Add(new Network.AdventureState.BagItem
                                    {
                                        ItemId = shopItem.ItemId,
                                        Type = shopItem.Type,
                                        Quantity = shopItem.Quantity
                                    });
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

        public List<Network.AdventureState.ActionResult> PlayerRest()
        {
            List<Network.AdventureState.ActionResult> messages = new List<Network.AdventureState.ActionResult>();

            int toHeal = (int)_state.State.MaxHealth / 30;

            messages.Add(new Network.AdventureState.ActionResult
            {
                ResultType = Network.AdventureState.ActionResult.Type.Heal,
                Amount = toHeal
            });

            _state.State.CurrentHealth += toHeal;

            if (_state.State.CurrentHealth > _state.State.MaxHealth)
            {
                _state.State.CurrentHealth = _state.State.MaxHealth;
            }

            int toRegen = (int)_state.State.MaxMana / 30;

            messages.Add(new Network.AdventureState.ActionResult
            {
                ResultType = Network.AdventureState.ActionResult.Type.ReceiveMana,
                Amount = toRegen
            });

            _state.State.CurrentMana += toRegen;

            if (_state.State.CurrentMana > _state.State.MaxMana)
            {
                _state.State.CurrentMana = _state.State.MaxMana;
            }

            return messages;
        }

        public List<Network.LootItem> GetLoots()
        {
            return _loots.Values.ToList();
        }

        public bool LootItem(Guid clientId, Guid lootId)
        {
            if (_loots.ContainsKey(lootId))
            {
                Network.Items.ItemType type = (Network.Items.ItemType)Enum.Parse(typeof(Network.Items.ItemType), _loots[lootId].Type);
                var exists = _state.Bag.FirstOrDefault(i => i.ItemId.Equals(_loots[lootId].ItemId));
                if (exists != null)
                {
                    _state.Bag.Remove(exists);
                    ++exists.Quantity;
                    _state.Bag.Add(exists);
                }
                else
                {
                    _state.Bag.Add(new Network.AdventureState.BagItem
                    {
                        InventoryId = Guid.NewGuid(),
                        ItemId = _loots[lootId].ItemId,
                        Type = type.ToString(),
                        Quantity = 1
                    });
                }

                --_loots[lootId].Quantity;
                if (_loots[lootId].Quantity <= 0)
                {
                    _loots.Remove(lootId);
                }

                return true;
            }

            return false;
        }

        public Network.AdventureState GetActualState()
        {
            return _state;
        }

        public void SetPlayerState(Network.SoulDatas datas)
        {
            _state.State = new Network.PlayerState
            {
                MaxHealth = datas.MaxHealth,
                MaxMana = datas.MaxMana,

                CurrentHealth = datas.MaxHealth,
                CurrentMana = datas.MaxMana,

                Stamina = datas.TotalStamina,
                Energy = datas.TotalEnergy,
                Strength = datas.TotalStrength,
                Agility = datas.TotalAgility,
                Intelligence = datas.TotalIntelligence,
                Wisdom = datas.TotalWisdom,

                WeaponTypes = datas.WeaponTypes,
                MinDamages = datas.MinDamages,
                MaxDamages = datas.MaxDamages
            };

            _state.StackedExperience = 0;
            _state.StackedShards = 0;
        }

        private List<Network.AdventureState.ActionResult> TickTurn()
        {
            var message = TickMarks(); // TODO : is this the right way ?...

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

            return message;
        }

        private List<Network.AdventureState.ActionResult> TickMarks()
        {
            List<Network.AdventureState.ActionResult> result = new List<Network.AdventureState.ActionResult>();

            foreach (var buff in _state.Marks)
            {
                var insc = _marks.FirstOrDefault(i => i.Id.Equals(buff.Id));

                var res = ExecutePlayerInscription(insc, buff.EnemyId);
                var inside = result.FirstOrDefault(m => m.IsConsumable == false && m.Id.Equals(res.Id));
                if (inside != null)
                {
                    result.Remove(inside);
                    res.Amount += inside.Amount;
                }
                result.Add(res);
            }

            foreach (var mod in _state.Effects)
            {
                var effect = _effects.FirstOrDefault(i => i.Id.Equals(mod.Id));

                var res = ExecutePlayerEffects(effect, mod.EnemyId);
                var inside = result.FirstOrDefault(m => m.IsConsumable == false && m.Id.Equals(res.Id));
                if (inside != null)
                {
                    result.Remove(inside);
                    res.Amount += inside.Amount;
                }
                result.Add(res);
            }

            foreach (var enemyMarks in _enemyMarks)
            {
                foreach (var buff in enemyMarks.Value)
                {
                    var res = ExecuteEnemyInscription(buff, enemyMarks.Key);
                    var inside = result.FirstOrDefault(m => m.IsConsumable == false && m.TargetId.Equals(res.TargetId) && m.Id.Equals(res.Id));
                    if (inside != null)
                    {
                        result.Remove(inside);
                        res.Amount += inside.Amount;
                    }
                    result.Add(res);
                }
            }

            return result;
        }

        private Network.AdventureState.ActionResult ExecutePlayerInscription(Network.Skills.Inscription inscription, Guid enemyId = new Guid())
        {
            var enemy = Guid.Empty.Equals(enemyId) ? null : _state.Enemies[enemyId];
            Network.PlayerState state;
            Network.PlayerState enemyNewState;

            var result = SkillAndEffect.ExecuteInscription(_state.State, enemy?.State, inscription, out state, out enemyNewState);

            result.TargetId = enemy.MonsterId;

            _state.State = state;
            if (enemy != null)
            {
                enemy.State = enemyNewState;
                _state.Enemies[enemyId] = enemy;
            }

            return result;
        }

        private Network.AdventureState.ActionResult ExecuteEnemyInscription(Network.Skills.Inscription inscription, Guid enemyId)
        {
            var enemy = _state.Enemies[enemyId];
            Network.PlayerState state;
            Network.PlayerState enemyNewState;

            var result = SkillAndEffect.ExecuteInscription(enemy.State, _state.State, inscription, out enemyNewState, out state);

            _state.State = state;
            enemy.State = enemyNewState;
            _state.Enemies[enemyId] = enemy;

            return result;
        }

        private Network.AdventureState.ActionResult ExecutePlayerEffects(Network.Items.ItemEffect effect, Guid enemyId = new Guid())
        {
            var enemy = Guid.Empty.Equals(enemyId) ? null : _state.Enemies[enemyId];
            Network.PlayerState state;
            Network.PlayerState enemyNewState;

            var result = SkillAndEffect.ExecuteEffects(_state.State, enemy?.State, effect, out state, out enemyNewState);

            _state.State = state;
            if (enemy != null)
            {
                enemy.State = enemyNewState;
                _state.Enemies[enemyId] = enemy;
            }

            return result;
        }

        private void SetState()
        {
            _enemiesStats = new Dictionary<Guid, EnemyStats>();

            _state.Enemies = new Dictionary<Guid, Network.AdventureState.EnemyState>();
            _state.Shops = new Dictionary<Guid, Network.AdventureState.ShopState>();

            _state.Cooldowns = new List<Network.AdventureState.SkillCooldowns>();

            _state.Marks = new List<Network.AdventureState.ModifierApplied>();
            _state.Effects = new List<Network.AdventureState.ModifierApplied>();

            _loots = new Dictionary<Guid, Network.LootItem>();
            _marks = new List<Network.Skills.Inscription>();
            _effects = new List<Network.Items.ItemEffect>();
            _enemyMarks = new Dictionary<Guid, List<Network.Skills.Inscription>>();

            var room = _dungeon.Rooms.FirstOrDefault(r => r.RoomNumber == _roomNumber);

            _state.IsRestingArea = room.Type == Network.Adventures.RoomType.Rest;
            _state.IsFightArea = room.Type == Network.Adventures.RoomType.Fight
                || room.Type == Network.Adventures.RoomType.Elite
                || room.Type == Network.Adventures.RoomType.Boss;
            _state.IsShopArea = room.Type == Network.Adventures.RoomType.Shop;
            _state.IsEliteArea = room.Type == Network.Adventures.RoomType.Elite;
            _state.IsBossFight = room.Type == Network.Adventures.RoomType.Boss;
            _state.IsExit = room.Type == Network.Adventures.RoomType.Exit;

            foreach (var enemy in room.Ennemies)
            {
                var tempId = Guid.NewGuid();
                var monster = DataRepositories.MonsterRepository.GetById(enemy.MonsterId);

                _state.Enemies.Add(tempId, new Network.AdventureState.EnemyState
                {
                    MonsterId = enemy.MonsterId,
                    EnemyType = enemy.EnemyType.ToString(),
                    State = new Network.PlayerState
                    {
                        MaxHealth = monster.BaseHealth + (enemy.Level * monster.HealthPerLevel),
                        CurrentHealth = monster.BaseHealth + (enemy.Level * monster.HealthPerLevel)
                    }
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

            foreach (var item in room.ShopItems)
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
