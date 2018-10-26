using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models
{
    public class InventoryItemModel<T> where T : Network.Items.AItem
    {
        public Network.InventorySlot Slot { get; set; }
        public T Item { get; set; }
    }
}
