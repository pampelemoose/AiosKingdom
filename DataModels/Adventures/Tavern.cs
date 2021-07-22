using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public class Tavern : AVersionized
    {
        [Key]
        public Guid Id { get; set; }

        public List<ShopItem> ShopItems { get; set; }

        public int RestShardCost { get; set; }
        public int RestStamina { get; set; }

        public int FoodCost { get; set; }
        public int FoodHealth { get; set; }
    }
}
