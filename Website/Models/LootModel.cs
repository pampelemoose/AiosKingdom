using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class LootModel
    {
        [Required]
        [Range(0, 100)]
        public double DropRate { get; set; }

        [Display(Name = "Type")]
        public DataModels.Items.ItemType Type { get; set; }

        public Guid SelectedItem { get; set; }
        [Display(Name = "Item")]
        public List<DataModels.Items.AItem> Items
        {
            get
            {
                var items = new List<DataModels.Items.AItem>();

                var consumables = DataRepositories.ConsumableRepository.GetAll();
                foreach (var cons in consumables)
                {
                    items.Add(cons);
                }

                var armors = DataRepositories.ArmorRepository.GetAll();
                foreach (var arm in armors)
                {
                    items.Add(arm);
                }

                var bags = DataRepositories.BagRepository.GetAll();
                foreach (var bag in bags)
                {
                    items.Add(bag);
                }

                return items;
            }
        }

        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
    }
}