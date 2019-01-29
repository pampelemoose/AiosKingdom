﻿using Newtonsoft.Json;
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
        private struct SkillBuildFrom
        {
            public string Skill { get; set; }
            public string Knowledges { get; set; }
            public string BuildSkill { get; set; }
        }

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

        private Network.Adventures.Adventure _adventure;
        private int _roomNumber;

        private Network.AdventureState _state;
        private Dictionary<Guid, EnemyStats> _enemiesStats;
        private Dictionary<Guid, Network.LootItem> _loots;
        private List<Network.Skills.BuiltInscription> _marks;
        private List<Network.Items.ItemEffect> _effects;
        private Dictionary<Guid, List<Network.Skills.BuiltInscription>> _enemyMarks;

        private List<SkillBuildFrom> _skillBuiltTracking = new List<SkillBuildFrom>();

        public Adventure(Network.Adventures.Adventure adventure,
            Network.SoulDatas datas, List<Network.Knowledge> knowledges,
            List<Network.AdventureState.BagItem> bagItems, int roomNumber = 0)
        {
            _adventure = adventure;
            _roomNumber = roomNumber;

            _state = new Network.AdventureState();

            SetPlayerState(datas, knowledges);

            _state.Bag = bagItems;

            SetState();
        }

        public Guid AdventureId => _adventure.Id;
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

        public bool UseSkill(Network.Skills.BuiltSkill skill, Guid enemyId, out List<Network.ActionResult> message)
        {
            // check if skill is in cooldown list
            if (_state.Cooldowns.FirstOrDefault(c => c.SkillId.Equals(skill.Id)) != null)
            {
                // In Cooldown
                message = new List<Network.ActionResult>();
                return false;
            }

            message = TickTurn(); // TODO : Maybe create a step in between to execute turn related logic

            foreach (var inscription in skill.Inscriptions)
            {
                var res = ExecutePlayerInscription(inscription, enemyId);
                res.Action = skill.Name;
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
                    _state.Cooldowns.Add(new Network.AdventureState.SkillCooldown
                    {
                        SkillId = skill.Id,
                        Left = skill.Cooldown - 1
                    });
                }
            }

            _state.State.CurrentMana -= skill.ManaCost;
            Console.WriteLine($"Consumed {skill.ManaCost} mana.");
            message.Add(new Network.ActionResult
            {
                Id = skill.Id,
                Action = skill.Name,
                ResultType = Network.ActionResult.Type.ConsumedMana,
                Amount = skill.ManaCost
            });

            ClearDeadEnemies();

            return true;
        }

        public bool UseConsumable(Network.AdventureState.BagItem bagItem, Network.Items.Item consumable, Guid enemyId, out List<Network.ActionResult> message)
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
                res.Action = consumable.Name;
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

        public bool EnemyTurn(out List<Network.ActionResult> message)
        {
            message = TickTurn(); // TODO : Maybe create a step in between to execute turn related logic

            var enemies = _enemiesStats.Keys.ToList();
            foreach (var enemyKeys in enemies)
            {
                var enemy = _state.Enemies[enemyKeys];
                var enemyStats = _enemiesStats[enemyKeys];

                var skill = enemy.State.Skills.ElementAt(enemy.NextPhase);

                if (skill == null) return false;

                foreach (var inscription in skill.Inscriptions)
                {
                    var res = ExecuteEnemyInscription(inscription, enemyKeys);
                    res.Action = skill.Name;
                    var inside = message.FirstOrDefault(m => m.IsConsumable == false && m.FromId.Equals(res.FromId) && m.Id.Equals(res.Id));
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

        public bool DoNothingTurn(out List<Network.ActionResult> message)
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
                    var monster = DataManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(enemy.MonsterId));

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
                                MonsterId = enemy.MonsterId,
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
                            Quantity = shopItem.Quantity
                        });
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

        public List<Network.ActionResult> PlayerRest()
        {
            List<Network.ActionResult> messages = new List<Network.ActionResult>();

            int toHeal = (int)_state.State.MaxHealth / 30;

            messages.Add(new Network.ActionResult
            {
                ResultType = Network.ActionResult.Type.SelfHeal,
                Amount = toHeal
            });

            _state.State.CurrentHealth += toHeal;

            if (_state.State.CurrentHealth > _state.State.MaxHealth)
            {
                _state.State.CurrentHealth = _state.State.MaxHealth;
            }

            int toRegen = (int)_state.State.MaxMana / 30;

            messages.Add(new Network.ActionResult
            {
                ResultType = Network.ActionResult.Type.ReceiveMana,
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
                        Quantity = 1
                    });
                }

                --_loots[lootId].Quantity;
                if (_loots[lootId].Quantity <= 0)
                {
                    _loots.Remove(lootId);
                }

                var history = new DataModels.Items.LootHistory
                {
                    LooterId = clientId,
                    AdventureVid = _adventure.Id,
                    MonsterVid = _loots[lootId].MonsterId,
                    ItemVid = _loots[lootId].ItemId,
                    Quantity = _loots[lootId].Quantity,
                    LootedAt = DateTime.Now
                };

                DataRepositories.LootingHistoryRepository.Create(history);

                return true;
            }

            return false;
        }

        public Network.AdventureState GetActualState()
        {
            return _state;
        }

        public void SetPlayerState(Network.SoulDatas datas, List<Network.Knowledge> knowledges)
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

                Armor = datas.Armor,
                MagicArmor = datas.MagicArmor,

                WeaponTypes = datas.WeaponTypes,
                MinDamages = datas.MinDamages,
                MaxDamages = datas.MaxDamages,

                Skills = new List<Network.Skills.BuiltSkill>()
            };

            foreach (var knowledge in knowledges)
            {
                var skill = DataManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(knowledge.BookId));
                var talents = skill.Talents.Where(t => knowledge.Talents.Any(u => u.TalentId.Equals(t.Id)));
                var manaCost = (int)talents.Where(t => t.Type == Network.Skills.TalentType.ManaCost).Sum(t => t.Value);
                var cooldown = (int)talents.Where(t => t.Type == Network.Skills.TalentType.Cooldown).Sum(t => t.Value);
                var built = new Network.Skills.BuiltSkill
                {
                    Id = Guid.NewGuid(),
                    Name = skill.Name,
                    Description = skill.Description,
                    Quality = skill.Quality,
                    ManaCost = skill.ManaCost - manaCost,
                    Cooldown = skill.Cooldown - cooldown,

                    Inscriptions = new List<Network.Skills.BuiltInscription>()
                };

                foreach (var inscription in skill.Inscriptions)
                {
                    var baseValue = (int)talents.Where(t => t.Type == Network.Skills.TalentType.BaseValue).Sum(t => t.Value);
                    var ratio = (int)talents.Where(t => t.Type == Network.Skills.TalentType.Ratio).Sum(t => t.Value);
                    var duration = (int)talents.Where(t => t.Type == Network.Skills.TalentType.Duration).Sum(t => t.Value);
                    var inscBuilt = new Network.Skills.BuiltInscription
                    {
                        Id = Guid.NewGuid(),
                        Type = inscription.Type,
                        BaseMinValue = inscription.BaseValue + baseValue + datas.MinDamages,
                        BaseMaxValue = inscription.BaseValue + baseValue + datas.MaxDamages,
                        StatType = inscription.StatType,
                        Ratio = inscription.Ratio + ratio,
                        Duration = inscription.Duration + duration
                    };

                    if (inscription.IncludeWeaponDamages)
                    {
                        if (inscription.PreferredWeaponTypes.Where(w => datas.WeaponTypes.Contains(w.ToString())).Count() > 0)
                        {
                            inscBuilt.BaseMinValue += (int)(datas.MinDamages * inscription.WeaponDamagesRatio);
                            inscBuilt.BaseMaxValue += (int)(datas.MaxDamages * inscription.WeaponDamagesRatio);
                        }
                        else if (inscription.WeaponTypes.Where(w => datas.WeaponTypes.Contains(w.ToString())).Count() > 0)
                        {
                            inscBuilt.BaseMinValue += (int)(datas.MinDamages * inscription.WeaponDamagesRatio);
                            inscBuilt.BaseMaxValue += (int)(datas.MaxDamages * inscription.WeaponDamagesRatio);
                        }
                    }

                    built.Inscriptions.Add(inscBuilt);
                }

                // TODO : Use this for statistics... Dunno how but still...
                TrackSkillBuild(skill, knowledges, built);

                _state.State.Skills.Add(built);
            }

            _state.StackedExperience = 0;
            _state.StackedShards = 0;
        }

        // This is meant to be used for statistics.. I would like to put in place some sort of tracking in the skills/talents setup for
        // balancing purposes.
        private void TrackSkillBuild(Network.Skills.Book skill, List<Network.Knowledge> knowledges, Network.Skills.BuiltSkill built)
        {
            _skillBuiltTracking.Add(new SkillBuildFrom
            {
                Skill = JsonConvert.SerializeObject(skill),
                Knowledges = JsonConvert.SerializeObject(knowledges),
                BuildSkill = JsonConvert.SerializeObject(built)
            });
        }

        private List<Network.ActionResult> TickTurn()
        {
            var message = TickMarks(); // TODO : is this the right way ?...

            var rand = new Random();

            foreach (var key in _state.Enemies.Keys.ToList())
            {
                var enemy = _state.Enemies[key];
                var monsterPhaseCount = DataManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(enemy.MonsterId)).Phases.Count;

                enemy.NextPhase = rand.Next(monsterPhaseCount);

                _state.Enemies[key] = enemy;
            }

            var newCooldowns = new List<Network.AdventureState.SkillCooldown>();
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

        private List<Network.ActionResult> TickMarks()
        {
            List<Network.ActionResult> result = new List<Network.ActionResult>();

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

            foreach (var eff in _state.Effects)
            {
                var effect = _effects.FirstOrDefault(i => i.Id.Equals(eff.Id));

                var res = ExecutePlayerEffects(effect, eff.EnemyId);
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
                    var inside = result.FirstOrDefault(m => m.IsConsumable == false && m.FromId.Equals(res.FromId) && m.Id.Equals(res.Id));
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

        private Network.ActionResult ExecutePlayerInscription(Network.Skills.BuiltInscription inscription, Guid enemyId = new Guid())
        {
            var enemy = Guid.Empty.Equals(enemyId) ? null : _state.Enemies[enemyId];
            Network.PlayerState state;
            Network.PlayerState enemyNewState;

            var result = SkillAndEffect.ExecuteInscription(_state.State, enemy?.State, inscription, out state, out enemyNewState);

            result.ToId = enemy.MonsterId;

            _state.State = state;
            if (enemy != null)
            {
                enemy.State = enemyNewState;
                _state.Enemies[enemyId] = enemy;
            }

            return result;
        }

        private Network.ActionResult ExecuteEnemyInscription(Network.Skills.BuiltInscription inscription, Guid enemyId)
        {
            var enemy = _state.Enemies[enemyId];
            Network.PlayerState state;
            Network.PlayerState enemyNewState;

            var result = SkillAndEffect.ExecuteInscription(enemy.State, _state.State, inscription, out enemyNewState, out state);

            result.FromId = enemy.MonsterId;

            _state.State = state;
            enemy.State = enemyNewState;
            _state.Enemies[enemyId] = enemy;

            return result;
        }

        private Network.ActionResult ExecutePlayerEffects(Network.Items.ItemEffect effect, Guid enemyId = new Guid())
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
            _state.Name = _adventure.Name;
            _state.CurrentRoom = _roomNumber + 1;
            _state.TotalRoomCount = _adventure.Rooms.Count;

            _enemiesStats = new Dictionary<Guid, EnemyStats>();

            _state.Enemies = new Dictionary<Guid, Network.AdventureState.EnemyState>();
            _state.Shops = new Dictionary<Guid, Network.AdventureState.ShopState>();

            _state.Cooldowns = new List<Network.AdventureState.SkillCooldown>();

            _state.Marks = new List<Network.AdventureState.ModifierApplied>();
            _state.Effects = new List<Network.AdventureState.ModifierApplied>();

            _loots = new Dictionary<Guid, Network.LootItem>();
            _marks = new List<Network.Skills.BuiltInscription>();
            _effects = new List<Network.Items.ItemEffect>();
            _enemyMarks = new Dictionary<Guid, List<Network.Skills.BuiltInscription>>();

            var room = _adventure.Rooms.FirstOrDefault(r => r.RoomNumber == _roomNumber);

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
                var monster = DataManager.Instance.Monsters.FirstOrDefault(m => m.Id.Equals(enemy.MonsterId));
                var state = new Network.AdventureState.EnemyState
                {
                    MonsterId = enemy.MonsterId,
                    EnemyType = enemy.EnemyType.ToString(),
                    State = new Network.PlayerState
                    {
                        MaxHealth = monster.BaseHealth + (enemy.Level * monster.HealthPerLevel),
                        CurrentHealth = monster.BaseHealth + (enemy.Level * monster.HealthPerLevel),

                        Skills = new List<Network.Skills.BuiltSkill>()
                    }
                };

                foreach (var phase in monster.Phases)
                {
                    var phaseSkill = DataManager.Instance.Books.FirstOrDefault(b => b.Id.Equals(phase.SkillId));
                    var built = new Network.Skills.BuiltSkill
                    {
                        Id = Guid.NewGuid(),
                        Name = phaseSkill.Name,
                        Description = phaseSkill.Description,
                        Quality = phaseSkill.Quality,
                        ManaCost = phaseSkill.ManaCost,
                        Cooldown = phaseSkill.Cooldown,

                        Inscriptions = new List<Network.Skills.BuiltInscription>()
                    };

                    foreach (var phaseInscription in phaseSkill.Inscriptions)
                    {
                        var inscBuilt = new Network.Skills.BuiltInscription
                        {
                            Id = Guid.NewGuid(),
                            Type = phaseInscription.Type,
                            BaseMinValue = phaseInscription.BaseValue,
                            BaseMaxValue = phaseInscription.BaseValue,
                            StatType = phaseInscription.StatType,
                            Ratio = phaseInscription.Ratio,
                            Duration = phaseInscription.Duration
                        };
                        built.Inscriptions.Add(inscBuilt);
                    }

                    state.State.Skills.Add(built);
                }

                _state.Enemies.Add(tempId, state);

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
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    ShardPrice = item.ShardPrice
                });
            }

            _state.ExperienceReward = _adventure.ExperienceReward;
            _state.ShardReward = _adventure.ShardReward;
        }
    }
}
