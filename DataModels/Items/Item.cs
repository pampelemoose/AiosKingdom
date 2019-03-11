using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Items
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

    public class Item : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public ItemQuality Quality { get; set; }

        private ItemType _type;
        public ItemType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        private ItemSlot? _slot;
        public ItemSlot? Slot
        {
            get { return _slot; }
            private set { _slot = value; }
        }
        
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

        public Item()
        {
            _type = ItemType.Junk;
        }

        public Item(ItemType type, ItemSlot? slot)
        {
            _type = type;
            _slot = slot;
        }
    }
}
