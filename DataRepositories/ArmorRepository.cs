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
        private static DispatchDbContext _context = new DispatchDbContext();

        public static List<DataModels.Items.Armor> GetAll()
        {
            return _context.Armors.Include(a => a.Stats).ToList();
        }

        public static bool Create(DataModels.Items.Armor armor)
        {
            if (_context.Armors.FirstOrDefault(u => u.Name.Equals(armor.Name)) != null)
                return false;

            if (armor.Id.Equals(Guid.Empty))
                return false;

            _context.Armors.Add(armor);
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
