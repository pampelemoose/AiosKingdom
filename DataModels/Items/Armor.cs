using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
{
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

    public class Armor : AEquipableItem
    {
        [Required(ErrorMessage = "Part required")]
        [Display(Name = "Part")]
        public ArmorPart Part { get; set; }

        [Required(ErrorMessage = "ArmorValue required")]
        [Display(Name = "Armor Value")]
        [Range(1, 400)]
        public int ArmorValue { get; set; }

        public Armor()
            : base(ItemType.Armor)
        {
        }
    }
}
