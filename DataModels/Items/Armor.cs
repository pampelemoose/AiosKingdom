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

    public class Armor : AItem
    {
        [Required]
        public ArmorPart Part { get; set; }

        [Required]
        public int ArmorValue { get; set; }

        public List<ItemStat> Stats { get; set; }

        public Armor()
            : base(ItemType.Armor)
        {
        }
    }
}
