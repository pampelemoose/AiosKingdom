using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class ArmorRepository
    {
        public static List<DataModels.Items.Armor> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Armors.Include(a => a.Stats).ToList();
            }
        }

        public static DataModels.Items.Armor GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Armors.Include(a => a.Stats).FirstOrDefault(a => a.ItemId.Equals(id));
            }
        }

        public static bool Create(DataModels.Items.Armor armor)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Armors.FirstOrDefault(u => u.Name.Equals(armor.Name)) != null)
                    return false;

                if (armor.Id.Equals(Guid.Empty))
                    return false;

                context.Armors.Add(armor);
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

        public static bool Update(DataModels.Items.Armor armor)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Armors
                    .Include(s => s.Stats)
                    .FirstOrDefault(u => u.Id.Equals(armor.Id));

                if (online == null)
                    return false;

                online.VersionId = armor.VersionId;
                online.Name = armor.Name;
                online.Description = armor.Description;
                online.Image = armor.Image;
                online.ItemLevel = armor.ItemLevel;
                online.Quality = armor.Quality;
                online.UseLevelRequired = armor.UseLevelRequired;
                online.Space = armor.Space;
                online.SellingPrice = armor.SellingPrice;
                online.ArmorValue = armor.ArmorValue;

                context.ItemStats.RemoveRange(online.Stats);

                online.Stats = armor.Stats;

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
