using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class MarketSlot
    {
        public Guid Id { get; set; }

        public Items.ItemType Type { get; set; }
        public Guid ItemId { get; set; }

        public Guid? SellerId { get; set; }
        public int Quantity { get; set; }
        public int ShardPrice { get; set; }
        public int BitPrice { get; set; }
    }
}
