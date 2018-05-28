using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Dungeons
{
    public class ShopItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public Guid ItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int ShardPrice { get; set; }
    }
}
