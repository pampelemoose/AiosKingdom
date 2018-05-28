using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels.Dungeons
{
    public enum RoomType
    {
        Normal = 0,
        Rest = 1,
        Shop = 2,
        Boss = 3
    }

    public class Room
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid DungeonId { get; set; }

        [Required]
        public RoomType Type { get; set; }

        [Required]
        [Range(0, 5)]
        public int Position { get; set; }

        public List<ShopItem> ShopItems { get; set; }
        public List<Monsters.Monster> Monsters { get; set; }
    }
}
