using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models
{
    public class InventoryItemModel : Network.Items.Item
    {
        public Network.InventorySlot Slot { get; set; }
        public Network.Items.Item Item { get; set; }
    }
}
