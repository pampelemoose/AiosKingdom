using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace DataRepositories
{
    public static class ItemRepository
    {
        public static List<DataModels.Items.Item> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Items.ToList();
            }
        }

        public static List<DataModels.Items.Item> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Items
                    .Include(a => a.Stats)
                    .Include(a => a.Effects)
                    .Where(b => b.VersionId.Equals(versionId)).ToList();
            }
        }

        public static List<DataModels.Items.Item> GetAllNullableForQuality(DataModels.Items.ItemQuality quality)
        {
            using (var context = new AiosKingdomContext())
            {
                var result = new List<DataModels.Items.Item> { null };
                result.AddRange(context.Items.Where(i => i.Quality == quality).ToList());
                return result;
            }
        }

        public static DataModels.Items.Item GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Items
                    .Include(a => a.Stats)
                    .Include(a => a.Effects)
                    .FirstOrDefault(a => a.Id.Equals(id));
            }
        }

        public static bool Create(DataModels.Items.Item item)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Items.FirstOrDefault(u => u.Name.Equals(item.Name)) != null)
                    return false;

                if (item.Id.Equals(Guid.Empty))
                    return false;

                context.Items.Add(item);
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

        public static bool Update(DataModels.Items.Item item)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Items
                    .Include(s => s.Stats)
                    .Include(s => s.Effects)
                    .FirstOrDefault(u => u.Id.Equals(item.Id));

                if (online == null)
                    return false;

                online.VersionId = item.VersionId;
                online.Name = item.Name;
                online.Description = item.Description;
                online.ItemLevel = item.ItemLevel;
                online.Quality = item.Quality;
                online.UseLevelRequired = item.UseLevelRequired;
                online.Space = item.Space;
                online.SellingPrice = item.SellingPrice;

                context.ItemStats.RemoveRange(online.Stats);
                online.Stats = item.Stats;

                context.ItemEffects.RemoveRange(online.Effects);
                online.Effects = item.Effects;

                online.ArmorValue = item.ArmorValue;
                online.SlotCount = item.SlotCount;

                online.MinDamages = item.MinDamages;
                online.MaxDamages = item.MaxDamages;

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
