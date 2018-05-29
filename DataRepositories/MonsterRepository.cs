using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public class MonsterRepository
    {
        public static List<DataModels.Monsters.Monster> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Monsters
                    .Include(a => a.Phases)
                    .ToList();
            }
        }

        public static DataModels.Monsters.Monster GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Monsters
                    .Include(a => a.Loots)
                    .Include(a => a.Phases)
                    .FirstOrDefault(a => a.MonsterId.Equals(id));
            }
        }

        public static bool Create(DataModels.Monsters.Monster monster)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Monsters.FirstOrDefault(u => u.Name.Equals(monster.Name)) != null)
                    return false;

                if (monster.Id.Equals(Guid.Empty))
                    return false;

                context.Monsters.Add(monster);
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
