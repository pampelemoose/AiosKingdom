using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public enum TransactionType
    {
        Buy,
        Sell
    }

    public class MarketHistory
    {
        public TransactionType Type { get; set; }
        public bool ToServer { get; set; }

        public Guid BuyerId { get; set; }
        public Guid? SellerId { get; set; }

        public Guid ItemId { get; set; }

        public int Quantity { get; set; }
        public int Price { get; set; }

        public DateTime BoughtAt { get; set; }
    }
}
