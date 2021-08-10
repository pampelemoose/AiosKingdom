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
                    .Include(a => a.Quests)
                    .Include(a => a.Quests.Select(q => q.Objectives))
                    .Include(a => a.Locks)
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static List<DataModels.Adventures.Tavern> GetAllTavernsForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Taverns
                    .Include(a => a.ShopItems)
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static List<DataModels.Adventures.Bookstore> GetAllBookstoresForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Bookstores
                    .Include(a => a.Books)
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static List<DataModels.Adventures.Npc> GetAllNpcsForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Npcs
                    .Include(a => a.Dialogues)
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static List<DataModels.Adventures.NpcDialogue> GetNextDialoguesForDialogue(Guid versionId, Guid diologueId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.NpcDialogues
                    .Where(b => b.VersionId.Equals(versionId) && b.Vid.Equals(diologueId)).ToList();
            }
        }

        public static List<DataModels.Adventures.Enemy> GetAllEnemiesForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Enemies
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static DataModels.Adventures.Adventure GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Adventures
                    .Include(a => a.Quests)
                    .Include(a => a.Locks)
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
                    .Include(a => a.Quests)
                    .Include(a => a.Locks)
                    .FirstOrDefault(a => a.Id.Equals(adventure.Id));

                if (online == null)
                    return false;

                online.VersionId = adventure.VersionId;
                online.Name = adventure.Name;
                online.RequiredLevel = adventure.RequiredLevel;
                online.MaxLevelAuthorized = adventure.MaxLevelAuthorized;
                online.ExperienceReward = adventure.ExperienceReward;
                online.ShardReward = adventure.ShardReward;

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
