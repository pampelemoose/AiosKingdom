using System;
using System.Collections.Generic;
using System.Text;

namespace AiosKingdom.Models
{
    public class MarketItemModel
    {
        public DataModels.MarketSlot Slot { get; set; }
        public DataModels.Items.AItem Item { get; set; }
    }
}
