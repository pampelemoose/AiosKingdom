using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public class LootingHistoryRepository
    {
        public static List<DataModels.Items.LootHistory> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.LootHistory.ToList();
            }
        }

        public static List<DataModels.Items.LootHistory> GetAllForSoul(Guid soulId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.LootHistory.Where(m => m.LooterId.Equals(soulId)).ToList();
            }
        }

        public static DataModels.Items.LootHistory GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.LootHistory.FirstOrDefault(a => a.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.Items.LootHistory history)
        {
            using (var context = new AiosKingdomContext())
            {
                context.LootHistory.Add(history);
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
