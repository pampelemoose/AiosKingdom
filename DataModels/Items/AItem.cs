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

        [Required(ErrorMessage = "Name required"), MinLength(4), MaxLength(50)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description required"), MinLength(4), MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        private string _image = "https://mosaikweb.com/wp-content/plugins/lightbox/images/No-image-found.jpg";
        [Display(Name = "Image")]
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }

        private ItemType _type;
        [Required(ErrorMessage = "Type required")]
        [Display(Name = "Type")]
        public ItemType Type
        {
            get { return _type; }
            private set { _type = value; }
        }

        [Required(ErrorMessage = "Quality required")]
        [Display(Name = "Quality")]
        public ItemQuality Quality { get; set; }

        [Required(ErrorMessage = "ItemLevel required")]
        [Display(Name = "Item Level")]
        [Range(1, 400, ErrorMessage = "ItemLevel should be higher than 0")]
        public int ItemLevel { get; set; }

        [Required(ErrorMessage = "UseLevelRequired required")]
        [Display(Name = "Level Required")]
        [Range(1, 400)]
        public int UseLevelRequired { get; set; }

        public AItem(ItemType type)
        {
            _type = type;
        }
    }
}
