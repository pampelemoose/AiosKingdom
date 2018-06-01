﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Website.Models
{
    public class RoomModel
    {
        [Display(Name = "Type")]
        public DataModels.Dungeons.RoomType Type { get; set; }

        [Required(ErrorMessage = "RoomNumber required")]
        [Display(Name = "RoomNumber")]
        [Range(0, 1000, ErrorMessage = "RoomNumber should be >= 0")]
        public int RoomNumber { get; set; }

        public List<ShopItemModel> ShopItems { get; set; }

        public List<EnemyModel> Enemies { get; set; }
    }
}