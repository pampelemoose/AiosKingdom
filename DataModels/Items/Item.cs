using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class Item
    {
        [Key]
        public Guid Id { get; set; }

        public Guid VersionId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quality required")]
        public ItemQuality Quality { get; set; }

        private ItemType _type;
        [Required(ErrorMessage = "Type required")]
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

        [Required(ErrorMessage = "ItemLevel required")]
        [Range(1, 400, ErrorMessage = "ItemLevel should be higher than 0")]
        public int ItemLevel { get; set; }

        [Required(ErrorMessage = "UseLevelRequired required")]
        [Range(1, 400)]
        public int UseLevelRequired { get; set; }

        [Required(ErrorMessage = "Space required")]
        [Range(1, 400)]
        public int Space { get; set; }

        [Required(ErrorMessage = "SellingPrice required")]
        [Range(1, 10000000)]
        public int SellingPrice { get; set; }

        public List<ItemStat> Stats { get; set; }
        public List<ItemEffect> Effects { get; set; }

        [Range(1, 400)]
        public int? ArmorValue { get; set; }

        public int? SlotCount { get; set; }

        [Range(1, 400)]
        public int? MinDamages { get; set; }
        [Range(1, 400)]
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
