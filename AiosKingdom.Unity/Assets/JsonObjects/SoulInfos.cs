using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects
{
    public class SoulInfos
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        public int Experience { get; set; }
        public int TotalExperience { get; set; }

        public int Stamina { get; set; }
        public int Energy { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
    }
}
