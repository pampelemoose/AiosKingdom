using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
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

    public abstract class AItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }

        private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private ItemType _type;
        [Required]
        public ItemType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        [Required]
        public ItemQuality Quality { get; set; }

        [Required]
        public int ItemLevel { get; set; }

        [Required]
        public int UseLevelRequired { get; set; }

        public AItem(ItemType type)
        {
            _type = type;
        }
    }
}
