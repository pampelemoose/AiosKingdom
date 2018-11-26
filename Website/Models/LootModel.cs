using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class LootModel
    {
        public Guid Id { get; set; }

        [Required]
        [Range(0.00001, 100)]
        public double DropRate { get; set; }

        [Display(Name = "Type")]
        public DataModels.Items.ItemType Type { get; set; }

        public Guid SelectedItem { get; set; }
        [Display(Name = "Item")]
        public List<DataModels.Items.Item> Items
        {
            get
            {
                var items = DataRepositories.ItemRepository.GetAll().ToList();

                return items;
            }
        }

        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
    }
}