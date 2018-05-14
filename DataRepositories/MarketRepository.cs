using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class MarketRepository
    {
        public static List<DataModels.MarketSlot> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Market
                    .Include(a => a.Item)
                    .Include(a => a.Seller).ToList();
            }
        }

        public static DataModels.MarketSlot GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Market
                    .Include(a => a.Item)
                    .Include(a => a.Seller)
                    .FirstOrDefault(a => a.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.MarketSlot slot)
        {
            using (var context = new AiosKingdomContext())
            {
                // TODO : Check if item by seller exists
                /*if (context.Market.FirstOrDefault(u => u.Name.Equals(armor.Name)) != null)
                    return false;*/

                if (slot.Id.Equals(Guid.Empty))
                    return false;

                context.Market.Add(slot);
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

        public static bool Update(DataModels.MarketSlot slot)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Market.FirstOrDefault(i => i.Id.Equals(slot.Id));

                if (online == null) return false;

                online.Quantity = slot.Quantity;

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

        public static bool DeleteById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                var itemExists = context.Market.FirstOrDefault(i => i.Id.Equals(id));

                if (itemExists == null) return false;

                context.Market.Remove(itemExists);
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
