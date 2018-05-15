using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class BagRepository
    {
        public static List<DataModels.Items.Bag> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Bags.Include(a => a.Stats).ToList();
            }
        }

        public static DataModels.Items.Bag GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Bags.Include(a => a.Stats).FirstOrDefault(b => b.ItemId.Equals(id));
            }
        }

        public static bool Create(DataModels.Items.Bag bag)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Bags.FirstOrDefault(u => u.Name.Equals(bag.Name)) != null)
                    return false;

                if (bag.Id.Equals(Guid.Empty))
                    return false;

                context.Bags.Add(bag);
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
