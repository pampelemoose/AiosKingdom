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

        [Required]
        public Guid ItemId { get; set; }

        public Guid? SellerId { get; set; }
        [ForeignKey("SellerId")]
        public Soul Seller { get; set; }

        [Required]
        public bool IsSpecial { get; set; }

        private int _quantity = -1;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        private int _price = 0;
        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }
    }
}
