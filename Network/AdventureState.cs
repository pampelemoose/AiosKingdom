using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public struct AdventureState
    {
        public struct EnemyState
        {
            public Guid MonsterId { get; set; }

            public string EnemyType { get; set; }

            public double CurrentHealth { get; set; }
            public double MaxHealth { get; set; }

            public int NextPhase { get; set; }
        }

        public struct ShopState
        {
            public string Type { get; set; }
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

            public string Type { get; set; }
            public Guid ItemId { get; set; }

            public int Quantity { get; set; }
        }

        public class ActionResult
        {
            public enum Type
            {
                Damage,
                Heal,

                ReceiveMana,
                ConsumedMana,

                EarnExperience,
                EarnShards,
                LevelUp,
                EnemyDeath,
                PlayerDeath
            }

            public Guid TargetId { get; set; }
            public bool IsConsumable { get; set; }
            public Type ResultType { get; set; }
            public Guid Id { get; set; }
            public double Amount { get; set; }
        }

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

        public double CurrentHealth { get; set; }
        public double CurrentMana { get; set; }

        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }

        public int StackedExperience { get; set; }
        public int StackedShards { get; set; }
    }
}
