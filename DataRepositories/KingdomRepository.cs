using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class KingdomRepository
    {
        public static List<DataModels.Kingdom> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Kingdoms.ToList();
            }
        }

        public static DataModels.Kingdom GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Kingdoms
                    .Include(a => a.Towns)
                    .FirstOrDefault(a => a.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.Kingdom kingdom)
        {
            using (var context = new AiosKingdomContext())
            {
                if (kingdom.Id.Equals(Guid.Empty))
                    return false;

                context.Kingdoms.Add(kingdom);
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

        public static bool Update(DataModels.Kingdom kingdom)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Kingdoms.FirstOrDefault(i => i.Id.Equals(kingdom.Id));

                if (online == null) return false;

                online.LevelGap = kingdom.LevelGap;
                online.CurrentMaxLevel = kingdom.CurrentMaxLevel;
                online.MaxLevelCountForGap = kingdom.MaxLevelCountForGap;
                online.CurrentMaxLevelCount = kingdom.CurrentMaxLevelCount;

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
