using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
{
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

    public class Weapon : AEquipableItem
    {
        [Required(ErrorMessage = "HandlingType required")]
        [Display(Name = "HandlingType")]
        public HandlingType HandlingType { get; set; }

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
