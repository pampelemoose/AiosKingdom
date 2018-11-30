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
        public Guid ItemId { get; set; }

        private int _quantity = 1;
        public int Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        public DateTime LootedAt { get; set; }
    }
}
