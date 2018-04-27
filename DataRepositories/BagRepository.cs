using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class BagRepository
    {
        private static DispatchDbContext _context = new DispatchDbContext();

        public static List<DataModels.Items.Bag> GetAll()
        {
            return _context.Bags.Include(a => a.Stats).ToList();
        }

        public static bool Create(DataModels.Items.Bag bag)
        {
            if (_context.Bags.FirstOrDefault(u => u.Name.Equals(bag.Name)) != null)
                return false;

            if (bag.Id.Equals(Guid.Empty))
                return false;

            _context.Bags.Add(bag);
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
