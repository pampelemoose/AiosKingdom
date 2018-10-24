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

        public static List<DataModels.Items.Bag> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Bags.Include(a => a.Stats).Where(b => b.VersionId.Equals(versionId)).ToList();
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

        public static bool Update(DataModels.Items.Bag bag)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Bags
                    .Include(s => s.Stats)
                    .FirstOrDefault(u => u.Id.Equals(bag.Id));

                if (online == null)
                    return false;

                online.VersionId = bag.VersionId;
                online.Name = bag.Name;
                online.Description = bag.Description;
                online.Image = bag.Image;
                online.ItemLevel = bag.ItemLevel;
                online.Quality = bag.Quality;
                online.UseLevelRequired = bag.UseLevelRequired;
                online.SlotCount = bag.SlotCount;
                online.SellingPrice = bag.SellingPrice;

                context.ItemStats.RemoveRange(online.Stats);

                online.Stats = bag.Stats;

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
