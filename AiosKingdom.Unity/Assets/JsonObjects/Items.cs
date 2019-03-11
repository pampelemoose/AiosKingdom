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
        Jewelry = 4,
        CraftingMaterial = 5,
        Enchant = 6,
        Gem = 7,

        Junk = 10,

        Fist = 100,
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

    public enum ItemQuality
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    public enum ItemSlot
    {
        Head = 0,
        Shoulder = 1,
        Torso = 2,
        Belt = 3,
        Pants = 4,
        Leg = 5,
        Feet = 6,
        Hand = 7,

        OneHand = 10,
        TwoHand = 11
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

    public class ItemEffect
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public EffectType Type { get; set; }

        public float AffectValue { get; set; }
        public int AffectTime { get; set; }
    }

    public class Item
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public ItemQuality Quality { get; set; }
        public ItemType Type { get; set; }
        public ItemSlot? Slot { get; set; }

        public int ItemLevel { get; set; }
        public int UseLevelRequired { get; set; }
        public int Space { get; set; }
        public int SellingPrice { get; set; }

        public List<ItemStat> Stats { get; set; }
        public List<ItemEffect> Effects { get; set; }

        public int? ArmorValue { get; set; }
        public int? MagicArmorValue { get; set; }

        public int? SlotCount { get; set; }

        public int? MinDamages { get; set; }
        public int? MaxDamages { get; set; }
    }
}
