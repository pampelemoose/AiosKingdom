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
        private static AiosKingdomContext _context = new AiosKingdomContext();

        public static List<DataModels.Soul> GetSoulsByUserId(Guid id)
        {
            return _context.Souls
                .Include(s => s.Equipment)
                .Include(s => s.Inventory)
                .Where(s => s.UserId.Equals(id)).ToList();
        }

        public static bool CreateSoul(DataModels.Soul soul)
        {
            var nameUsed = _context.Souls.FirstOrDefault(s => s.Name.Equals(soul.Name));
            if (nameUsed != null)
            {
                return false;
            }

            if (soul.Id.Equals(Guid.Empty))
            {
                return false;
            }

            _context.Souls.Add(soul);
            try
            {
                _context.SaveChanges();
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
