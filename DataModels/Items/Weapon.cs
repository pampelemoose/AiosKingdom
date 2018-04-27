using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
{
    public enum WeaponType
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

    public class Weapon : AEquipableItem
    {
        [Required(ErrorMessage = "WeaponType required")]
        [Display(Name = "WeaponType")]
        public WeaponType WeaponType { get; set; }

        [Required(ErrorMessage = "MinDamages required")]
        [Display(Name = "MinDamages")]
        [Range(1, 400)]
        public int MinDamages { get; set; }

        [Required(ErrorMessage = "MaxDamages required")]
        [Display(Name = "MaxDamages")]
        [Range(1, 400)]
        public int MaxDamages { get; set; }

        public Weapon()
            : base(ItemType.Weapon)
        {
        }
    }
}
