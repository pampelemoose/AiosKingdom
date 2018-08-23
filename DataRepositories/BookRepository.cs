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

        public static bool Update(DataModels.Skills.Book book)
        {
            using (var context = new AiosKingdomContext())
            {
                var online = context.Books
                    .Include(p => p.Pages)
                    .Include(a => a.Pages.Select(i => i.Inscriptions))
                    .FirstOrDefault(u => u.Id.Equals(book.Id));

                if (online == null)
                    return false;

                online.VersionId = book.VersionId;
                online.Name = book.Name;
                online.Quality = book.Quality;

                // PAGES
                var oldPages = online.Pages;
                online.Pages = new List<DataModels.Skills.Page>();
                foreach (var page in book.Pages)
                {
                    if (Guid.Empty.Equals(page.Id))
                    {
                        page.Id = Guid.NewGuid();
                        online.Pages.Add(page);
                    }
                    else
                    {
                        var onlinePage = context.Pages
                            .Include(i => i.Inscriptions)
                            .FirstOrDefault(i => i.Id.Equals(page.Id));
                        onlinePage.Description = page.Description;
                        onlinePage.Image = page.Image;
                        onlinePage.Rank = page.Rank;
                        onlinePage.EmberCost = page.EmberCost;
                        onlinePage.ManaCost = page.ManaCost;
                        onlinePage.Cooldown = page.Cooldown;

                        // INSCRIPTIONS
                        var oldInsc = onlinePage.Inscriptions;
                        onlinePage.Inscriptions = new List<DataModels.Skills.Inscription>();
                        foreach (var insc in page.Inscriptions)
                        {
                            if (Guid.Empty.Equals(insc.Id))
                            {
                                insc.Id = Guid.NewGuid();
                                onlinePage.Inscriptions.Add(insc);
                            }
                            else
                            {
                                var onlineInsc = context.Inscriptions.FirstOrDefault(i => i.Id.Equals(insc.Id));
                                onlineInsc.Type = insc.Type;
                                onlineInsc.BaseValue = insc.BaseValue;
                                onlineInsc.StatType = insc.StatType;
                                onlineInsc.Ratio = insc.Ratio;
                                onlineInsc.IncludeWeaponDamages = insc.IncludeWeaponDamages;
                                onlineInsc.WeaponTypes = insc.WeaponTypes;
                                onlineInsc.WeaponDamagesRatio = insc.WeaponDamagesRatio;
                                onlineInsc.PreferredWeaponTypes = insc.PreferredWeaponTypes;
                                onlineInsc.PreferredWeaponDamagesRatio = insc.PreferredWeaponDamagesRatio;
                                onlinePage.Inscriptions.Add(onlineInsc);
                                oldInsc.Remove(oldInsc.FirstOrDefault(o => o.Id.Equals(insc.Id)));
                            }
                        }

                        foreach (var toDel in oldInsc)
                        {
                            context.Inscriptions.Remove(toDel);
                        }

                        online.Pages.Add(onlinePage);
                        oldPages.Remove(oldPages.FirstOrDefault(o => o.Id.Equals(page.Id)));
                    }
                }

                foreach (var toDel in oldPages)
                {
                    context.Pages.Remove(toDel);
                }

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
