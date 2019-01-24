using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models
{
    public class MarketItemModel
    {
        public Network.MarketSlot Slot { get; set; }
        public Network.Items.Item Item { get; set; }
    }
}
