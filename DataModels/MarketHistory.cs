using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public enum MarketTransactionType
    {
        Buy,
        Sell
    }

    public class MarketHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public MarketTransactionType Type { get; set; }
        public bool ToServer { get; set; }

        public Guid BuyerId { get; set; }
        public Guid? SellerId { get; set; }

        public Guid MarketId { get; set; }
        public Guid ItemVid { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }

        public DateTime BoughtAt { get; set; }
    }
}
