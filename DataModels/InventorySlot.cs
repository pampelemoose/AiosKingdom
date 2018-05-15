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

        [Required]
        public Guid SoulId { get; set; }

        [Required]
        public Items.ItemType Type { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        private int _quantity = 1;
        [Required]
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
    }
}
