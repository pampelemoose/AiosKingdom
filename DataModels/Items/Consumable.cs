using System;
using System.Collections.Generic;
using System.Text;

namespace DataModels.Items
{
    public class Consumable : AItem
    {
        public List<ConsumableEffect> Effects { get; set; }

        public Consumable()
            : base(ItemType.Consumable)
        {
        }
    }
}
