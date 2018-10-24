using System;
using System.Collections.Generic;
using System.Text;

namespace JsonObjects.Adventures
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

    public enum EnemyType
    {
        Normal = 0,
        Elite = 1,
        Boss = 2
    }

    public class Dungeon
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int MaxLevelAuthorized { get; set; }

        public List<Room> Rooms { get; set; }

        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }
    }

    public class Room
    {
        public RoomType Type { get; set; }

        public int RoomNumber { get; set; }

        public List<ShopItem> ShopItems { get; set; }
        public List<Enemy> Ennemies { get; set; }
    }

    public class ShopItem
    {
        public Guid ItemId { get; set; }

        public Items.ItemType Type { get; set; }

        public int Quantity { get; set; }
        public int ShardPrice { get; set; }
    }

    public class Enemy
    {
        public Guid MonsterId { get; set; }

        public EnemyType EnemyType { get; set; }

        public int Level { get; set; }
        public int ShardReward { get; set; }
    }
}
