using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataModels
{
    public class InventorySlot
    {
        [Key]
        public Guid Id { get; set; }

        public Guid SoulId { get; set; }
        [ForeignKey("SoulId")]
        public Soul Soul { get; set; }

        [Required]
        public Items.ItemType Type { get; set; }

        public Guid ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Items.AItem Item { get; set; }

        private int _quantity = 1;
        [Required]
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }
}
