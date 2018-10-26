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
                return context.Weapons.ToList();
            }
        }

        public static List<DataModels.Items.Weapon> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Weapons.Include(a => a.Stats).Where(b => b.VersionId.Equals(versionId)).ToList();
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

        public static bool Update(DataModels.Items.Weapon weapon)
        {
            using (var context = new AiosKingdomContext())
            {
                var oldWeapon = context.Weapons
                    .Include(a => a.Stats)
                    .FirstOrDefault(u => u.Id.Equals(weapon.Id));

                if (oldWeapon == null)
                    return false;

                oldWeapon.VersionId = weapon.VersionId;
                oldWeapon.Name = weapon.Name;
                oldWeapon.Description = weapon.Description;
                oldWeapon.Image = weapon.Image;
                oldWeapon.Quality = weapon.Quality;
                oldWeapon.ItemLevel = weapon.ItemLevel;
                oldWeapon.HandlingType = weapon.HandlingType;
                oldWeapon.WeaponType = weapon.WeaponType;
                oldWeapon.UseLevelRequired = weapon.UseLevelRequired;
                oldWeapon.Space = weapon.Space;
                oldWeapon.SellingPrice = weapon.SellingPrice;
                oldWeapon.MinDamages = weapon.MinDamages;
                oldWeapon.MaxDamages = weapon.MaxDamages;

                context.ItemStats.RemoveRange(oldWeapon.Stats);

                oldWeapon.Stats = weapon.Stats;

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
