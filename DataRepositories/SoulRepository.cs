using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace DataRepositories
{
    public static class SoulRepository
    {
        public static List<DataModels.Soul> GetSoulsByUserId(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Souls
                    .Where(s => s.UserId.Equals(id)).ToList();
            }
        }

        public static DataModels.Soul GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Souls
                    .Include(s => s.Equipment)
                    .Include(s => s.Inventory)
                    .FirstOrDefault(s => s.Id.Equals(id));
            }
        }

        public static bool CreateSoul(DataModels.Soul soul)
        {
            using (var context = new AiosKingdomContext())
            {
                var nameUsed = context.Souls.FirstOrDefault(s => s.Name.Equals(soul.Name));
                if (nameUsed != null)
                {
                    return false;
                }

                if (soul.Id.Equals(Guid.Empty))
                {
                    return false;
                }

                context.Souls.Add(soul);
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

        public static bool Update(DataModels.Soul soul)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Souls.FirstOrDefault(s => s.Id.Equals(soul.Id));
                if (online == null) return false;

                online.TimePlayed = soul.TimePlayed;

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
