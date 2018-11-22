using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public enum TransactionType
    {
        Buy,
        Sell
    }

    public class MarketHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public TransactionType Type { get; set; }
        [Required]
        public bool ToServer { get; set; }

        [Required]
        public Guid BuyerId { get; set; }
        public Guid? SellerId { get; set; }

        [Required]
        public Guid MarketId { get; set; }
        [Required]
        public Guid ItemId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int Price { get; set; }

        [Required]
        public DateTime BoughtAt { get; set; }
    }
}
