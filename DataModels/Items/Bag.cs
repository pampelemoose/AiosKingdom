using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataModels.Items
{
    public class Bag : AItem
    {
        [Required]
        public int SlotCount { get; set; }

        public Bag()
            : base(ItemType.Bag)
        {
        }
    }
}
