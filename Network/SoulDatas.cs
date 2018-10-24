using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public enum Stats
    {
        Stamina = 0,
        Energy = 1,
        Strength = 2,
        Agility = 3,
        Intelligence = 4,
        Wisdom = 5
    }

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

    public class Equipment
    {
        public Guid Bag { get; set; }

        public Guid Head { get; set; }
        public Guid Shoulder { get; set; }
        public Guid Torso { get; set; }
        public Guid Belt { get; set; }
        public Guid Pants { get; set; }
        public Guid Leg { get; set; }
        public Guid Feet { get; set; }
        public Guid Hand { get; set; }

        public Guid WeaponRight { get; set; }
        public Guid WeaponLeft { get; set; }
    }

    public class InventorySlot
    {
        public Guid Id { get; set; }

        public Items.ItemType Type { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime LootedAt { get; set; }
    }

    public class Knowledge
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public int Rank { get; set; }
    }
}
