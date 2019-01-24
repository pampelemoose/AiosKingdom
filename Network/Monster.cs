using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Monsters
{
    public enum MonsterType
    {
        Human = 0,
        Undead,
        Animal,
        Elementary,
        Dragon
    }

    public class Monster
    {
        public Guid Id { get; set; }

        public string InternalTypes { get; set; }
        public List<MonsterType> Types
        {
            get
            {
                var result = new List<MonsterType>();
                if (InternalTypes != null)
                {
                    foreach (var str in InternalTypes.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(str))
                            result.Add((MonsterType)Enum.Parse(typeof(MonsterType), str));
                    }
                }
                return result;
            }
            set
            {
                InternalTypes = String.Join(";", value);
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Story { get; set; }
        public string Image { get; set; }

        public int BaseHealth { get; set; }
        public double HealthPerLevel { get; set; }
        public int BaseExperience { get; set; }
        public double ExperiencePerLevelRatio { get; set; }
        public double StaminaPerLevel { get; set; }
        public double EnergyPerLevel { get; set; }
        public double StrengthPerLevel { get; set; }
        public double AgilityPerLevel { get; set; }
        public double IntelligencePerLevel { get; set; }
        public double WisdomPerLevel { get; set; }

        public List<Loot> Loots { get; set; }
        public List<Phase> Phases { get; set; }
    }

    public class Loot
    {
        public Guid Id { get; set; }

        public double DropRate { get; set; }
        public Items.ItemType Type { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class Phase
    {
        public Guid Id { get; set; }
        public Guid SkillId { get; set; }
    }
}
