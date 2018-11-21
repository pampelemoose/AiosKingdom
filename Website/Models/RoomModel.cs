using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class RoomModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Type")]
        public DataModels.Adventures.RoomType Type { get; set; }

        [Required(ErrorMessage = "RoomNumber required")]
        [Display(Name = "RoomNumber")]
        [Range(0, 1000, ErrorMessage = "RoomNumber should be >= 0")]
        public int RoomNumber { get; set; }

        [Display(Name = "New Items")]
        [Range(0, 1000000)]
        public int NewItems { get; set; }

        public List<ShopItemModel> ShopItems { get; set; }

        [Display(Name = "New Enemies")]
        [Range(0, 1000000)]
        public int NewEnemies { get; set; }

        public List<EnemyModel> Enemies { get; set; }
    }
}