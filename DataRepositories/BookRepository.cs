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
                    .Include(a => a.Inscriptions)
                    .Include(a => a.Talents)
                    .ToList();
            }
        }

        public static List<DataModels.Skills.Book> GetAllForVersion(Guid versionId)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Books
                    .Include(a => a.Inscriptions)
                    .Include(a => a.Talents)
                    .Where(b => b.VersionId.Equals(versionId))
                    .ToList();
            }
        }

        public static DataModels.Skills.Book GetById(Guid id)
        {
            using (var context = new AiosKingdomContext())
            {
                return context.Books
                    .Include(a => a.Inscriptions)
                    .Include(a => a.Talents)
                    .FirstOrDefault(a => a.Id.Equals(id));
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
                    .Include(p => p.Inscriptions)
                    .Include(a => a.Talents)
                    .FirstOrDefault(u => u.Id.Equals(book.Id));

                if (online == null)
                    return false;

                online.VersionId = book.VersionId;
                online.Name = book.Name;
                online.Description = book.Description;
                online.Quality = book.Quality;
                online.EmberCost = book.EmberCost;
                online.ManaCost = book.ManaCost;
                online.Cooldown = book.Cooldown;

                // INSCRIPTIONS
                var oldInsc = online.Inscriptions;
                online.Inscriptions = new List<DataModels.Skills.Inscription>();
                foreach (var insc in book.Inscriptions)
                {
                    if (Guid.Empty.Equals(insc.Id))
                    {
                        insc.Id = Guid.NewGuid();
                        online.Inscriptions.Add(insc);
                    }
                    else
                    {
                        var onlineInsc = context.Inscriptions.FirstOrDefault(i => i.Id.Equals(insc.Id));
                        onlineInsc.Type = insc.Type;
                        onlineInsc.BaseValue = insc.BaseValue;
                        onlineInsc.StatType = insc.StatType;
                        onlineInsc.Ratio = insc.Ratio;
                        onlineInsc.Duration = insc.Duration;
                        onlineInsc.IncludeWeaponDamages = insc.IncludeWeaponDamages;
                        onlineInsc.WeaponTypes = insc.WeaponTypes;
                        onlineInsc.WeaponDamagesRatio = insc.WeaponDamagesRatio;
                        onlineInsc.PreferredWeaponTypes = insc.PreferredWeaponTypes;
                        onlineInsc.PreferredWeaponDamagesRatio = insc.PreferredWeaponDamagesRatio;
                        online.Inscriptions.Add(onlineInsc);
                        oldInsc.Remove(oldInsc.FirstOrDefault(o => o.Id.Equals(insc.Id)));
                    }
                }

                foreach (var toDel in oldInsc)
                {
                    context.Inscriptions.Remove(toDel);
                }

                // TALENTS
                var oldTalent = online.Talents;
                online.Talents = new List<DataModels.Skills.Talent>();
                foreach (var tal in book.Talents)
                {
                    if (Guid.Empty.Equals(tal.Id))
                    {
                        tal.Id = Guid.NewGuid();
                        online.Talents.Add(tal);
                    }
                    else
                    {
                        var onlineTal = context.Talents.FirstOrDefault(i => i.Id.Equals(tal.Id));
                        onlineTal.Branch = tal.Branch;
                        onlineTal.Leaf = tal.Leaf;
                        onlineTal.Unlocks = tal.Unlocks;
                        onlineTal.TargetInscription = tal.TargetInscription;
                        onlineTal.TalentPointsRequired = tal.TalentPointsRequired;
                        onlineTal.Type = tal.Type;
                        onlineTal.Value = tal.Value;
                        online.Talents.Add(onlineTal);
                        oldTalent.Remove(oldTalent.FirstOrDefault(o => o.Id.Equals(tal.Id)));
                    }
                }

                foreach (var toDel in oldTalent)
                {
                    context.Talents.Remove(toDel);
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
