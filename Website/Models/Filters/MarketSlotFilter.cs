using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models.Filters
{
    public class MarketSlotFilter
    {
        [Display(Name = "Server")]
        public Guid SelectedServer { get; set; }

        [Display(Name = "Type")]
        public DataModels.Items.ItemType? Type { get; set; }

        public List<DataModels.MarketSlot> Slots { get; set; }

        public List<DataModels.MarketSlot> FilterList(List<DataModels.MarketSlot> list)
        {
            if (!Guid.Empty.Equals(SelectedServer))
            {
                list = list.Where(a => a.ServerId.Equals(SelectedServer)).ToList();
            }

            if (Type != null)
            {
                list = list.Where(a => a.Type.Equals(Type)).ToList();
            }

            return list;
        }
    }
}