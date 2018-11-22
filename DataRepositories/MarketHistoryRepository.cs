using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class MarketHistoryRepository
    {
        public static List<DataModels.MarketHistory> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.MarketHistory.ToList();
            }
        }

        public static List<DataModels.MarketHistory> GetAllForSoul(Guid soulId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.MarketHistory.Where(m => m.BuyerId.Equals(soulId) || m.SellerId.Equals(soulId)).ToList();
            }
        }

        public static DataModels.MarketHistory GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.MarketHistory.FirstOrDefault(a => a.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.MarketHistory history)
        {
            using (var context = new AiosKingdomContext())
            {
                if (history.Id.Equals(Guid.Empty))
                    return false;

                context.MarketHistory.Add(history);
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
