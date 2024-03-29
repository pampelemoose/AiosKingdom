﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class ShopItem
    {
        [Key]
        public Guid Id { get; set; }

        public Guid TavernId { get; set; }
        public Guid ItemVid { get; set; }

        public Items.ItemType Type { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
