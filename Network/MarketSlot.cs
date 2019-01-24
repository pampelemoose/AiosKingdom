using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class MarketSlot
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

        public Guid? SellerId { get; set; }

        public bool IsSpecial { get; set; }

        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
