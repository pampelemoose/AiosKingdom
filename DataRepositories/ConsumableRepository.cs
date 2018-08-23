using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class ConsumableRepository
    {
        public static List<DataModels.Items.Consumable> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Consumables.Include(a => a.Effects).ToList();
            }
        }

        public static DataModels.Items.Consumable GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Consumables.Include(a => a.Effects).FirstOrDefault(c => c.ItemId.Equals(id));
            }
        }

        public static bool Create(DataModels.Items.Consumable consumable)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Consumables.FirstOrDefault(u => u.Name.Equals(consumable.Name)) != null)
                    return false;

                if (consumable.Id.Equals(Guid.Empty))
                    return false;

                context.Consumables.Add(consumable);
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

        public static bool Update(DataModels.Items.Consumable consumable)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Consumables
                    .Include(a => a.Effects)
                    .FirstOrDefault(u => u.Id.Equals(consumable.Id));

                if (online == null)
                    return false;

                online.VersionId = consumable.VersionId;
                online.Name = consumable.Name;
                online.Description = consumable.Description;
                online.Image = consumable.Image;
                online.ItemLevel = consumable.ItemLevel;
                online.Quality = consumable.Quality;
                online.UseLevelRequired = consumable.UseLevelRequired;
                online.Space = consumable.Space;
                online.SellingPrice = consumable.SellingPrice;

                context.ConsumableEffects.RemoveRange(online.Effects);

                online.Effects = consumable.Effects;

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
