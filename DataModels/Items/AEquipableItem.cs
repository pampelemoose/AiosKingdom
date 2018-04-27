using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels.Items
{
    public abstract class AEquipableItem : AItem
    {
        public AEquipableItem(ItemType type)
            : base(type)
        {
        }

        [Display(Name = "Stats")]
        public List<ItemStat> Stats { get; set; }
    }
}
