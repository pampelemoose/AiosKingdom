using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class ShopItemModel
    {
        public Guid Id { get; set; }

        public string RoomId { get; set; }

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

        [Required]
        [Range(1, 100000)]
        public int ShardPrice { get; set; }
    }
}