using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models.Dungeon
{
    public class ConsumableSelectionItemModel
    {
        public Guid SlotId { get; set; }
        public int Quantity { get; set; }
        public DataModels.Items.Consumable Consumable { get; set; }
    }
}
