﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public struct AdventureState
    {
        public class EnemyState
        {
            public Guid MonsterId { get; set; }
            public string EnemyType { get; set; }
            public PlayerState State { get; set; }
            public int NextPhase { get; set; }
        }

        public struct ShopState
        {
            public Guid ItemId { get; set; }
            public int Quantity { get; set; }
            public int ShardPrice { get; set; }
        }

        public class QuestState
        {
            public Guid QuestId { get; set; }
            public bool Finished { get; set; }
            public List<QuestObjectiveState> Objectives { get; set; }
        }

        public class QuestObjectiveState
        {
            public Guid ObjectiveId { get; set; }
            public int Quantity { get; set; }
            public bool Required { get; set; }
            public bool Finished { get; set; }
        }

        public class SkillCooldown
        {
            public Guid SkillId { get; set; }
            public int Left { get; set; }
        }

        public struct ModifierApplied
        {
            public Guid Id { get; set; }
            public int Left { get; set; }
            public Guid EnemyId { get; set; }
        }

        public class BagItem
        {
            public Guid InventoryId { get; set; }
            public Guid ItemId { get; set; }
            public int Quantity { get; set; }
        }

        public class TavernState
        {
            public Guid TavernId { get; set; }
            public Dictionary<Guid, ShopState> Shops { get; set; }
        }

        public Guid AdventureId { get; set; }
        public string Name { get; set; }

        public Dictionary<Guid, EnemyState> Enemies { get; set; }
        public Dictionary<Guid, TavernState> Taverns { get; set; }

        public List<SkillCooldown> Cooldowns { get; set; }

        public List<ModifierApplied> Marks { get; set; }
        public List<ModifierApplied> Effects { get; set; }

        public List<BagItem> Bag { get; set; }
        public List<QuestState> Quests { get; set; }

        public PlayerState State { get; set; }
        public MovingState MovingState { get; set; }

        public int Shards { get; set; }
    }
}
