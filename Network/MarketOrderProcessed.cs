using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class MarketOrderProcessed
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
