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
