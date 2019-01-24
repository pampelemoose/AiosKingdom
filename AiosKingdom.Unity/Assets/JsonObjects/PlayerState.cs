using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects
{
    public class PlayerState
    {
        // MAX
        public double MaxHealth { get; set; }
        public double MaxMana { get; set; }

        // CURRENT
        public double CurrentHealth { get; set; }
        public double CurrentMana { get; set; }

        // STATS
        public int Stamina { get; set; }
        public int Energy { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }

        // DAMAGES
        public List<string> WeaponTypes { get; set; }
        public int MinDamages { get; set; }
        public int MaxDamages { get; set; }

        public List<Skills.BuiltSkill> Skills { get; set; }
    }
}
