using System;
using System.Collections.Generic;
using System.Text;

namespace Network.Adventures
{
    public enum ObjectiveType
    {
        EnemyKill = 0,
        NpcDialogue = 1,
        ExploreArea = 2
    }

    public enum EnemyType
    {
        Normal = 0,
        Elite = 1,
        Boss = 2,
        Rare = 3
    }

    public class Adventure
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int RequiredLevel { get; set; }
        public int MaxLevelAuthorized { get; set; }

        public List<Quest> Quests { get; set; }

        public Guid MapIdentifier { get; set; }
        public int SpawnCoordinateX { get; set; }
        public int SpawnCoordinateY { get; set; }

        public int ExperienceReward { get; set; }
        public int ShardReward { get; set; }

        public List<Lock> Locks { get; set; }
    }

    public class Quest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<QuestObjective> Objectives { get; set; }
    }

    public class QuestObjective
    {
        public Guid Id { get; set; }
        public ObjectiveType Type { get; set; }
        public string Title { get; set; }
        public string DataContent { get; set; }
    }

    public class Tavern
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ShopItem> ShopItems { get; set; }

        public int RestShardCost { get; set; }
        public int RestStamina { get; set; }

        public int FoodCost { get; set; }
        public int FoodHealth { get; set; }
    }

    public class Bookstore
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<BookItem> Books { get; set; }
    }

    public class Lock
    {
        public Guid LockedId { get; set; }
    }

    public class ShopItem
    {
        public Guid ItemId { get; set; }

        public Items.ItemType Type { get; set; }

        public int Quantity { get; set; }
        public int ShardPrice { get; set; }
    }

    public class BookItem
    {
        public Guid BookId { get; set; }
    }

    public class Enemy
    {
        public Guid MonsterId { get; set; }

        public EnemyType EnemyType { get; set; }

        public int Level { get; set; }
        public int ShardReward { get; set; }
    }

    public class Npc
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<NpcDialogue> Dialogues { get; set; }
    }

    public class NpcDialogue
    {
        public string Content { get; set; }

        public List<NpcDialogue> NextDialogues { get; set; }
    }
}
