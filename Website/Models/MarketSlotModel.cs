using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class MarketSlotModel
    {
        public Guid SelectedServer { get; set; }
        [Display(Name = "Server")]
        public List<DataModels.GameServer> Servers { get; set; }

        public Guid SelectedItem { get; set; }
        [Display(Name = "Item")]
        public List<DataModels.Items.AItem> Items { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "ShardPrice")]
        public int ShardPrice { get; set; }

        [Display(Name = "BitPrice")]
        public int BitPrice { get; set; }
    }
}