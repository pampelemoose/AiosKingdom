using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public struct SoulDatas
    {
        public int CurrentLevel { get; set; }
        public int CurrentExperience { get; set; }
        public int RequiredExperience { get; set; }

        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }

        public int CurrentMana { get; set; }
        public int MaxMana { get; set; }

        public int Stamina { get; set; }
        public int Energy { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }

        public int Spirits { get; set; }
        public int Embers { get; set; }
        public int Shards { get; set; }
        public int Bits { get; set; }

        public int TotalStamina { get; set; }
        public int TotalEnergy { get; set; }
        public int TotalStrength { get; set; }
        public int TotalAgility { get; set; }
        public int TotalIntelligence { get; set; }
        public int TotalWisdom { get; set; }

        public int ItemLevel { get; set; }
        public int Armor { get; set; }
    }
}
