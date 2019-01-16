using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class AdventureRepository
    {
        public static List<DataModels.Adventures.Adventure> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Adventures.ToList();
            }
        }

        public static List<DataModels.Adventures.Adventure> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Adventures
                    .Include(a => a.Rooms)
                    .Include(a => a.Locks)
                    .Include(a => a.Rooms.Select(r => r.ShopItems))
                    .Include(a => a.Rooms.Select(r => r.Ennemies))
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static DataModels.Adventures.Adventure GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Adventures
                    .Include(a => a.Rooms)
                    .Include(a => a.Locks)
                    .Include(a => a.Rooms.Select(r => r.ShopItems))
                    .Include(a => a.Rooms.Select(r => r.Ennemies))
                    .FirstOrDefault(a => a.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.Adventures.Adventure dungeon)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Adventures.FirstOrDefault(u => u.Name.Equals(dungeon.Name)) != null)
                    return false;

                if (dungeon.Id.Equals(Guid.Empty))
                    return false;

                context.Adventures.Add(dungeon);
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

        public static bool Update(DataModels.Adventures.Adventure adventure)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Adventures
                    .Include(a => a.Rooms)
                    .Include(a => a.Locks)
                    .Include(a => a.Rooms.Select(r => r.ShopItems))
                    .Include(a => a.Rooms.Select(r => r.Ennemies))
                    .FirstOrDefault(a => a.Id.Equals(adventure.Id));

                if (online == null)
                    return false;

                online.VersionId = adventure.VersionId;
                online.Name = adventure.Name;
                online.RequiredLevel = adventure.RequiredLevel;
                online.MaxLevelAuthorized = adventure.MaxLevelAuthorized;
                online.ExperienceReward = adventure.ExperienceReward;
                online.ShardReward = adventure.ShardReward;

                // ROOMS
                var oldRooms = online.Rooms;
                online.Rooms = new List<DataModels.Adventures.Room>();
                foreach (var room in adventure.Rooms)
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
                        onlineRoom.ShopItems = new List<DataModels.Adventures.ShopItem>();
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
                                onlineShopItem.ItemVid = shopItem.ItemVid;
                                onlineShopItem.Type = shopItem.Type;
                                onlineShopItem.Quantity = shopItem.Quantity;
                                onlineShopItem.Price = shopItem.Price;
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
                        onlineRoom.Ennemies = new List<DataModels.Adventures.Enemy>();
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
                                onlineEnemy.MonsterVid = enemy.MonsterVid;
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

                // LOCKS
                var oldLocks = online.Locks;
                online.Locks = new List<DataModels.Adventures.Lock>();
                foreach (var lockItem in adventure.Locks)
                {
                    if (Guid.Empty.Equals(lockItem.Id))
                    {
                        lockItem.Id = Guid.NewGuid();
                        online.Locks.Add(lockItem);
                    }
                    else
                    {
                        var onlineLock = context.Locks.FirstOrDefault(i => i.Id.Equals(lockItem.Id));
                        onlineLock.LockedId = lockItem.LockedId;
                        oldLocks.Remove(oldLocks.FirstOrDefault(o => o.Id.Equals(lockItem.Id)));
                    }
                }

                foreach (var toDel in oldLocks)
                {
                    context.Locks.Remove(toDel);
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
