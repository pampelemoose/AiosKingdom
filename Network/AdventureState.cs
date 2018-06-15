﻿using System;
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

        }

        public Dictionary<Guid, EnemyState> Enemies { get; set; }
        public Dictionary<Guid, ShopState> Shops { get; set; }

        public bool IsRestingArea { get; set; }
        public bool IsFightArea { get; set; }
        public bool IsShopArea { get; set; }
        public bool IsEliteArea { get; set; }
        public bool IsBossFight { get; set; }

        public int CurrentHealth { get; set; }
        public int CurrentMana { get; set; }

        public int StackedExperience { get; set; }
    }
}