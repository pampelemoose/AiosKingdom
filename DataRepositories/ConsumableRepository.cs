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
        private static DispatchDbContext _context = new DispatchDbContext();

        public static List<DataModels.Items.Consumable> GetAll()
        {
            return _context.Consumables.Include(a => a.Effects).ToList();
        }

        public static bool Create(DataModels.Items.Consumable consumable)
        {
            if (_context.Consumables.FirstOrDefault(u => u.Name.Equals(consumable.Name)) != null)
                return false;

            if (consumable.Id.Equals(Guid.Empty))
                return false;

            _context.Consumables.Add(consumable);
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
