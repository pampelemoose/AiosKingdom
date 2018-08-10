using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class SoulDatas
    {
        public string Name { get; set; }
        public int Level { get; set; }

        public int CurrentExperience { get; set; }
        public int RequiredExperience { get; set; }

        public int MaxHealth { get; set; }
        public int MaxMana { get; set; }

        public int TotalStamina { get; set; }
        public int TotalEnergy { get; set; }
        public int TotalStrength { get; set; }
        public int TotalAgility { get; set; }
        public int TotalIntelligence { get; set; }
        public int TotalWisdom { get; set; }

        public int ItemLevel { get; set; }
        public int Armor { get; set; }

        public List<string> WeaponTypes { get; set; }
        public int MinDamages { get; set; }
        public int MaxDamages { get; set; }

        public int BagSpace { get; set; }
    }
}
