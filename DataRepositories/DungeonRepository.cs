using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class DungeonRepository
    {
        public static List<DataModels.Dungeons.Dungeon> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Dungeons.ToList();
            }
        }

        public static List<DataModels.Dungeons.Dungeon> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Dungeons.Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static DataModels.Dungeons.Dungeon GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Dungeons
                    .Include(a => a.Rooms)
                    .Include(a => a.Rooms.Select(r => r.ShopItems))
                    .Include(a => a.Rooms.Select(r => r.Ennemies))
                    .FirstOrDefault(a => a.DungeonId.Equals(id));
            }
        }

        public static bool Create(DataModels.Dungeons.Dungeon dungeon)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Dungeons.FirstOrDefault(u => u.Name.Equals(dungeon.Name)) != null)
                    return false;

                if (dungeon.Id.Equals(Guid.Empty))
                    return false;

                context.Dungeons.Add(dungeon);
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var error in e.EntityValidationErrors)
                    {
                        foreach (var mess in error.ValidationErrors)
                        {
                            Console.WriteLine(mess.ErrorMessage);
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        public static bool Update(DataModels.Dungeons.Dungeon dungeon)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Dungeons
                    .Include(a => a.Rooms)
                    .Include(a => a.Rooms.Select(r => r.ShopItems))
                    .Include(a => a.Rooms.Select(r => r.Ennemies))
                    .FirstOrDefault(a => a.Id.Equals(dungeon.Id));

                if (online == null)
                    return false;

                online.VersionId = dungeon.VersionId;
                online.Name = dungeon.Name;
                online.RequiredLevel = dungeon.RequiredLevel;
                online.MaxLevelAuthorized = dungeon.MaxLevelAuthorized;
                online.ExperienceReward = dungeon.ExperienceReward;
                online.ShardReward = dungeon.ShardReward;

                // ROOMS
                var oldRooms = online.Rooms;
                online.Rooms = new List<DataModels.Dungeons.Room>();
                foreach (var room in dungeon.Rooms)
                {
                    if (Guid.Empty.Equals(room.Id))
                    {
                        room.Id = Guid.NewGuid();
                        online.Rooms.Add(room);
                    }
                    else
                    {
                        var onlineRoom = context.Rooms
                            .Include(i => i.ShopItems)
                            .Include(i => i.Ennemies)
                            .FirstOrDefault(i => i.Id.Equals(room.Id));
                        onlineRoom.Type = room.Type;
                        onlineRoom.RoomNumber = room.RoomNumber;

                        // SHOPITEMS
                        var oldShopItems = onlineRoom.ShopItems;
                        onlineRoom.ShopItems = new List<DataModels.Dungeons.ShopItem>();
                        foreach (var shopItem in room.ShopItems)
                        {
                            if (Guid.Empty.Equals(shopItem.Id))
                            {
                                shopItem.Id = Guid.NewGuid();
                                onlineRoom.ShopItems.Add(shopItem);
                            }
                            else
                            {
                                var onlineShopItem = context.ShopItems.FirstOrDefault(i => i.Id.Equals(shopItem.Id));
                                onlineShopItem.ItemId = shopItem.ItemId;
                                onlineShopItem.Type = shopItem.Type;
                                onlineShopItem.Quantity = shopItem.Quantity;
                                onlineShopItem.ShardPrice = shopItem.ShardPrice;
                                onlineRoom.ShopItems.Add(onlineShopItem);
                                oldShopItems.Remove(oldShopItems.FirstOrDefault(o => o.Id.Equals(shopItem.Id)));
                            }
                        }

                        foreach (var toDel in oldShopItems)
                        {
                            context.ShopItems.Remove(toDel);
                        }

                        // ENEMIES
                        var oldEnemies = onlineRoom.Ennemies;
                        onlineRoom.Ennemies = new List<DataModels.Dungeons.Enemy>();
                        foreach (var enemy in room.Ennemies)
                        {
                            if (Guid.Empty.Equals(enemy.Id))
                            {
                                enemy.Id = Guid.NewGuid();
                                onlineRoom.Ennemies.Add(enemy);
                            }
                            else
                            {
                                var onlineEnemy = context.Enemies.FirstOrDefault(i => i.Id.Equals(enemy.Id));
                                onlineEnemy.EnemyType = enemy.EnemyType;
                                onlineEnemy.MonsterId = enemy.MonsterId;
                                onlineEnemy.Level = enemy.Level;
                                onlineEnemy.ShardReward = enemy.ShardReward;
                                onlineRoom.Ennemies.Add(onlineEnemy);
                                oldEnemies.Remove(oldEnemies.FirstOrDefault(o => o.Id.Equals(enemy.Id)));
                            }
                        }

                        foreach (var toDel in oldShopItems)
                        {
                            context.ShopItems.Remove(toDel);
                        }

                        online.Rooms.Add(onlineRoom);
                        oldRooms.Remove(oldRooms.FirstOrDefault(o => o.Id.Equals(room.Id)));
                    }
                }

                foreach (var toDel in oldRooms)
                {
                    context.Rooms.Remove(toDel);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var error in e.EntityValidationErrors)
                    {
                        foreach (var mess in error.ValidationErrors)
                        {
                            Console.WriteLine(mess.ErrorMessage);
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        public static bool SaveProgress(Guid soulId, Guid dungeonId, int currentRoom)
        {
            using (var context = new AiosKingdomContext())
            {
                var progressExists = context.DungeonProgresses.FirstOrDefault(p => p.SoulId.Equals(soulId) && p.DungeonId.Equals(dungeonId));
                if (progressExists != null)
                {
                    progressExists.CurrentRoom = currentRoom;
                }
                else
                {
                    var progress = new DataModels.DungeonProgress
                    {
                        Id = Guid.NewGuid(),
                        SoulId = soulId,
                        DungeonId = dungeonId,
                        CurrentRoom = currentRoom
                    };
                    context.DungeonProgresses.Add(progress);
                }

                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var error in e.EntityValidationErrors)
                    {
                        foreach (var mess in error.ValidationErrors)
                        {
                            Console.WriteLine(mess.ErrorMessage);
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
