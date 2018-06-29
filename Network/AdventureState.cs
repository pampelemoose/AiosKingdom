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

        public Dictionary<Guid, EnemyState> Enemies { get; set; }
        public Dictionary<Guid, ShopState> Shops { get; set; }

        public bool IsRestingArea { get; set; }
        public bool IsFightArea { get; set; }
        public bool IsShopArea { get; set; }
        public bool IsEliteArea { get; set; }
        public bool IsBossFight { get; set; }

        public double CurrentHealth { get; set; }
        public double CurrentMana { get; set; }

        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }

        public int StackedExperience { get; set; }
        public int StackedShards { get; set; }
    }
}
