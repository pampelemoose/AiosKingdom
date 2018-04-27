using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
{
    public class Bag : AEquipableItem
    {
        [Required]
        public int SlotCount { get; set; }

        public Bag()
            : base(ItemType.Bag)
        {
        }
    }
}
