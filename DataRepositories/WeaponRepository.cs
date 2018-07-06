using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class WeaponRepository
    {
        public static List<DataModels.Items.Weapon> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Weapons.Include(a => a.Stats).ToList();
            }
        }

        public static DataModels.Items.Weapon GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Weapons.Include(a => a.Stats).FirstOrDefault(a => a.ItemId.Equals(id));
            }
        }

        public static bool Create(DataModels.Items.Weapon weapon)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Weapons.FirstOrDefault(u => u.Name.Equals(weapon.Name)) != null)
                    return false;

                if (weapon.Id.Equals(Guid.Empty))
                    return false;

                context.Weapons.Add(weapon);
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
