using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects
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

        public class SkillCooldowns
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

        public class ActionResult
        {
            public enum Type
            {
                PhysicDamage,
                MagicDamage,

                SelfHeal,
                TargetHeal,

                ReceiveMana,
                ConsumedMana,

                EarnExperience,
                EarnShards,
                LevelUp,
                EnemyDeath,
                PlayerDeath
            }

            public Guid FromId { get; set; }
            public Guid ToId { get; set; }

            public Type ResultType { get; set; }
            public Guid Id { get; set; }
            public string Action { get; set; }
            public bool IsConsumable { get; set; }

            public double Amount { get; set; }
        }

        public string Name { get; set; }
        public int CurrentRoom { get; set; }
        public int TotalRoomCount { get; set; }

        public Dictionary<Guid, EnemyState> Enemies { get; set; }
        public Dictionary<Guid, ShopState> Shops { get; set; }

        public List<SkillCooldowns> Cooldowns { get; set; }

        public List<ModifierApplied> Marks { get; set; }
        public List<ModifierApplied> Effects { get; set; }

        public List<BagItem> Bag { get; set; }

        public bool IsRestingArea { get; set; }
        public bool IsFightArea { get; set; }
        public bool IsShopArea { get; set; }
        public bool IsEliteArea { get; set; }
        public bool IsBossFight { get; set; }
        public bool IsExit { get; set; }

        public PlayerState State { get; set; }

        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }

        public int StackedExperience { get; set; }
        public int StackedShards { get; set; }
    }
}
