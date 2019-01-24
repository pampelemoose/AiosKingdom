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
        public int MagicArmor { get; set; }

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

        public Guid GetArmorBySlot(Items.ItemSlot slot)
        {
            switch (slot)
            {
                case Items.ItemSlot.Belt:
                    return Belt;
                case Items.ItemSlot.Feet:
                    return Feet;
                case Items.ItemSlot.Hand:
                    return Hand;
                case Items.ItemSlot.Head:
                    return Head;
                case Items.ItemSlot.Leg:
                    return Leg;
                case Items.ItemSlot.Pants:
                    return Pants;
                case Items.ItemSlot.Torso:
                    return Torso;
            }

            return Guid.Empty;
        }

        public void SetArmorBySlot(Guid id, Items.ItemSlot slot)
        {
            switch (slot)
            {
                case Items.ItemSlot.Belt:
                    Belt = id;
                    break;
                case Items.ItemSlot.Feet:
                    Feet = id;
                    break;
                case Items.ItemSlot.Hand:
                    Hand = id;
                    break;
                case Items.ItemSlot.Head:
                    Head = id;
                    break;
                case Items.ItemSlot.Leg:
                    Leg = id;
                    break;
                case Items.ItemSlot.Pants:
                    Pants = id;
                    break;
                case Items.ItemSlot.Torso:
                    Torso = id;
                    break;
            }
        }
    }

    public class InventorySlot
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime LootedAt { get; set; }
    }

    public class Knowledge
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public bool IsNew { get; set; }
        public int TalentPoints { get; set; }
        public List<TalentUnlocked> Talents { get; set; }
    }

    public class AdventureUnlocked
    {
        public Guid Id { get; set; }
        public Guid AdventureId { get; set; }
        public DateTime? UnlockedAt { get; set; }
    }

    public class TalentUnlocked
    {
        public Guid Id { get; set; }

        public Guid KnowledgeId { get; set; }
        public Guid TalentId { get; set; }

        public bool IsNew { get; set; }
        public DateTime? UnlockedAt { get; set; }
    }
}
