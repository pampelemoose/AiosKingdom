using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Adventures
{
    public enum RoomType
    {
        Fight = 0,
        Rest = 1,
        Shop = 2,
        Elite = 3,
        Boss = 4,
        Exit = 5
    }

    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AdventureId { get; set; }

        public RoomType Type { get; set; }
        public int RoomNumber { get; set; }

        public List<ShopItem> ShopItems { get; set; }
        public List<Enemy> Ennemies { get; set; }
    }
}
