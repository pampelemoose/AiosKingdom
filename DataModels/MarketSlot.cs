using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class MarketSlot
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ServerId { get; set; }

        public Items.ItemType Type { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        public Guid? SellerId { get; set; }
        [ForeignKey("SellerId")]
        public Soul Seller { get; set; }

        private int _quantity = -1;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        private int _shardPrice = 0;
        public int ShardPrice
        {
            get { return _shardPrice; }
            set { _shardPrice = value; }
        }

        private int _bitPrice = 0;
        public int BitPrice
        {
            get { return _bitPrice; }
            set { _bitPrice = value; }
        }
    }
}
