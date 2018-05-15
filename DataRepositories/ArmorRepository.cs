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
    }
}
