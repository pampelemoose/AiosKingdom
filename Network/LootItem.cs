using System;
using System.Collections.Generic;
using System.Text;

namespace Network
{
    public class LootItem
    {
        public Guid LootId { get; set; }
        public Guid MonsterId { get; set; }
        public string Type { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
