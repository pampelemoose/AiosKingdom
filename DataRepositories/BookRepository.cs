using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepositories
{
    public static class BookRepository
    {
        public static List<DataModels.Skills.Book> GetAll()
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Books
                    .Include(a => a.Pages)
                    .Include(a => a.Pages.Select(i => i.Inscriptions))
                    .ToList();
            }
        }

        public static DataModels.Skills.Book GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Books
                    .Include(a => a.Pages)
                    .Include(a => a.Pages.Select(i => i.Inscriptions))
                    .FirstOrDefault(a => a.BookId.Equals(id));
            }
        }

        public static bool Create(DataModels.Skills.Book book)
        {
            using (var context = new AiosKingdomContext())
            {
                if (context.Books.FirstOrDefault(u => u.Name.Equals(book.Name)) != null)
                    return false;

                if (book.Id.Equals(Guid.Empty))
                    return false;

                context.Books.Add(book);
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
