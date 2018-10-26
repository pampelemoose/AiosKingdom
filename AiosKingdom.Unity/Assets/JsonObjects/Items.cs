using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects.Items
{
    public enum ItemType
    {
        Consumable = 0,
        Bag = 1,
        Armor = 2,
        Weapon = 3,
        Jewelry = 4
    }

    public enum ItemQuality
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    public enum ArmorPart
    {
        Head = 0,
        Shoulder = 1,
        Torso = 2,
        Belt = 3,
        Pants = 4,
        Leg = 5,
        Feet = 6,
        Hand = 7
    }

    public enum HandlingType
    {
        OneHand = 0,
        TwoHand
    }

    public enum WeaponType
    {
        Fist = 0,
        Dagger,
        Sword,
        Axe,
        Mace,

        Polearm,
        Staff,

        Shield,

        Wand,
        Bow,
        Gun,
        Crossbow,
        Book,
        Whip
    }

    public enum EffectType
    {
        RestoreHealth = 0,
        ResoreMana = 1,

        IncreaseStamina = 10,
        IncreaseEnergy = 11,
        IncreaseStrength = 12,
        IncreaseAgility = 13,
        IncreaseIntelligence = 14,
        IncreaseWisdom = 15
    }

    public class ItemStat
    {
        public Stats Type { get; set; }
        public int StatValue { get; set; }
    }

    public abstract class AItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ItemName
        {
            get { return string.Format("[{0}] {1}", Type, Name); }
        }

        public string Description { get; set; }
        public string Image { get; set; }

        public ItemType Type { get; set; }
        public ItemQuality Quality { get; set; }

        public int ItemLevel { get; set; }
        public int UseLevelRequired { get; set; }
        public int Space { get; set; }

        public int SellingPrice { get; set; }
    }

    public class Armor : AItem
    {
        public ArmorPart Part { get; set; }
        public int ArmorValue { get; set; }

        public List<ItemStat> Stats { get; set; }
    }

    public class Bag : AItem
    {
        public int SlotCount { get; set; }

        public List<ItemStat> Stats { get; set; }
    }

    public class Weapon : AItem
    {
        public HandlingType HandlingType { get; set; }
        public WeaponType WeaponType { get; set; }
        public int MinDamages { get; set; }
        public int MaxDamages { get; set; }

        public List<ItemStat> Stats { get; set; }
    }

    public class Consumable : AItem
    {
        public List<ConsumableEffect> Effects { get; set; }
    }

    public class ConsumableEffect
    {
        public Guid Id { get; set; }
        public Guid ConsumableId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public EffectType Type { get; set; }

        public float AffectValue { get; set; }
        public int AffectTime { get; set; }
    }
}
